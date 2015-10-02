using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public abstract class PlayerInput : InputControlProvider
	{
		public abstract ActionMap actionMap { get; }
		public abstract int controlSchemeIndex { get; }
		protected abstract List<InputState> deviceStates { get; }

		private InputEventTree treeNode { get; set; }

		InputState GetDeviceStateForDeviceType(Type deviceType)
		{
			foreach (var deviceState in deviceStates)
			{
				if (deviceType.IsInstanceOfType(deviceState.controlProvider))
					return deviceState;
			}
			throw new ArgumentException("deviceType");
		}

		public void Activate()
		{
			if (treeNode != null)
				return;
			treeNode = new InputEventTree
			{
				name = "Map"
				, processInput = ProcessEvent
				, beginNewFrame = BeginNewFrameEvent
			};
			InputSystem.consumerStack.children.Add(treeNode);
		}
		
		public void Deactivate()
		{
			if (treeNode == null)
				return;
			InputSystem.consumerStack.children.Remove(treeNode);
			treeNode = null;
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
			for (var entryIndex = 0; entryIndex < actionMap.entries.Count; ++ entryIndex)
			{
				var entry = actionMap.entries[entryIndex];
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
			var entry = actionMap.entries[controlIndex];
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
			for (int i = 0; i < deviceStates.Count; i++)
				list.Add(deviceStates[i].controlProvider as InputDevice);
			return list;
		}
		
		public List<InputState> GetDeviceStates()
		{
			return deviceStates;
		}

		void BeginNewFrameEvent()
		{
			state.BeginNewFrame();
			foreach (var deviceState in deviceStates)
				deviceState.BeginNewFrame();
		}

		public InputControl this[InputAction entry]
		{
			get { return state[entry.controlIndex]; }
		}
	}
}
