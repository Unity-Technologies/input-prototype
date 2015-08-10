using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class Mouse
		: Pointer
	{
		#region Constructors

		public Mouse( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public static Mouse CreateDefault()
		{
			var controls = CreateDefaultControls();
			return new Mouse( controls );
		}

		#endregion
	}
}