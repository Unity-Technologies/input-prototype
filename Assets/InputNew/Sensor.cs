using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public abstract class Sensor
		: InputDevice
	{
		#region Constructors

		protected Sensor( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion
	}
}