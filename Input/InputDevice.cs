using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public abstract class InputDevice
		: InputControlProvider
	{
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
		
		public override sealed bool ProcessEvent(InputEvent inputEvent)
		{
			ProcessEventIntoState(inputEvent, state);
			return false;
		}

		public virtual bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			lastEventTime = inputEvent.time;

			if (inputEvent is DeviceStateResetEvent)
			{
				intoState.Reset();
				// All states that use this device should be reset, so don't use event.
				return false;
			}

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

		public PlayerDeviceAssignment assignment
		{
			get
			{
				return m_Assignment;
			}
			set
			{
				if (m_Assignment == value)
					return;

				var inputEvent = InputSystem.CreateEvent<DeviceStateResetEvent>();
				inputEvent.deviceType = GetType();
				inputEvent.deviceIndex = InputSystem.GetDeviceIndex(this);
				InputSystem.ExecuteEvent(inputEvent);
				
				m_Assignment = value;
			}
		}

		#endregion
		
		private InputDeviceProfile m_Profile;
		private PlayerDeviceAssignment m_Assignment = null;
	}

	public class DeviceStateResetEvent : InputEvent {}
}
