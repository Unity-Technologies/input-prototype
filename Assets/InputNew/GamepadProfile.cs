using UnityEngine;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
	public class GamepadProfile
		: JoystickProfile
	{
		#region Public Methods

		public void SetMapping(int sourceControlIndex, GamepadControl targetControlIndex, string displayName)
		{
			SetMapping(sourceControlIndex, (int)targetControlIndex, displayName, Range.full, Range.full);
		}

		public void SetMapping(int sourceControlIndex, GamepadControl targetControlIndex, string displayName, Range sourceRange, Range targetRange)
		{
			SetMapping(sourceControlIndex, (int)targetControlIndex, displayName, sourceRange, targetRange);
		}

		#endregion
	}
}
