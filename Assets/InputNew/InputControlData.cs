using System;

namespace UnityEngine.InputNew
{
	[Serializable]
	public struct InputControlData
	{
		public string name;
		public InputControlType controlType;
		public int[] componentControlIndices;
	}
}
