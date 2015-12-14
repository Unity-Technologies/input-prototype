using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class ActionMapInput : IInputControlProvider
	{
		private ActionMap m_ActionMap;
		private bool m_AutoSwitch = false;
		private int m_SchemeIndex = 0;
		private List<ControlSchemeInput> m_ControlSchemeInputs;
		private InputEventTree treeNode { get; set; }

		public ActionMap actionMap { get { return m_ActionMap; } }
		public List<InputControlData> controlDataList { get { return currentControlScheme.controlDataList; } }
		public InputState state { get { return currentControlScheme.state; } }
		public bool autoSwitching { get { return m_AutoSwitch; } }
		public ControlSchemeInput currentControlScheme { get { return m_ControlSchemeInputs[m_SchemeIndex]; } }

		public ActionMapInput(ActionMap actionMap)
		{
			m_ActionMap = actionMap;
			m_AutoSwitch = true;
			
			// TODO: Invoke Rebind when new input devices have been plugged in when m_AutoSwitch is true.
			Rebind();
		}

		public ActionMapInput(ControlSchemeInput controlSchemeInput)
		{
			m_ActionMap = controlSchemeInput.actionMap;
			m_ControlSchemeInputs = new List<ControlSchemeInput>();
			m_ControlSchemeInputs.Add(controlSchemeInput);
			m_AutoSwitch = false;
		}

		void Rebind()
		{
			m_ControlSchemeInputs = InputSystem.CreateAllPotentialPlayers(m_ActionMap).ToList();
			
			float mostRecentTime = 0;
			for (int i = 0; i < m_ControlSchemeInputs.Count; i++)
			{
				float time = m_ControlSchemeInputs[i].lastEventTime;
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
					currentControlScheme.Reset();
				}
			}
		}

		public InputControl this[int index]
		{
			get { return currentControlScheme[index]; }
		}

		public bool ProcessEvent(InputEvent inputEvent)
		{
			if (currentControlScheme.ProcessEvent(inputEvent))
				return true;
			
			if (!m_AutoSwitch)
				return false;
			
			for (int i = 0; i < m_ControlSchemeInputs.Count; i++)
			{
				if (i == m_SchemeIndex)
					continue;
				bool consumed = m_ControlSchemeInputs[i].ProcessEvent(inputEvent);
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
			currentControlScheme.BeginFrame();
		}
		
		void EndFrameEvent()
		{
			currentControlScheme.EndFrame();
		}
	}
}
