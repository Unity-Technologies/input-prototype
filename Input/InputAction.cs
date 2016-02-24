using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.InputNew
{
	public class InputAction : ScriptableObject
	{
		public new string name
		{
			get
			{
				return m_ControlData.name;
			}
			set
			{
				m_ControlData.name = value;
				base.name = value;
			}
		}

		[SerializeField]
		private InputControlData m_ControlData;
		public InputControlData controlData { get { return m_ControlData; } set { m_ControlData = value; } }
	}
}
