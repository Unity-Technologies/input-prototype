using System;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	[ Serializable ]
	public class ControlBinding
	{
		public List< InputControlDescriptor > sources;
		public float deadZone = 0.3f;
		public List< ButtonAxisSource > buttonAxisSources;
		public float gravity = 1000;
		public float sensitivity = 1000;
		public bool snap = true;
	}
}