using UnityEngine;
using UnityEngine.InputNew;

public class GamepadInputToEvents
	: MonoBehaviour
{
	public void Update()
	{
		if ( Input.GetKeyDown( KeyCode.JoystickButton0 ) )
			SendEvent( 0, GamepadControl.ButtonA, 1.0f );
		if ( Input.GetKeyUp( KeyCode.JoystickButton0 ) )
			SendEvent( 0, GamepadControl.ButtonA, 0.0f );
	}

	private void SendEvent( int deviceIndex, GamepadControl controlIndex, float value )
	{
		var inputEvent = InputSystem.CreateEvent< GenericControlEvent >();
		inputEvent.deviceType = typeof( Gamepad );
		inputEvent.deviceIndex = deviceIndex; 
		inputEvent.controlIndex = ( int ) controlIndex;
		inputEvent.value = value;
		Debug.Log( inputEvent );
		InputSystem.QueueEvent( inputEvent );	
	}
}
