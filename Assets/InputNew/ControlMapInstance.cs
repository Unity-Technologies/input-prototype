using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class ControlMapInstance
		: InputControlProvider
	{
		#region Constructors

		public ControlMapInstance
			(
			ControlMap controlMap
			, int controlSchemeIndex
			, List<InputControlData> controls
			, List<InputState> deviceStates
			)
			: base(controls)
		{
			this.controlSchemeIndex = controlSchemeIndex;
			m_DeviceStates = deviceStates;
			m_ControlMap = controlMap;
		}

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

		public void Activate()
		{
			var treeNode = new InputEventTree
			{
				name = "Map"
				, processInput = ProcessEvent
				, beginNewFrame = BeginNewFrameEvent
			};
			InputSystem.eventTree.children.Add(treeNode);
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
				if (entry.bindings == null || entry.bindings.Count == 0)
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

		#endregion

		void BeginNewFrameEvent ()
		{
			state.BeginNewFrame ();
		}

		#region Public Properties

		public int controlSchemeIndex { get; private set; }

		public InputControl this[ControlMapEntry entry]
		{
			get { return state[entry.controlIndex]; }
		}

		#endregion

		#region Fields

		readonly ControlMap m_ControlMap;
		readonly List<InputState> m_DeviceStates;

		#endregion
	}
}
