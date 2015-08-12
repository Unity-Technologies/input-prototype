using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
	public class Keyboard
		: InputDevice
	{
		#region Constructors

		public Keyboard( string deviceName, List< InputControlData > controls )
			: base( deviceName, controls )
		{
		}

		#endregion

		#region Public Methods

		public override bool ProcessEventIntoState( InputEvent inputEvent, InputState intoState )
		{
			var consumed = false;

			var keyEvent = inputEvent as KeyboardEvent;
			if ( keyEvent != null )
				consumed |= state.SetCurrentValue( ( int ) keyEvent.key, keyEvent.isDown );

			if ( consumed )
				return true;

			return base.ProcessEventIntoState( inputEvent, intoState );
		}

		public static Keyboard CreateDefault()
		{
			var controlCount = EnumHelpers.GetValueCount< KeyControl >();
			var controls = Enumerable.Repeat( new InputControlData(), controlCount ).ToList();

			for ( var i = 0; i < controlCount; ++ i )
				InitKey( controls, ( KeyControl ) i );
			
			return new Keyboard( "Generic Keyboard", controls );
		}

		#endregion

		#region Non-Public Methods

		private static void InitKey( List< InputControlData > controls, KeyControl key )
		{
			controls[ ( int ) key ] = new InputControlData
				{
					  name = key.ToString()
					, controlType = InputControlType.Button
				};
		}
		
		#endregion
	}
}