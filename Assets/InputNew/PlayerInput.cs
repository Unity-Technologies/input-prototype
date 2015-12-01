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

		public ActionMap actionMap { get { return m_ActionMap; } }
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

		void Rebind()
		{
			m_SchemeInputs = InputSystem.CreateAllPotentialPlayers(m_ActionMap).ToList();
			
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

		public bool active
		{
			get { return (treeNode != null); }
			set
			{
				if ((treeNode != null) == value)
					return;
				if (value)
				{
					treeNode = new InputEventTree
					{
						name = "Map"
						, processInput = ProcessEvent
						, beginFrame = BeginFrameEvent
						, endFrame = EndFrameEvent
					};
					InputSystem.consumerStack.children.Add(treeNode);
				}
				else
				{
					InputSystem.consumerStack.children.Remove(treeNode);
					treeNode = null;
					currentScheme.Reset();
				}
			}
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

		void BeginFrameEvent()
		{
			currentScheme.BeginFrame();
		}
		
		void EndFrameEvent()
		{
			currentScheme.EndFrame();
		}
	}
}
