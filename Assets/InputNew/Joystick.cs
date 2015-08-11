using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	// Must be different from Gamepad as the standardized controls for Gamepads don't
	// work for joysticks.
	public class Joystick
		: InputDevice
	{
		#region Constructors

		public Joystick( string deviceName, List< InputControlData > controls )
			: base( deviceName, controls )
		{
		}

		#endregion
	}
}