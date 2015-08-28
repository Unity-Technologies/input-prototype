using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	[Serializable]
	public class ControlBinding
	{
		public List<InputControlDescriptor> sources = new List<InputControlDescriptor>();
		public float deadZone = 0.3f;
		public List<ButtonAxisSource> buttonAxisSources = new List<ButtonAxisSource>();
		public float gravity = 1000;
		public float sensitivity = 1000;
		public bool snap = true;
		public bool primaryIsButtonAxis = false;
	}
}
