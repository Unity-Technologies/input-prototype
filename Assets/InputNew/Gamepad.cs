using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class Gamepad
		: InputDevice
	{
		#region Constructors

		public Gamepad( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion
	}
}