using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class GamepadProfile
		: JoystickProfile
	{
		#region Public Methods

		public void SetMapping(int sourceControlIndex, GamepadControl targetControlIndex, string displayName = null)
		{
			SetMapping(sourceControlIndex, (int)targetControlIndex, displayName);
		}

		#endregion
	}
}
