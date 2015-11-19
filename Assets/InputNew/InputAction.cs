using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.InputNew
{
	// Currently this class is little more than a wrapper for InputControlData,
	// but since we may need to store a GUID for InputActions (but not other InputControlData possibly)
	// I'm leaving it for now.
	[Serializable]
	public class InputAction
	{
		public string name { get { return m_ControlData.name; } set { m_ControlData.name = value; } }

		[SerializeField]
		private InputControlData m_ControlData;
		public InputControlData controlData { get { return m_ControlData; } set { m_ControlData = value; } }
	}
}
