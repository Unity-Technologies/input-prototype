using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class PlayerInput
		: InputControlProvider
	{
		#region Constructors

		public PlayerInput(ActionMap controlMap, int controlSchemeIndex, List<InputState> deviceStates)
		{
			Setup(controlMap, controlSchemeIndex, deviceStates);
		}

		protected PlayerInput() {}

		#endregion

		#region Non-Public Methods

		InputState GetDeviceStateForDeviceType(Type deviceType)
		{
			foreach (var deviceState in m_DeviceStates)
			{
				if (deviceType.IsInstanceOfType(deviceState.controlProvider))
					return deviceState;
			}
			throw new ArgumentException("deviceType");
		}

		#endregion

		#region Public Methods

		protected void Setup(ActionMap controlMap, int controlSchemeIndex, List<InputState> deviceStates)
		{
			this.controlSchemeIndex = controlSchemeIndex;
			m_DeviceStates = deviceStates;
			m_ControlMap = controlMap;
			
			// Create list of controls from InputMap.
			var controls = new List<InputControlData>();
			foreach (var entry in controlMap.entries)
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
		
		public void Activate()
		{
			if (m_TreeNode != null)
				return;
			m_TreeNode = new InputEventTree
			{
				name = "Map"
				, processInput = ProcessEvent
				, beginNewFrame = BeginNewFrameEvent
			};
			InputSystem.consumerStack.children.Add(m_TreeNode);
		}
		
		public void Deactivate()
		{
			if (m_TreeNode == null)
				return;
			InputSystem.consumerStack.children.Remove(m_TreeNode);
			m_TreeNode = null;
		}

		public override bool ProcessEvent(InputEvent inputEvent)
		{
			var consumed = false;

			// Update device state (if event actually goes to one of the devices we talk to).
			foreach (var deviceState in m_DeviceStates)
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
			// Synchronize the ControlMapInstance's own state.
			for (var entryIndex = 0; entryIndex < m_ControlMap.entries.Count; ++ entryIndex)
			{
				var entry = m_ControlMap.entries[entryIndex];
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
			var entry = m_ControlMap.entries[controlIndex];
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
		
		public List<InputDevice> GetUsedDevices()
		{
			List<InputDevice> list = new List<InputDevice>();
			for (int i = 0; i < m_DeviceStates.Count; i++)
				list.Add(m_DeviceStates[i].controlProvider as InputDevice);
			return list;
		}
		
		public List<InputState> GetDeviceStates()
		{
			return m_DeviceStates;
		}

		#endregion

		#region Non-Public Methods

		void BeginNewFrameEvent()
		{
			state.BeginNewFrame();
			foreach (var deviceState in m_DeviceStates)
				deviceState.BeginNewFrame();
		}

		#endregion

		#region Public Properties

		public int controlSchemeIndex { get; private set; }

		public InputControl this[InputAction entry]
		{
			get { return state[entry.controlIndex]; }
		}

		#endregion

		#region Fields

		protected ActionMap m_ControlMap;
		protected List<InputState> m_DeviceStates;
		private InputEventTree m_TreeNode = null;

		#endregion
	}
}
