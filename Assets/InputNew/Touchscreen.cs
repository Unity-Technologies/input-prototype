using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class Touchscreen
		: Pointer
	{
		#region Constructors

		public Touchscreen( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public static Touchscreen CreateDefault()
		{
			var controls = CreateDefaultControls();
			return new Touchscreen( controls );
		}

		#endregion
	}
}