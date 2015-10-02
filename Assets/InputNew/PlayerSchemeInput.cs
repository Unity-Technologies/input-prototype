using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class PlayerSchemeInput : PlayerInput
	{
		private List<InputControlData> m_Controls;
		private InputState m_State;
		private ActionMap m_ActionMap;
		private int m_ControlSchemeIndex = 0;
		private List<InputState> m_DeviceStates;

		public override List<InputControlData> controls { get { return m_Controls; } }
		public override InputState state { get { return m_State; } }
		public override ActionMap actionMap { get { return m_ActionMap; } }
		public override int controlSchemeIndex { get { return m_ControlSchemeIndex; } }
		protected override List<InputState> deviceStates { get { return m_DeviceStates; } }

		public PlayerSchemeInput(ActionMap actionMap, int controlSchemeIndex, List<InputState> deviceStates)
		{
			Setup(actionMap, controlSchemeIndex, deviceStates);
		}

		private void SetControls(List<InputControlData> controls)
		{
			m_Controls = controls;
			m_State = new InputState(this);
		}

		protected void Setup(ActionMap actionMap, int controlSchemeIndex, List<InputState> deviceStates)
		{
			m_ControlSchemeIndex = controlSchemeIndex;
			m_DeviceStates = deviceStates;
			m_ActionMap = actionMap;
			
			// Create list of controls from InputMap.
			var controls = new List<InputControlData>();
			foreach (var entry in actionMap.entries)
			{
				////REVIEW: why are we making copies here?
				var control = new InputControlData
				{
					name = entry.controlData.name,
					controlType = entry.controlData.controlType,
					////REVIEW: doesn't handle compounds
				};
				controls.Add(control);
			}
			SetControls(controls);
		}
	}
}
