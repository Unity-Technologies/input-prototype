using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class Touchscreen
		: Pointer
	{
		#region Constructors

		public Touchscreen(string deviceName, List<InputControlData> controls)
			: base(deviceName, controls) { }

		#endregion

		#region Public Methods

		public static Touchscreen CreateDefault()
		{
			var controls = CreateDefaultControls();
			return new Touchscreen("Generic Touchscreen", controls);
		}

		#endregion
	}
}
