using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public struct InputControl
	{
		#region Constructors

		internal InputControl(int index, InputState state)
		{
			m_Index = index;
			m_State = state;
		}

		#endregion

		#region Public Properties

		public int index
		{
			get { return m_Index; }
		}

		public bool boolValue
		{
			get
			{
				var currentValue = m_State.GetCurrentValue(m_Index);
				if (currentValue > 0.001f)
					return true;
				return false;
			}
		}

		public float floatValue
		{
			get { return m_State.GetCurrentValue(m_Index); }
		}

		public Vector3 vector3Value
		{
			get
			{
				var controlData = m_State.controlProvider.controls[m_Index];
				////TODO: typecheck control type; convert if necessary
				return new Vector3(
					m_State.GetCurrentValue(controlData.componentControlIndices[0])
					, m_State.GetCurrentValue(controlData.componentControlIndices[1])
					, m_State.GetCurrentValue(controlData.componentControlIndices[2])
					);
			}
		}

		public bool isEnabled
		{
			get { return m_State.IsControlEnabled(m_Index); }
		}

		#endregion

		#region Fields

		readonly int m_Index;
		readonly InputState m_State;

		#endregion
	}
}
