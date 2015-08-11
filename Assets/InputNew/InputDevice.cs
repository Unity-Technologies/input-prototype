using System.Collections.Generic;

// TODO
// X device need to have some kind of identification string
// X create fakepad profile asset
// - look for matching profile on device registration

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

		public sealed override bool ProcessEvent( InputEvent inputEvent )
		{
			ProcessEventIntoState( inputEvent, state );
			return false;
		}

		public virtual bool ProcessEventIntoState( InputEvent inputEvent, InputState intoState )
		{
			return base.ProcessEvent( inputEvent );
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
