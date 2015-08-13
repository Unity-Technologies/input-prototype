using System;
using System.Collections.Generic;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.InputNew
{
	[Serializable]
	public struct JoystickControlMapping
	{
		public Sprite displayIcon;
		public string displayName;
		public Range fromRange;
		public int targetIndex;
		public Range toRange;
	}
}
