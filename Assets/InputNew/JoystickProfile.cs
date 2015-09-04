using UnityEngine;
using Assets.Utilities;

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
				{
					controlEvent.controlIndex = mapping.targetIndex;
					controlEvent.value = Mathf.InverseLerp(mapping.fromRange.min, mapping.fromRange.max, controlEvent.value);
					controlEvent.value = Mathf.Lerp(mapping.toRange.min, mapping.toRange.max, controlEvent.value);
				}
			}
		}

		public void SetMappingsCount(int sourceControlCount, int tarcontrolCount)
		{
			mappings = new JoystickControlMapping[sourceControlCount];
			nameOverrides = new string[tarcontrolCount];
		}

		public void SetMapping(int sourceControlIndex, int targetControlIndex, string displayName, Range sourceRange, Range targetRange)
		{
			mappings[sourceControlIndex] = new JoystickControlMapping
			{
				targetIndex = targetControlIndex,
				fromRange = sourceRange,
				toRange = targetRange
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
