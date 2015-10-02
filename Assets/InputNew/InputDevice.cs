using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public abstract class InputDevice
		: InputControlProvider
	{
		private List<InputControlData> m_Controls;
		private InputState m_State;
		
		public override List<InputControlData> controls { get { return m_Controls; } }
		public override InputState state { get { return m_State; } }
		
		#region Constructors

		protected InputDevice(string deviceName, List<InputControlData> controls)
		{
			SetControls(controls);
			this.deviceName = deviceName;
		}

		protected InputDevice()
		{
			this.deviceName = "Generic Input Device";
		}

		#endregion

		#region Public Methods

		////REVIEW: right now the devices don't check whether the event was really meant for them; they go purely by the
		////  type of event they receive. should they check more closely?
		
		protected void SetControls(List<InputControlData> controls)
		{
			m_Controls = controls;
			m_State = new InputState(this);
		}
		
		public override sealed bool ProcessEvent(InputEvent inputEvent)
		{
			ProcessEventIntoState(inputEvent, state);
			return false;
		}

		public virtual bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			lastEventTime = inputEvent.time;
			return false;
		}

		public virtual bool RemapEvent(InputEvent inputEvent)
		{
			if (profile != null)
				profile.Remap(inputEvent);
			return false;
		}
		
		private void SetNameOverrides()
		{
			if (profile == null)
				return;
			
			// Assign control override names
			for (int i = 0; i < controlCount; i++) {
				string nameOverride = profile.GetControlNameOverride(i);
				if (nameOverride != null)
					SetControlNameOverride(i, nameOverride);
			}
		}

		#endregion

		#region Public Properties

		public bool connected { get; internal set; }

		public InputDeviceProfile profile
		{
			get { return m_Profile; } set { m_Profile = value; SetNameOverrides(); }
		}

		public string deviceName { get; protected set; }

		#endregion
		
		private InputDeviceProfile m_Profile;
	}
}
