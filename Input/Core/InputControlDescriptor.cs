using System;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	[Serializable]
	public class InputControlDescriptor
	{
		public int controlIndex;
		public SerializableDeviceType deviceType;

		public virtual InputControlDescriptor Clone()
		{
			var clone = (InputControlDescriptor) Activator.CreateInstance(GetType());
			clone.controlIndex = controlIndex;
			clone.deviceType = deviceType.Clone();
			return clone;
		}
		
		public override string ToString()
		{
			return string.Format( "(device:{0}, control:{1})", deviceType.type.Name, controlIndex );
		}
		
		public void ExtractDeviceTypeAndControlIndex(Dictionary<Type, List<int>> controlIndicesPerDeviceType)
		{
			List<int> entries;
			if (!controlIndicesPerDeviceType.TryGetValue(deviceType.type, out entries))
			{
				entries = new List<int>();
				controlIndicesPerDeviceType[deviceType.type] = entries;
			}
			
			entries.Add(controlIndex);
		}
	}
}
