using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class InputState
	{
		#region Constructors

		public InputState(InputControlProvider controlProvider)
			: this(controlProvider, null) { }

		public InputState(InputControlProvider controlProvider, List<int> usedControlIndices)
		{
			this.controlProvider = controlProvider;

			var controlCount = controlProvider.controlCount;
			m_CurrentStates = new float[controlCount];
			m_PreviousStates = new float[controlCount];
			m_Enabled = new bool[controlCount];
			
			SetUsedControls(usedControlIndices);
		}
		
		public void SetUsedControls(List<int> usedControlIndices)
		{
			if (usedControlIndices == null)
			{
				SetAllControlsEnabled(true);
			}
			else
			{
				SetAllControlsEnabled(false);
				for (var i = 0; i < usedControlIndices.Count; i++)
					m_Enabled[usedControlIndices[i]] = true;
			}
		}

		#endregion

		#region Public Methods

		public float GetCurrentValue(int index)
		{
			return m_CurrentStates[index];
		}

		public float GetPreviousValue(int index)
		{
			return m_PreviousStates[index];
		}

		public bool SetCurrentValue(int index, bool value)
		{
			return SetCurrentValue(index, value ? 1.0f : 0.0f);
		}

		public bool SetCurrentValue(int index, float value)
		{
			if (index < 0 || index >= m_CurrentStates.Length)
				throw new ArgumentOutOfRangeException("index",
					string.Format("Control index {0} is out of range; state has {1} entries", index, m_CurrentStates.Length));

			if (!IsControlEnabled(index))
				return false;

			m_CurrentStates[index] = value;

			return true;
		}

		public bool IsControlEnabled(int index)
		{
			return m_Enabled[index];
		}

		public void SetControlEnabled(int index, bool enabled)
		{
			m_Enabled[index] = enabled;
		}

		public void SetAllControlsEnabled(bool enabled)
		{
			for (var i = 0; i < m_Enabled.Length; ++ i)
			{
				m_Enabled[i] = enabled;
			}
		}
		
		public void Reset()
		{
			for (int i = 0; i < m_CurrentStates.Length; i++)
			{
				m_CurrentStates[i] = 0;
				m_PreviousStates[i] = 0;
			}
			m_FirstFrameAfterReset = true;
		}

		#endregion

		#region Non-Public Methods

		internal void BeginFrame()
		{
			var stateCount = m_Enabled.Length;
			for (var index = 0; index < stateCount; ++index)
			{
				if (!m_Enabled[index])
					continue;
				if (m_PreviousStates[index] == m_CurrentStates[index])
					continue;
				
				if (InputSystem.listeningForBinding)
				{
					// TODO: Figure out how to use sensible thresholds for different controls.
					if (Mathf.Abs(m_CurrentStates[index]) >= 0.5f && Mathf.Abs(m_PreviousStates[index]) < 0.5f)
						InputSystem.RegisterBinding(controlProvider[index]);
				}
				
				m_PreviousStates[index] = m_CurrentStates[index];
			}
		}

		internal void EndFrame()
		{
			if (!m_FirstFrameAfterReset)
				return;
			
			// The first frame after resetting, we want the previous value to be the same as the new value.
			// Effectively there was no previous value, and we should not detect a change just because
			// the first registered value is different from the default initialized value.
			// This prevents registering a button press for a button that is held down while the state
			// becomes activated.
			// For example, when pressing a menu button, the user might switch to a different action map,
			// and in the new action map we don't want to immediately register the menu button as being
			// pressed down again (potentially closing the menu).
			// The problem doesn't exist with e.g. mouse clicks since they only generate a single event
			// when pressed and when released.
			// But for a button based on an axis, such as a gamepad trigger, we have continuous events.
			m_FirstFrameAfterReset = false;
			var stateCount = m_Enabled.Length;
			for (var index = 0; index < stateCount; ++index)
				m_PreviousStates[index] = m_CurrentStates[index];
		}

		#endregion

		#region Public Properties

		public InputControlProvider controlProvider { get; set; }

		public int Count
		{
			get { return m_CurrentStates.Length; }
		}

		#endregion

		#region Fields

		private bool m_FirstFrameAfterReset = true;
		readonly float[] m_CurrentStates;
		readonly float[] m_PreviousStates;
		readonly bool[] m_Enabled;

		#endregion
	}
}
