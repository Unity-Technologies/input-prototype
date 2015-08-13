using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class ControlMapEntry
		: ScriptableObject
	{
		#region Fields

		[SerializeField]
		InputControlData m_ControlData;

		#endregion

		#region Public Properties

		public InputControlData controlData
		{
			get { return m_ControlData; }
			set
			{
				m_ControlData = value;
				name = m_ControlData.name;
			}
		}

		// This is one entry for each control scheme (matching indices).
		public List<ControlBinding> bindings;

		[NonSerialized]
		public int controlIndex;

		public override string ToString()
		{
			return string.Format("({0}, bindings:{1})", controlData.name, bindings.Count);
		}

		#endregion
	}
}
