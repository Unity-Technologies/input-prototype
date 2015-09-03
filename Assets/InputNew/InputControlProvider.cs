using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class InputControlProvider
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
		
		public InputControl anyButton
		{
			get
			{
				int count = controlCount;
				for (int i = 0; i < count; i++)
				{
					var control = this[i];
					if (control.controlType == InputControlType.Button && control.button)
						return control;
				}
				
				return this[0];
			}
		}

		#endregion

		#region Public Properties

		public InputState state
		{
			get { return m_State; }
		}
		
		public InputControlData GetControlData(int index)
		{
			return m_Controls[index];
		}
		
		public int controlCount
		{
			get { return m_Controls.Count; }
		}
		
		public InputControl this[int index]
		{
			get { return new InputControl(index, m_State); }
		}
		
		public InputControl this[string controlName]
		{
			get
			{
				for (var i = 0; i < m_Controls.Count; ++ i)
				{
					if (m_Controls[i].name == controlName)
						return this[i];
				}
				
				throw new KeyNotFoundException(controlName);
			}
		}

		public virtual string GetPrimarySourceName(int controlIndex, string buttonAxisFormattingString = "{0} & {1}")
		{
			return this[controlIndex].name;
		}
		
		protected void SetControlNameOverride (int controlIndex, string nameOverride)
		{
			InputControlData data = m_Controls[controlIndex];
			data.name = nameOverride;
			m_Controls[controlIndex] = data;
		}

		public float lastEventTime { get; protected set; }

		#endregion

		protected List<InputControlData> GetControls()
		{
			return m_Controls;
		}

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
