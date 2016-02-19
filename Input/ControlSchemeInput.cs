using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class ControlSchemeInput : InputControlProvider
	{
		private ControlScheme m_ControlScheme;
		private List<InputState> m_DeviceStates;

		public ActionMap actionMap { get { return m_ControlScheme.actionMap; } }
		public ControlScheme controlScheme { get { return m_ControlScheme; } }
		protected List<InputState> deviceStates { get { return m_DeviceStates; } }

		public ControlSchemeInput(ControlScheme controlScheme, List<InputDevice> devices)
		{
			// Create state for every device.
			var deviceStates = new List<InputState>();
			foreach (var device in devices)
			{
				deviceStates.Add(new InputState(device));
			}

			Setup(controlScheme, deviceStates);
		}

		public ControlSchemeInput(ControlScheme controlScheme, List<InputState> deviceStates)
		{
			Setup(controlScheme, deviceStates);
		}

		protected void Setup(ControlScheme controlScheme, List<InputState> deviceStates)
		{
			m_ControlScheme = controlScheme;
			m_DeviceStates = deviceStates;
			
			// Create list of controls from ActionMap.
			////REVIEW: doesn't handle compounds
			var controls = new List<InputControlData>();
			foreach (var entry in actionMap.actions)
				controls.Add(entry.controlData);
			SetControls(controls);
		}
		
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
			
			RefreshBindings();
			
			return true;
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
		
		public List<InputDevice> GetUsedDevices()
		{
			List<InputDevice> list = new List<InputDevice>();
			for (int i = 0; i < deviceStates.Count; i++)
				list.Add(deviceStates[i].controlProvider as InputDevice);
			return list;
		}
		
		public List<InputState> GetDeviceStates()
		{
			return deviceStates;
		}
		
		public override ButtonInputControl anyButton
		{
			get
			{
				foreach (var state in deviceStates)
					if (state.controlProvider.anyButton.isHeld)
						return state.controlProvider.anyButton;
				return this[0] as ButtonInputControl;
			}
		}
		
		InputState GetDeviceStateForDeviceType(Type deviceType)
		{
			foreach (var deviceState in deviceStates)
			{
				if (deviceType.IsInstanceOfType(deviceState.controlProvider))
					return deviceState;
			}
			throw new ArgumentException("deviceType");
		}
		
		public override bool ProcessEvent(InputEvent inputEvent)
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
		
		public void Reset()
		{
			state.Reset();
			foreach (var deviceState in GetDeviceStates())
				deviceState.Reset();
		}

		public void BeginFrame()
		{
			state.BeginFrame();
			foreach (var deviceState in GetDeviceStates())
				deviceState.BeginFrame();
		}
		
		public void EndFrame()
		{
			foreach (var deviceState in GetDeviceStates())
				deviceState.EndFrame();

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

			state.EndFrame();
		}

		float GetSourceValue(InputControlDescriptor source)
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
	}
}
