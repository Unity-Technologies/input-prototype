using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public abstract class InputControlProvider
	{
		public abstract InputState state { get; }
		public abstract List<InputControlData> controls { get; }

		public virtual bool ProcessEvent(InputEvent inputEvent)
		{
			lastEventTime = inputEvent.time;
			return false;
		}

		public virtual InputControl anyButton
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

		public InputControlData GetControlData(int index)
		{
			return controls[index];
		}
		
		public int controlCount
		{
			get { return controls.Count; }
		}
		
		public InputControl this[int index]
		{
			get { return new InputControl(index, state); }
		}
		
		public InputControl this[string controlName]
		{
			get
			{
				for (var i = 0; i < controls.Count; ++ i)
				{
					if (controls[i].name == controlName)
						return this[i];
				}
				
				throw new KeyNotFoundException(controlName);
			}
		}

		public virtual string GetPrimarySourceName(int controlIndex, string buttonAxisFormattingString = "{0} & {1}")
		{
			return this[controlIndex].name;
		}
		
		protected void SetControlNameOverride(int controlIndex, string nameOverride)
		{
			InputControlData data = controls[controlIndex];
			data.name = nameOverride;
			controls[controlIndex] = data;
		}

		public float lastEventTime { get; protected set; }

		protected List<InputControlData> GetControls()
		{
			return controls;
		}
	}
}
