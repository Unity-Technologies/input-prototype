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
		enum GamepadControl
		{
			// Standardized.

			LeftStickX,
			LeftStickY,
			LeftStickButton,

			RightStickX,
			RightStickY,
			RightStickButton,

			DPadLeft,
			DPadRight,
			DPadUp,
			DPadDown,

			Action1,
			Action2,
			Action3,
			Action4,

			LeftTrigger,
			RightTrigger,

			LeftBumper,
			RightBumper,

			// Compound controls.

			LeftStick,
			RightStick,
			DPad,

			// Not standardized, but provided for convenience.

			Back,
			Start,
			Select,
			System,
			Pause,
			Menu,
			Share,
			View,
			Options,
			TiltX,
			TiltY,
			TiltZ,
			ScrollWheel,
			TouchPadTap,
			TouchPadXAxis,
			TouchPadYAxis,

			// Not standardized.

			Analog0,
			Analog1,
			Analog2,
			Analog3,
			Analog4,
			Analog5,
			Analog6,
			Analog7,
			Analog8,
			Analog9,
			Analog10,
			Analog11,
			Analog12,
			Analog13,
			Analog14,
			Analog15,
			Analog16,
			Analog17,
			Analog18,
			Analog19,

			Button0,
			Button1,
			Button2,
			Button3,
			Button4,
			Button5,
			Button6,
			Button7,
			Button8,
			Button9,
			Button10,
			Button11,
			Button12,
			Button13,
			Button14,
			Button15,
			Button16,
			Button17,
			Button18,
			Button19,
		}

		#region Constructors

		public Gamepad()
			: this("Gamepad", null) {}

		public Gamepad(string deviceName, List<InputControlData> additionalControls)
		{
			this.deviceName = deviceName;
			var controlCount = EnumHelpers.GetValueCount<GamepadControl>();
			var controls = Enumerable.Repeat(new InputControlData(), controlCount).ToList();

			// Compounds.
			controls[(int)GamepadControl.LeftStick] = new InputControlData
			{
				name = "Left Stick"
				, controlType = InputControlType.Vector2
				, componentControlIndices = new[] { (int)GamepadControl.LeftStickX, (int)GamepadControl.LeftStickY }
			};
			controls[(int)GamepadControl.RightStick] = new InputControlData
			{
				name = "Right Stick"
				, controlType = InputControlType.Vector2
				, componentControlIndices = new[] { (int)GamepadControl.RightStickX, (int)GamepadControl.RightStickY }
			};
			////TODO: dpad (more complicated as the source is buttons which need to be translated into a vector)

			// Buttons.
			controls[(int)GamepadControl.Action1] = new InputControlData { name = "Action 1", controlType = InputControlType.Button };
			controls[(int)GamepadControl.Action2] = new InputControlData { name = "Action 2", controlType = InputControlType.Button };
			controls[(int)GamepadControl.Action3] = new InputControlData { name = "Action 3", controlType = InputControlType.Button };
			controls[(int)GamepadControl.Action4] = new InputControlData { name = "Action 4", controlType = InputControlType.Button };
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

			if (additionalControls != null)
				controls.AddRange(additionalControls);

			SetControls(controls);
		}

		#endregion

		public InputControl leftStickX { get { return this[(int)GamepadControl.LeftStickX]; } }
		public InputControl leftStickY { get { return this[(int)GamepadControl.LeftStickY]; } }
		public InputControl leftStickButton { get { return this[(int)GamepadControl.LeftStickButton]; } }

		public InputControl rightStickX { get { return this[(int)GamepadControl.RightStickX]; } }
		public InputControl rightStickY { get { return this[(int)GamepadControl.RightStickY]; } }
		public InputControl rightStickButton { get { return this[(int)GamepadControl.RightStickButton]; } }

		public InputControl dPadLeft { get { return this[(int)GamepadControl.DPadLeft]; } }
		public InputControl dPadRight { get { return this[(int)GamepadControl.DPadRight]; } }
		public InputControl dPadUp { get { return this[(int)GamepadControl.DPadUp]; } }
		public InputControl dPadDown { get { return this[(int)GamepadControl.DPadDown]; } }

		public InputControl action1 { get { return this[(int)GamepadControl.Action1]; } }
		public InputControl action2 { get { return this[(int)GamepadControl.Action2]; } }
		public InputControl action3 { get { return this[(int)GamepadControl.Action3]; } }
		public InputControl action4 { get { return this[(int)GamepadControl.Action4]; } }

		public InputControl leftTrigger { get { return this[(int)GamepadControl.LeftTrigger]; } }
		public InputControl rightTrigger { get { return this[(int)GamepadControl.RightTrigger]; } }

		public InputControl leftBumper { get { return this[(int)GamepadControl.LeftBumper]; } }
		public InputControl rightBumper { get { return this[(int)GamepadControl.RightBumper]; } }

		// Compound controls.

		public InputControl leftStick { get { return this[(int)GamepadControl.LeftStick]; } }
		public InputControl rightStick { get { return this[(int)GamepadControl.RightStick]; } }
		public InputControl dPad { get { return this[(int)GamepadControl.DPad]; } }

		// Not standardized, but provided for convenience.

		public InputControl back { get { return this[(int)GamepadControl.Back]; } }
		public InputControl start { get { return this[(int)GamepadControl.Start]; } }
		public InputControl select { get { return this[(int)GamepadControl.Select]; } }
		public InputControl system { get { return this[(int)GamepadControl.System]; } }
		public InputControl pause { get { return this[(int)GamepadControl.Pause]; } }
		public InputControl menu { get { return this[(int)GamepadControl.Menu]; } }
		public InputControl share { get { return this[(int)GamepadControl.Share]; } }
		public InputControl view { get { return this[(int)GamepadControl.View]; } }
		public InputControl options { get { return this[(int)GamepadControl.Options]; } }
		public InputControl tiltX { get { return this[(int)GamepadControl.TiltX]; } }
		public InputControl tiltY { get { return this[(int)GamepadControl.TiltY]; } }
		public InputControl tiltZ { get { return this[(int)GamepadControl.TiltZ]; } }
		public InputControl scrollWheel { get { return this[(int)GamepadControl.ScrollWheel]; } }
		public InputControl touchPadTap { get { return this[(int)GamepadControl.TouchPadTap]; } }
		public InputControl touchPadXAxis { get { return this[(int)GamepadControl.TouchPadXAxis]; } }
		public InputControl touchPadYAxis { get { return this[(int)GamepadControl.TouchPadYAxis]; } }
	}
}
