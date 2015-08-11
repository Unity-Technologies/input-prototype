using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class Mouse
		: Pointer
	{
		#region Constructors

		public Mouse( string deviceName, List< InputControlData > controls )
			: base( deviceName, controls )
		{
		}

		#endregion

		#region Public Methods

		public static Mouse CreateDefault()
		{
			var controls = CreateDefaultControls();
			return new Mouse( "Generic Mouse", controls );
		}

		#endregion
	}
}