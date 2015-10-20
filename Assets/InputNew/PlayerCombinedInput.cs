using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class PlayerCombinedInput : PlayerInput
	{
		private ActionMap m_ActionMap;
		private bool m_AutoSwitch = false;
		private int m_SchemeIndex = 0;
		private List<PlayerSchemeInput> m_SchemeInputs;

		public override List<InputControlData> controls { get { return m_SchemeInputs[m_SchemeIndex].controls; } }
		public override InputState state { get { return m_SchemeInputs[m_SchemeIndex].state; } }
		public override ActionMap actionMap { get { return m_SchemeInputs[m_SchemeIndex].actionMap; } }
		public override int controlSchemeIndex { get { return m_SchemeInputs[m_SchemeIndex].controlSchemeIndex; } }
		protected override List<InputState> deviceStates { get { return m_SchemeInputs[m_SchemeIndex].GetDeviceStates(); } }
		public bool autoSwitching { get { return m_AutoSwitch; } }

		public PlayerCombinedInput(ActionMap actionMap)
		{
			m_ActionMap = actionMap;
			m_AutoSwitch = true;
			
			// TODO: Invoke Rebind when new input devices have been plugged in when m_AutoSwitch is true.
			Rebind();
		}
		
		public PlayerCombinedInput(PlayerSchemeInput schemeInput)
		{
			m_ActionMap = schemeInput.actionMap;
			m_SchemeInputs = new List<PlayerSchemeInput> ();
			m_SchemeInputs.Add (schemeInput);
			m_AutoSwitch = false;
		}

		public void Rebind()
		{
			m_SchemeInputs = InputSystem.CreateAllPotentialPlayers(m_ActionMap, false).ToList();
			
			float mostRecentTime = 0;
			for (int i = 0; i < m_SchemeInputs.Count; i++)
			{
				float time = m_SchemeInputs[i].lastEventTime;
				if (time > mostRecentTime)
				{
					mostRecentTime = time;
					m_SchemeIndex = i;
				}
			}
		}

		public override bool ProcessEvent(InputEvent inputEvent)
		{
			if (m_SchemeInputs[m_SchemeIndex].ProcessEvent(inputEvent))
				return true;
			
			if (!m_AutoSwitch)
				return false;
			
			for (int i = 0; i < m_SchemeInputs.Count; i++)
			{
				if (i == m_SchemeIndex)
					continue;
				bool consumed = m_SchemeInputs[i].ProcessEvent(inputEvent);
				if (consumed)
				{
					m_SchemeIndex = i;
					return true;
				}
			}
			return false;
		}
	}
}
