using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class InputControl
	{
		protected readonly int m_Index;
		protected readonly InputState m_State;

		internal InputControl(int index, InputState state)
		{
			m_Index = index;
			m_State = state;
		}

		public int index
		{
			get { return m_Index; }
		}
		
		public InputControlProvider provider
		{
			get { return m_State.controlProvider; }
		}

		public bool isEnabled
		{
			get { return m_State.IsControlEnabled(m_Index); }
		}

		public InputControlData data
		{
			get { return provider.GetControlData(index); }
		}

		public string name
		{
			get { return data.name; }
		}
		
		public InputControlType controlType
		{
			get { return data.controlType; }
		}

		public string GetPrimarySourceName(string buttonAxisFormattingString = "{0} & {1}")
		{
			return m_State.controlProvider.GetPrimarySourceName(index, buttonAxisFormattingString);
		}
	}

	public class AxisInputControl : InputControl
	{
		public readonly ButtonInputControl negative;
		public readonly ButtonInputControl positive;

		public AxisInputControl(int index, InputState state) : base(index, state)
		{
			negative = new ButtonInputControl(index, state);
			negative.SetValueMultiplier(-1);
			positive = new ButtonInputControl(index, state);
		}

		public float value
		{
			get { return m_State.GetCurrentValue(m_Index); }
		}
	}

	public class ButtonInputControl : InputControl
	{
		private const float k_ButtonThreshold = 0.5f;
		private float m_ValueMultiplier = 1;

		public ButtonInputControl(int index, InputState state) : base(index, state) {}

		public bool isHeld
		{
			get { return m_State.GetCurrentValue(m_Index) * m_ValueMultiplier > k_ButtonThreshold; }
		}

		public bool wasJustPressed
		{
			get { return isHeld && (m_State.GetPreviousValue(m_Index) * m_ValueMultiplier <= k_ButtonThreshold); }
		}

		public bool wasJustReleased
		{
			get { return !isHeld && (m_State.GetPreviousValue(m_Index) * m_ValueMultiplier > k_ButtonThreshold); }
		}

		public void SetValueMultiplier(float multiplier)
		{
			m_ValueMultiplier = multiplier;
		}
	}

	public class Vector2InputControl : InputControl
	{
		public Vector2InputControl(int index, InputState state) : base(index, state) {}

		public Vector2 vector2
		{
			get
			{
				var controlData = m_State.controlProvider.GetControlData(m_Index);
				return new Vector2(
					m_State.GetCurrentValue(controlData.componentControlIndices[0]),
					m_State.GetCurrentValue(controlData.componentControlIndices[1])
				);
			}
		}
	}

	public class Vector3InputControl : InputControl
	{
		public Vector3InputControl(int index, InputState state) : base(index, state) {}

		public Vector3 vector3
		{
			get
			{
				var controlData = m_State.controlProvider.GetControlData(m_Index);
				return new Vector3(
					m_State.GetCurrentValue(controlData.componentControlIndices[0]),
					m_State.GetCurrentValue(controlData.componentControlIndices[1]),
					m_State.GetCurrentValue(controlData.componentControlIndices[2])
				);
			}
		}
	}
}
