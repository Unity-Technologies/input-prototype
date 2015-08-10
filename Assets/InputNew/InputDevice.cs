using System.Collections.Generic;

////TODO: connect/disconnect support

namespace UnityEngine.InputNew
{
	public abstract class InputDevice
		: InputControlProvider
	{
		#region Constructors

		protected InputDevice( List< InputControlData > controls )
			: base( controls )
		{
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
			////TODO: implement remapping
			return false;
		}

		#endregion
	}
}