using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.InputNew
{
	[Serializable]
	public class InputAction
	{
		public string name { get { return m_ControlData.name; } set { m_ControlData.name = value; } }

		[SerializeField]
		private InputControlData m_ControlData;
		public InputControlData controlData { get { return m_ControlData; } set { m_ControlData = value; } }

		// This is one entry for each control scheme (matching indices) -- except if there are no bindings for the entry.
		[FormerlySerializedAs("bindings")]
		[SerializeField]
		private List<ControlBinding> m_Bindings = new List<ControlBinding>();
		public List<ControlBinding> bindings { get { return m_Bindings; } set { m_Bindings = value; } }

		public int controlIndex { get; set; }

		public override string ToString()
		{
			return string.Format("({0}, bindings:{1})", controlData.name, bindings.Count);
		}
	}
}
