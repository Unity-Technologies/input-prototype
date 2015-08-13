using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class JoystickProfile
		: InputDeviceProfile
	{
		#region Public Properties

		public JoystickControlMapping[] mappings;

		#endregion

		#region Public Methods

		public override void Remap(InputEvent inputEvent)
		{
			var controlEvent = inputEvent as GenericControlEvent;
			if (controlEvent != null)
			{
				var mapping = mappings[controlEvent.controlIndex];
				if (mapping.targetIndex != -1)
					controlEvent.controlIndex = mapping.targetIndex;
			}
		}

		public void SetMappingsCount(int sourceControlCount)
		{
			mappings = new JoystickControlMapping[sourceControlCount];
		}

		public void SetMapping(int sourceControlIndex, int targetControlIndex, string displayName = null, Sprite displayIcon = null)
		{
			mappings[sourceControlIndex] = new JoystickControlMapping
			{
				targetIndex = targetControlIndex
				, displayName = displayName
				, displayIcon = displayIcon
			};
		}

		#endregion
	}
}
