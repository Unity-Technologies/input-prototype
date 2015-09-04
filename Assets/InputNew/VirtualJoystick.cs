using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class VirtualJoystick
		: Joystick
	{
		#region Constructors

		public VirtualJoystick()
			: this("Virtual Joystick", null) { }

		public VirtualJoystick(string deviceName, List<InputControlData> additionalControls)
		{
			this.deviceName = deviceName;
			var controlCount = EnumHelpers.GetValueCount<GamepadControl>();
			var controls = Enumerable.Repeat(new InputControlData(), controlCount).ToList();
			
			// Compounds.
			controls[(int)VirtualJoystickControl.LeftStick] = new InputControlData
			{
				name = "Left Stick"
				, controlType = InputControlType.Vector2
				, componentControlIndices = new[] { (int)VirtualJoystickControl.LeftStickX, (int)VirtualJoystickControl.LeftStickY }
			};
			controls[(int)VirtualJoystickControl.RightStick] = new InputControlData
			{
				name = "Right Stick"
				, controlType = InputControlType.Vector2
				, componentControlIndices = new[] { (int)VirtualJoystickControl.RightStickX, (int)VirtualJoystickControl.RightStickY }
			};
			
			// Axes.
			controls[(int)VirtualJoystickControl.LeftStickX] = new InputControlData { name = "Left Stick X", controlType = InputControlType.AbsoluteAxis };
			controls[(int)VirtualJoystickControl.LeftStickY] = new InputControlData { name = "Left Stick Y", controlType = InputControlType.AbsoluteAxis };
			controls[(int)VirtualJoystickControl.RightStickX] = new InputControlData { name = "Right Stick X", controlType = InputControlType.AbsoluteAxis };
			controls[(int)VirtualJoystickControl.RightStickY] = new InputControlData { name = "Right Stick Y", controlType = InputControlType.AbsoluteAxis };
			
			// Buttons.
			controls[(int)VirtualJoystickControl.Action1] = new InputControlData { name = "Action 1", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Action2] = new InputControlData { name = "Action 2", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Action3] = new InputControlData { name = "Action 3", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Action4] = new InputControlData { name = "Action 4", controlType = InputControlType.Button };
			
			controls[(int)VirtualJoystickControl.Back] = new InputControlData { name = "Back", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Start] = new InputControlData { name = "Start", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Select] = new InputControlData { name = "Select", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Pause] = new InputControlData { name = "Pause", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Menu] = new InputControlData { name = "Menu", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Share] = new InputControlData { name = "Share", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.View] = new InputControlData { name = "View", controlType = InputControlType.Button };
			controls[(int)VirtualJoystickControl.Options] = new InputControlData { name = "Options", controlType = InputControlType.Button };
			
			if (additionalControls != null)
				controls.AddRange(additionalControls);
			
			SetControls(controls);
		}

		#endregion

		public static VirtualJoystick current { get { return InputSystem.GetMostRecentlyUsedDevice<VirtualJoystick>(); } }
		
		public void SetAxisValue(int controlIndex, float value)
		{
			float currentValue = this[controlIndex].value;
			if (value == currentValue)
				return;
			
			var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
			inputEvent.deviceType = typeof(VirtualJoystick);
			inputEvent.deviceIndex = 0; // TODO: Use index of device itself, but that's not currently stored on device.
			inputEvent.controlIndex = controlIndex;
			inputEvent.value = value;
			InputSystem.QueueEvent(inputEvent);
		}
		
		public void SetButtonValue(int controlIndex, bool state)
		{
			SetAxisValue(controlIndex, state ? 1 : 0);
		}
	}
}
