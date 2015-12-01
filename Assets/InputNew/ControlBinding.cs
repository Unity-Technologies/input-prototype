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
		
		public void ExtractDeviceTypesAndControlIndices(Dictionary<Type, List<int>> controlIndicesPerDeviceType)
		{
			foreach (var control in sources)
			{
				control.ExtractDeviceTypeAndControlIndex(controlIndicesPerDeviceType);
			}
			
			foreach (var axis in buttonAxisSources)
			{
				axis.negative.ExtractDeviceTypeAndControlIndex(controlIndicesPerDeviceType);
				axis.positive.ExtractDeviceTypeAndControlIndex(controlIndicesPerDeviceType);
			}
		}
	}
}
