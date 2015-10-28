using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class SchemeInput : InputControlProvider
	{
		private List<InputControlData> m_Controls;
		private InputState m_State;
		private ActionMap m_ActionMap;
		private int m_ControlSchemeIndex = 0;
		private List<InputState> m_DeviceStates;

		public override List<InputControlData> controls { get { return m_Controls; } }
		public override InputState state { get { return m_State; } }
		public ActionMap actionMap { get { return m_ActionMap; } }
		public int controlSchemeIndex { get { return m_ControlSchemeIndex; } }
		protected List<InputState> deviceStates { get { return m_DeviceStates; } }

		public SchemeInput(ActionMap actionMap, int controlSchemeIndex, List<InputState> deviceStates)
		{
			Setup(actionMap, controlSchemeIndex, deviceStates);
		}

		private void SetControls(List<InputControlData> controls)
		{
			m_Controls = controls;
			m_State = new InputState(this);
		}

		protected void Setup(ActionMap actionMap, int controlSchemeIndex, List<InputState> deviceStates)
		{
			m_ControlSchemeIndex = controlSchemeIndex;
			m_DeviceStates = deviceStates;
			m_ActionMap = actionMap;
			
			// Create list of controls from InputMap.
			var controls = new List<InputControlData>();
			foreach (var entry in actionMap.actions)
			{
				////REVIEW: why are we making copies here?
				var control = new InputControlData
				{
					name = entry.controlData.name,
					controlType = entry.controlData.controlType,
					////REVIEW: doesn't handle compounds
				};
				controls.Add(control);
			}
			SetControls(controls);
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
		
		public override InputControl anyButton
		{
			get
			{
				foreach (var state in deviceStates)
					if (state.controlProvider.anyButton.button)
						return state.controlProvider.anyButton;
				return this[0];
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
			
			////REVIEW: this probably needs to be done as a post-processing step after all events have been received
			// Synchronize the ActionMapInstance's own state.
			for (var entryIndex = 0; entryIndex < actionMap.actions.Count; ++ entryIndex)
			{
				var entry = actionMap.actions[entryIndex];
				if (entry.bindings == null || entry.bindings.Count <= controlSchemeIndex)
					continue;
				
				var binding = entry.bindings[controlSchemeIndex];
				
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
			
			return true;
		}
		
		float GetSourceValue(InputControlDescriptor source)
		{
			var deviceState = GetDeviceStateForDeviceType(source.deviceType);
			return deviceState[source.controlIndex].value;
		}
		
		public override string GetPrimarySourceName(int controlIndex, string buttonAxisFormattingString = "{0} & {1}")
		{
			var entry = actionMap.actions[controlIndex];
			if (entry.bindings == null || entry.bindings.Count == 0)
				return string.Empty;
			
			var binding = entry.bindings[controlSchemeIndex];
			
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
