using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public abstract class InputDevice
		: InputControlProvider
	{
		#region Constructors

		protected InputDevice( string deviceName, List< InputControlData > controls )
			: base( controls )
		{
			this.deviceName = deviceName;
		}

		#endregion

		#region Public Methods

		////REVIEW: right now the devices don't check whether the event was really meant for them; they go purely by the
		////  type of event they receive. should they check more closely?

		public sealed override bool ProcessEvent( InputEvent inputEvent )
		{
			ProcessEventIntoState( inputEvent, state );
			return false;
		}

		public virtual bool ProcessEventIntoState( InputEvent inputEvent, InputState intoState )
		{
			lastEventTime = inputEvent.time;
			return false;
		}

		public virtual bool RemapEvent( InputEvent inputEvent )
		{
			if ( profile != null )
				profile.Remap( inputEvent );
			return false;
		}

		#endregion

		#region Public Properties

		public bool connected { get; internal set; }

		public InputDeviceProfile profile { get; set; }

		public string deviceName { get; private set; }
		
		#endregion
	}
}
