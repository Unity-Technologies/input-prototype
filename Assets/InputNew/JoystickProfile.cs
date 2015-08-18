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
		public string[] nameOverrides;

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

		public void SetMappingsCount(int sourceControlCount, int targetControlCount)
		{
			mappings = new JoystickControlMapping[sourceControlCount];
			nameOverrides = new string[targetControlCount];
		}

		public void SetMapping(int sourceControlIndex, int targetControlIndex, string displayName = null)
		{
			mappings[sourceControlIndex] = new JoystickControlMapping
			{
				targetIndex = targetControlIndex
			};
			nameOverrides[targetControlIndex] = displayName;
		}
		
		public override string GetControlNameOverride(int controlIndex)
		{
			if (controlIndex >= nameOverrides.Length)
				return null;
			return nameOverrides[controlIndex];
		}

		#endregion
	}
}
