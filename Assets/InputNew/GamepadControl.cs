namespace UnityEngine.InputNew
{
	public enum GamepadControl
	{
		LeftThumbstick, // Compound control (vector2)
		LeftThumbstickX,
		LeftThumbstickY,

		RightThumbstick, // Compound control (vector2)
		RightThumbstickX,
		RightThumbstickY,

		Dpad, // Compound control (vector2)
		DpadUp,
		DpadDown,
		DpadLeft,
		DpadRight,

		LeftTrigger,
		RightTrigger,

		LeftShoulder,
		RightShoulder,

		ButtonA,
		ButtonB,
		ButtonX,
		ButtonY,

		Start,
		Back,

		// -- Optional:

		DpadPress,
		LeftThumbstickPress,
		RightThumbstickPress,
	}
}