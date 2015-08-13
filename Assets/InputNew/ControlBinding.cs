using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	[Serializable]
	public class ControlBinding
	{
		public List<ButtonAxisSource> buttonAxisSources;
		public float deadZone = 0.3f;
		public float gravity = 1000;
		public float sensitivity = 1000;
		public bool snap = true;
		public List<InputControlDescriptor> sources;
	}
}
