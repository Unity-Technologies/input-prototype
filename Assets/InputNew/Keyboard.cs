using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class Keyboard
		: InputDevice
	{
		#region Constructors

		public Keyboard( string deviceName, List< InputControlData > controls )
			: base( deviceName, controls )
		{
		}

		#endregion

		#region Public Methods

		public static Keyboard CreateDefault()
		{
			var controls = new List< InputControlData >();
			return new Keyboard( "Generic Keyboard", controls );
		}

		#endregion
	}
}