using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class Gamepad
		: Joystick
	{
		#region Constructors

		public Gamepad(string deviceName, List<InputControlData> controls)
			: base(deviceName, controls) { }

		#endregion
		
		public static InputDevice CreateDefault()
		{
			var controls = Enumerable.Repeat(new InputControlData(), EnumHelpers.GetValueCount<GamepadControl>()).ToList();

			// Compounds.
			controls[(int)GamepadControl.LeftStick] = new InputControlData
			{
				name = "LeftThumbstick"
				, controlType = InputControlType.Vector2
				, componentControlIndices = new int[2] { (int)GamepadControl.LeftStickX, (int)GamepadControl.LeftStickY }
			};
			controls[(int)GamepadControl.RightStick] = new InputControlData
			{
				name = "RightThumbstick"
				, controlType = InputControlType.Vector2
				, componentControlIndices = new int[2] { (int)GamepadControl.RightStickX, (int)GamepadControl.RightStickY }
			};
			////TODO: dpad (more complicated as the source is buttons which need to be translated into a vector)

			// Buttons.
			controls[(int)GamepadControl.ButtonA] = new InputControlData { name = "A", controlType = InputControlType.Button };
			controls[(int)GamepadControl.ButtonB] = new InputControlData { name = "B", controlType = InputControlType.Button };
			controls[(int)GamepadControl.ButtonX] = new InputControlData { name = "X", controlType = InputControlType.Button };
			controls[(int)GamepadControl.ButtonY] = new InputControlData { name = "Y", controlType = InputControlType.Button };
			controls[(int)GamepadControl.Start] = new InputControlData { name = "Start", controlType = InputControlType.Button };
			controls[(int)GamepadControl.Back] = new InputControlData { name = "Back", controlType = InputControlType.Button };
			controls[(int)GamepadControl.LeftStickButton] = new InputControlData { name = "Left Stick Button", controlType = InputControlType.Button };
			controls[(int)GamepadControl.RightStickButton] = new InputControlData { name = "Right Stick Button", controlType = InputControlType.Button };
			controls[(int)GamepadControl.DPadUp] = new InputControlData { name = "DPad Up", controlType = InputControlType.Button };
			controls[(int)GamepadControl.DPadDown] = new InputControlData { name = "DPad Down", controlType = InputControlType.Button };
			controls[(int)GamepadControl.DPadLeft] = new InputControlData { name = "DPad Left", controlType = InputControlType.Button };
			controls[(int)GamepadControl.DPadRight] = new InputControlData { name = "DPad Right", controlType = InputControlType.Button };
			controls[(int)GamepadControl.LeftBumper] = new InputControlData { name = "Left Bumper", controlType = InputControlType.Button };
			controls[(int)GamepadControl.RightBumper] = new InputControlData { name = "Right Bumper", controlType = InputControlType.Button };

			// Axes.
			controls[(int)GamepadControl.LeftStickX] = new InputControlData { name = "Left Stick X", controlType = InputControlType.AbsoluteAxis };
			controls[(int)GamepadControl.LeftStickY] = new InputControlData { name = "Left Stick Y", controlType = InputControlType.AbsoluteAxis };
			controls[(int)GamepadControl.RightStickX] = new InputControlData { name = "Right Stick X", controlType = InputControlType.AbsoluteAxis };
			controls[(int)GamepadControl.RightStickY] = new InputControlData { name = "Right Stick Y", controlType = InputControlType.AbsoluteAxis };
			controls[(int)GamepadControl.LeftTrigger] = new InputControlData { name = "Left Trigger", controlType = InputControlType.AbsoluteAxis };
			controls[(int)GamepadControl.RightTrigger] = new InputControlData { name = "Right Trigger", controlType = InputControlType.AbsoluteAxis };

			return new Gamepad("Generic Gamepad", controls);
		}
	}
}
