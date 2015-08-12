using UnityEngine;
using System.Collections.Generic;

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

		public static Keyboard CreateDefault()
		{
			var controls = new List< InputControlData >();
			
			// TODO REMOVE WHEN WE HAVE WORKING DUMMY KEYBOARD
			int keyCount = System.Enum.GetValues( typeof( KeyCode ) ).Length;
			for ( int i = 0; i < keyCount; i++ )
				controls.Add (new InputControlData ());
			
			return new Keyboard( "Generic Keyboard", controls );
		}

		#endregion
	}
}