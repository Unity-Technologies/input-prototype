using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Utilities
{
	[Serializable]
	public struct Range
	{
		public float max;
		public float min;

		public Range(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
