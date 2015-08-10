using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class InputControlProvider
		: IInputControlProvider
	{
		#region Constructors

		public InputControlProvider( List< InputControlData > controls )
		{
			_controls = controls;
			_state = new InputState( this );
		}

		#endregion

		#region Public Methods

		public virtual bool ProcessEvent( InputEvent inputEvent )
		{
			lastEventTime = inputEvent.time;
			return false;
		}

		#endregion

		#region Public Properties

		public InputState state 
		{
			get { return _state; }
		}

		////REVIEW: this view should be immutable
		public IList< InputControlData > controls
		{
			get { return _controls; }
		}

		public float lastEventTime { get; private set; }

		#endregion

		#region Fields

		private InputState _state;
		private List< InputControlData > _controls;

		#endregion
	}
}