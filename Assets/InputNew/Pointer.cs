using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	/// <summary>
	/// A device that can point at and click on things.
	/// </summary>
	public abstract class Pointer
		: InputDevice
	{
		#region Constructors

		protected Pointer( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public override bool ProcessEventIntoState( InputEvent inputEvent, InputState intoState )
		{
			if ( base.ProcessEventIntoState( inputEvent, intoState ) )
				return true;

			var consumed = false;

			var moveEvent = inputEvent as PointerMoveEvent;
			if ( moveEvent != null )
			{
				consumed |= intoState.SetCurrentValue( ( int ) PointerControl.PositionX, moveEvent.position.x );
				consumed |= intoState.SetCurrentValue( ( int ) PointerControl.PositionY, moveEvent.position.y );
				consumed |= intoState.SetCurrentValue( ( int ) PointerControl.PositionZ, moveEvent.position.z );

				return consumed;
			}

			return false;
		}

		#endregion

		#region Non-Public Methods

		protected static List< InputControlData > CreateDefaultControls()
		{
			var controls = new List< InputControlData >();

			// Compounds.
			controls.Add( new InputControlData
				{
                      name = "Position"
                    , controlType = InputControlType.Vector3
                    , componentControlIndices = new int[ 3 ] { ( int ) PointerControl.PositionX, ( int ) PointerControl.PositionY, ( int ) PointerControl.PositionZ }
                } );

			// Primitives.
			controls.Add( new InputControlData { name = "PositionX", controlType = InputControlType.RelativeAxis } );
			controls.Add( new InputControlData { name = "PositionY", controlType = InputControlType.RelativeAxis } );
			controls.Add( new InputControlData { name = "PositionZ", controlType = InputControlType.RelativeAxis } );

			return controls;
		}

		#endregion

		#region Public Properties

		public Vector3 position
		{
			get { return state[ ( int ) PointerControl.Position ].vector3Value; }
		}

		public float pressure
		{
			get { return state[ ( int ) PointerControl.Pressure ].floatValue; }
		}

		#endregion
	}
}