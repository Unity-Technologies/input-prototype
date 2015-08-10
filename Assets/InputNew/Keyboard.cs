using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class Keyboard
		: InputDevice
	{
		#region Constructors

		public Keyboard( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public static Keyboard CreateDefault()
		{
			var controls = new List< InputControlData >();
			return new Keyboard( controls );
		}

		#endregion
	}
}