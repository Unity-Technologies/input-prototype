using UnityEngine;
using UnityEngine.InputNew;

public class MouseInputToEvents
	: MonoBehaviour
{
	public void Update()
	{
		SendButtonEvents();
		SendMoveEvent();
	}

	private void SendButtonEvents()
	{
		if ( Input.GetKeyDown( KeyCode.Mouse0 ) )
			SendClickEvent( PointerControl.LeftButton, true );	
		if ( Input.GetKeyUp( KeyCode.Mouse0 ) )
			SendClickEvent( PointerControl.LeftButton, false );	
	}

	private void SendMoveEvent()
	{
		var newMousePosition = Input.mousePosition;
		if ( newMousePosition == _lastMousePosition )
			return;

		var inputEvent = InputSystem.CreateEvent< PointerMoveEvent >();
		inputEvent.deviceType = typeof( Mouse );
		inputEvent.deviceIndex = 0;
		inputEvent.delta = newMousePosition - _lastMousePosition;
		inputEvent.position = newMousePosition;

		InputSystem.QueueEvent( inputEvent );

		_lastMousePosition = newMousePosition;
	}

	private void SendClickEvent( PointerControl controlIndex, bool clicked )
	{
		////REVIEW: should this be a pointer-specific event type?
		var inputEvent = InputSystem.CreateEvent< GenericControlEvent >();
		inputEvent.deviceType = typeof( Mouse );
		inputEvent.deviceIndex = 0;
		inputEvent.controlIndex = ( int ) controlIndex;
		inputEvent.value = clicked ? 1.0f : 0.0f; 
		Debug.Log( inputEvent );
		InputSystem.QueueEvent( inputEvent );	
	}

	private Vector3 _lastMousePosition;
}
