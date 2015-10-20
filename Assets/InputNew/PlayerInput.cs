using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class PlayerInput : InputControlProvider
	{
		private ActionMap m_ActionMap;
		private bool m_AutoSwitch = false;
		private int m_SchemeIndex = 0;
		private List<SchemeInput> m_SchemeInputs;
		private InputEventTree treeNode { get; set; }

		public override List<InputControlData> controls { get { return currentScheme.controls; } }
		public override InputState state { get { return currentScheme.state; } }
		public bool autoSwitching { get { return m_AutoSwitch; } }
		public SchemeInput currentScheme { get { return m_SchemeInputs[m_SchemeIndex]; } }

		public PlayerInput(ActionMap actionMap)
		{
			m_ActionMap = actionMap;
			m_AutoSwitch = true;
			
			// TODO: Invoke Rebind when new input devices have been plugged in when m_AutoSwitch is true.
			Rebind();
		}

		public PlayerInput(SchemeInput schemeInput)
		{
			m_ActionMap = schemeInput.actionMap;
			m_SchemeInputs = new List<SchemeInput> ();
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

		public void Activate()
		{
			if (treeNode != null)
				return;
			treeNode = new InputEventTree
			{
				name = "Map"
				, processInput = ProcessEvent
				, beginNewFrame = BeginNewFrameEvent
			};
			InputSystem.consumerStack.children.Add(treeNode);
		}

		public void Deactivate()
		{
			if (treeNode == null)
				return;
			InputSystem.consumerStack.children.Remove(treeNode);
			treeNode = null;
		}

		public override bool ProcessEvent(InputEvent inputEvent)
		{
			if (currentScheme.ProcessEvent(inputEvent))
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

		public InputControl this[InputAction entry]
		{
			get { return state[entry.controlIndex]; }
		}

		void BeginNewFrameEvent()
		{
			currentScheme.state.BeginNewFrame();
			foreach (var deviceState in currentScheme.GetDeviceStates())
				deviceState.BeginNewFrame();
		}
	}
}
