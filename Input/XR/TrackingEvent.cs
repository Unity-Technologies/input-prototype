using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class TrackingEvent
		: InputEvent
	{
		public Vector3 localPosition { get; set; }
		public Quaternion localRotation { get; set; }
	}
}
