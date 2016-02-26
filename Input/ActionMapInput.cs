using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	/*
	Things to test for action map / control schemes.

	- When pressing e.g. mouse button or gamepad trigger in one action map creates a new action map
	  based on the same device, the new action map should not immediately have wasJustPressed be true.
	  Hence the state in the newly created control scheme should be initialized to the state
	  of the devices it's based on.
	
	- When pressing e.g. mouse button or gamepad trigger and it causes a switch in control scheme
	  within an existing action map, the new control scheme *should* immediately have wasJustPressed be true.
	  Hence the state in the newly created control scheme should not be initialized to the state
	  of the devices it's based on.

	*/
	public class ActionMapInput : InputControlProvider
	{
		private ActionMap m_ActionMap;
		public ActionMap actionMap { get { return m_ActionMap; } }

		private ControlScheme m_ControlScheme;
		public ControlScheme controlScheme { get { return m_ControlScheme; } }

		private List<InputState> m_DeviceStates;
		private List<InputState> deviceStates { get { return m_DeviceStates; } }

		bool m_Active;
		public bool active {
			get {
				return m_Active;
			}
			set {
				m_Active = value;
				Reset(value);
			}
		}

		public static ActionMapInput Create(ActionMap actionMap)
		{
			ActionMapInput map =
				(ActionMapInput)Activator.CreateInstance(actionMap.customActionMapType, new object[] { actionMap });
			return map;
		}

		protected ActionMapInput(ActionMap actionMap)
		{
			m_ActionMap = actionMap;
		}

		public bool TryInitializeControlScheme(InputDevice inputDevice)
		{
			if (inputDevice.assignment == null)
				return TryInitializeControlSchemeGlobal();
			else
				return TryInitializeControlSchemeForPlayer(inputDevice.assignment.player);
		}

		public bool TryInitializeControlSchemeGlobal()
		{
			// The reverse is needed!
			// Although we check for control scheme with latest time stamp in function below,
			// we early out when we reach the first matching device of a given time,
			// so it's important that the first found device is the latest used one.
			var devices = InputSystem.leastToMostRecentlyUsedDevices.Where(e => e.assignment == null).Reverse().ToList();
			return Assign(devices);
		}

		public bool TryInitializeControlSchemeForPlayer(PlayerHandle player)
		{
			var devices = player.assignments.Select(e => e.device).ToList();
			return Assign(devices);
		}

		public bool Assign(List<InputDevice> availableDevices)
		{
			int bestScheme = -1;
			List<InputDevice> bestFoundDevices = null;
			float mostRecentTime = -1;

			List<InputDevice> foundDevices = new List<InputDevice>();
			for (int scheme = 0; scheme < actionMap.controlSchemes.Count; scheme++)
			{
				float timeForScheme = -1;
				foundDevices.Clear();
				var types = actionMap.controlSchemes[scheme].deviceTypes;
				bool matchesAll = true;
				foreach (var type in types)
				{
					bool foundMatch = false;
					foreach (var device in availableDevices)
					{
						if (type.IsInstanceOfType(device))
						{
							foundDevices.Add(device);
							foundMatch = true;
							timeForScheme = Mathf.Max(timeForScheme, device.lastEventTime);
							break;
						}
					}
					
					if (!foundMatch)
					{
						matchesAll = false;
						break;
					}
				}
				if (!matchesAll)
					continue;

				// If we reach this point we know that control scheme both matches required and matches all.
				if (timeForScheme > mostRecentTime)
				{
					bestScheme = scheme;
					bestFoundDevices = new List<InputDevice>(foundDevices);
					mostRecentTime = timeForScheme;
				}
			}

			if (bestScheme == -1)
				return false;
			
			ControlScheme matchingControlScheme = actionMap.controlSchemes[bestScheme];
			Assign(matchingControlScheme, bestFoundDevices);
			return true;
		}

		private void Assign(ControlScheme controlScheme, List<InputDevice> devices)
		{
			m_ControlScheme = controlScheme;

			// Create state for every device.
			var deviceStates = new List<InputState>();
			foreach (var device in devices)
				deviceStates.Add(new InputState(device));
			m_DeviceStates = deviceStates;

			// Create list of controls from ActionMap.
			////REVIEW: doesn't handle compounds
			var controls = new List<InputControlData>();
			foreach (var entry in actionMap.actions)
				controls.Add(entry.controlData);
			SetControls(controls);

			Reset();

			if (PlayerHandle.onChange != null)
				PlayerHandle.onChange.Invoke();
		}

		public override bool ProcessEvent(InputEvent inputEvent)
		{
			if (ProcessEventForScheme(inputEvent))
				return true;

			if (deviceStates.Any(e => e.controlProvider == inputEvent.device))
				return false;

			if (!TryInitializeControlScheme(inputEvent.device))
				return false;

			// When changing control scheme, we do not want to init control scheme to device states
			// like we normally want, so do a hard reset here, before processing the new event.
			Reset(false);

			return ProcessEventForScheme(inputEvent);
		}

		private bool ProcessEventForScheme(InputEvent inputEvent)
		{
			var consumed = false;
			
			// Update device state (if event actually goes to one of the devices we talk to).
			foreach (var deviceState in deviceStates)
			{
				////FIXME: should refer to proper type
				var device = (InputDevice)deviceState.controlProvider;
				
				// Skip state if event is not meant for device associated with it.
				if (device != inputEvent.device)
					continue;
				
				// Give device a stab at converting the event into state.
				if (device.ProcessEventIntoState(inputEvent, deviceState))
				{
					consumed = true;
					break;
				}
			}
			
			if (!consumed)
				return false;
			
			return true;
		}

		public void Reset(bool initToDeviceState = true)
		{
			if (initToDeviceState)
			{
				foreach (var deviceState in deviceStates)
					deviceState.InitToDevice();
				
				ExtractCurrentValuesFromSources();

				// Copy current values into prev values.
				state.BeginFrame();
			}
			else
			{
				foreach (var deviceState in deviceStates)
					deviceState.Reset();
				state.Reset();
			}
		}

		public List<InputDevice> GetCurrentlyUsedDevices()
		{
			List<InputDevice> list = new List<InputDevice>();
			for (int i = 0; i < deviceStates.Count; i++)
				list.Add(deviceStates[i].controlProvider as InputDevice);
			return list;
		}

		private InputState GetDeviceStateForDeviceType(Type deviceType)
		{
			foreach (var deviceState in deviceStates)
			{
				if (deviceType.IsInstanceOfType(deviceState.controlProvider))
					return deviceState;
			}
			throw new ArgumentException("deviceType");
		}

		public void BeginFrame()
		{
			state.BeginFrame();
			foreach (var deviceState in deviceStates)
				deviceState.BeginFrame();
		}
		
		public void EndFrame()
		{
			ExtractCurrentValuesFromSources();
		}

		private void ExtractCurrentValuesFromSources()
		{
			for (var entryIndex = 0; entryIndex < actionMap.actions.Count; ++ entryIndex)
			{
				var binding = controlScheme.bindings[entryIndex];
				
				var controlValue = 0.0f;
				foreach (var source in binding.sources)
				{
					var value = GetSourceValue(source);
					if (Mathf.Abs(value) > Mathf.Abs(controlValue))
						controlValue = value;
				}
				
				foreach (var axis in binding.buttonAxisSources)
				{
					var negativeValue = GetSourceValue(axis.negative);
					var positiveValue = GetSourceValue(axis.positive);
					var value = positiveValue - negativeValue;
					if (Mathf.Abs(value) > Mathf.Abs(controlValue))
						controlValue = value;
				}
				
				state.SetCurrentValue(entryIndex, controlValue);
			}
		}

		private float GetSourceValue(InputControlDescriptor source)
		{
			var deviceState = GetDeviceStateForDeviceType(source.deviceType);
			return deviceState.GetCurrentValue(source.controlIndex);
		}

		public override string GetPrimarySourceName(int controlIndex, string buttonAxisFormattingString = "{0} & {1}")
		{
			var binding = controlScheme.bindings[controlIndex];
			
			if (binding.primaryIsButtonAxis && binding.buttonAxisSources != null && binding.buttonAxisSources.Count > 0)
			{
				return string.Format(buttonAxisFormattingString,
					GetSourceName(binding.buttonAxisSources[0].negative),
					GetSourceName(binding.buttonAxisSources[0].positive));
			}
			else if (binding.sources != null && binding.sources.Count > 0)
			{
				return GetSourceName(binding.sources[0]);
			}
			return string.Empty;
		}

		private string GetSourceName(InputControlDescriptor source)
		{
			var deviceState = GetDeviceStateForDeviceType(source.deviceType);
			return deviceState.controlProvider.GetControlData(source.controlIndex).name;
		}

		////REVIEW: the descriptor may come from anywhere; method assumes we get passed some state we actually own
		////REVIEW: this mutates the state of the current instance but also mutates the shared ActionMap; that's bad
		public bool BindControl(InputControlDescriptor descriptor, InputControl control, bool restrictToExistingDevices)
		{
			bool existingDevice = false;
			for (int i = 0; i < m_DeviceStates.Count; i++)
			{
				if (control.provider == m_DeviceStates[i].controlProvider)
				{
					existingDevice = true;
					break;
				}
			}
			
			if (!existingDevice)
			{
				if (restrictToExistingDevices)
					return false;
				
				deviceStates.Add(new InputState(control.provider, new List<int>() { control.index }));
			}
			
			descriptor.controlIndex = control.index;
			descriptor.deviceType = control.provider.GetType();

			m_ControlScheme.customized = true;
			
			RefreshBindings();
			
			return true;
		}


		public void RevertCustomizations()
		{
		    ////FIXME: doesn't properly reset control scheme
			m_ActionMap.RevertCustomizations();
			RefreshBindings();
		}
	
		private void RefreshBindings()
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeUsedControlIndices = new Dictionary<Type, List<int>>();
			controlScheme.ExtractDeviceTypesAndControlIndices(perDeviceTypeUsedControlIndices);
			
			foreach (var deviceType in perDeviceTypeUsedControlIndices.Keys)
			{
				InputState state = GetDeviceStateForDeviceType(deviceType);
				state.SetUsedControls(perDeviceTypeUsedControlIndices[deviceType]);
			}
			
			// TODO remove device states that are no longer used by any bindings?
		}
	}
}
