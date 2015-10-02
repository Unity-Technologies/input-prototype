using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class PlayerCustomInput : PlayerInput
	{
		private PlayerInput m_Input;

		public PlayerCustomInput(PlayerInput playerInput)
		{
			m_Input = playerInput;
		}

		public override List<InputControlData> controls { get { return m_Input.controls; } }
		public override InputState state { get { return m_Input.state; } }
		public override ActionMap actionMap { get { return m_Input.actionMap; } }
		public override int controlSchemeIndex { get { return m_Input.controlSchemeIndex; } }
		protected override List<InputState> deviceStates { get { return m_Input.GetDeviceStates(); } }
	}
}
