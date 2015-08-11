using System;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
	[ Serializable ]
	public struct JoystickControlMapping
	{
		public int targetIndex;
		public Sprite displayIcon; 
		public string displayName;
		public Range fromRange;
		public Range toRange;
	}
}