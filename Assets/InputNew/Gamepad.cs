using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
	public class Gamepad
		: Joystick
	{
		#region Constructors

		public Gamepad( string deviceName, List< InputControlData > controls )
			: base( deviceName, controls )
		{
		}

		#endregion

		public static InputDevice CreateDefault()
		{
			var controls = Enumerable.Repeat( new InputControlData(), EnumHelpers.GetValueCount< GamepadControl >() ).ToList();

			// Compounds.
			controls[ ( int ) GamepadControl.LeftThumbstick ] = new InputControlData
				{
					  name = "LeftThumbstick"
					, controlType = InputControlType.Vector2
					, componentControlIndices = new int[ 2 ] { ( int ) GamepadControl.LeftThumbstickX, ( int ) GamepadControl.LeftThumbstickY }
				};
			controls[ ( int ) GamepadControl.RightThumbstick ] = new InputControlData
				{
					  name = "RightThumbstick"
					, controlType = InputControlType.Vector2
					, componentControlIndices = new int[ 2 ] { ( int ) GamepadControl.RightThumbstickX, ( int ) GamepadControl.RightThumbstickY }
				};
			////TODO: dpad (more complicated as the source is buttons which need to be translated into a vector)

			// Buttons.
			controls[ ( int ) GamepadControl.ButtonA ] = new InputControlData { name = "A", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.ButtonB ] = new InputControlData { name = "B", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.ButtonX ] = new InputControlData { name = "X", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.ButtonY ] = new InputControlData { name = "Y", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.Start ] = new InputControlData { name = "Start", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.Back ] = new InputControlData { name = "Back", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.DpadPress ] = new InputControlData { name = "DpadPress", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.LeftThumbstickPress ] = new InputControlData { name = "LeftThumbstickPress", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.RightThumbstickPress ] = new InputControlData { name = "RightThumbstickPress", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.DpadUp ] = new InputControlData { name = "DpadUp", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.DpadDown ] = new InputControlData { name = "DpadDown", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.DpadLeft ] = new InputControlData { name = "DpadLeft", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.DpadRight ] = new InputControlData { name = "DpadRight", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.LeftShoulder ] = new InputControlData { name = "LeftShoulder", controlType = InputControlType.Button };
			controls[ ( int ) GamepadControl.RightShoulder ] = new InputControlData { name = "RightShoulder", controlType = InputControlType.Button };

			// Axes.
			controls[ ( int ) GamepadControl.LeftThumbstickX ] = new InputControlData { name = "LeftThumbstickX", controlType = InputControlType.AbsoluteAxis };
			controls[ ( int ) GamepadControl.LeftThumbstickY ] = new InputControlData { name = "LeftThumbstickY", controlType = InputControlType.AbsoluteAxis };
			controls[ ( int ) GamepadControl.RightThumbstickX ] = new InputControlData { name = "RightThumbstickX", controlType = InputControlType.AbsoluteAxis };
			controls[ ( int ) GamepadControl.RightThumbstickY ] = new InputControlData { name = "RightThumbstickY", controlType = InputControlType.AbsoluteAxis };
			controls[ ( int ) GamepadControl.LeftTrigger ] = new InputControlData { name = "LeftTrigger", controlType = InputControlType.AbsoluteAxis };
			controls[ ( int ) GamepadControl.RightTrigger ] = new InputControlData { name = "RightTrigger", controlType = InputControlType.AbsoluteAxis };

			return new Gamepad( "Generic Gamepad", controls );
		}
	}
}