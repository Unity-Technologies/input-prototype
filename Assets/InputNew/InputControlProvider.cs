using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class InputControlProvider
		: IInputControlProvider
	{
		#region Constructors

		public InputControlProvider(List<InputControlData> controls)
		{
			m_Controls = controls;
			m_State = new InputState(this);
		}
		
		protected InputControlProvider() {}

		#endregion

		#region Public Methods

		public virtual bool ProcessEvent(InputEvent inputEvent)
		{
			lastEventTime = inputEvent.time;
			return false;
		}

		#endregion

		#region Public Properties

		public InputState state
		{
			get { return m_State; }
		}

		////REVIEW: this view should be immutable
		public IList<InputControlData> controls
		{
			get { return m_Controls; }
		}

		public float lastEventTime { get; protected set; }

		#endregion

		protected void SetControls(List<InputControlData> controls)
		{
			m_Controls = controls;
			m_State = new InputState(this);
		}

		#region Fields

		private InputState m_State;
		private List<InputControlData> m_Controls;

		#endregion
	}
}
