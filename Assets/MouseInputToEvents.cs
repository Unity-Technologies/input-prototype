using UnityEngine;
using UnityEngine.InputNew;

public class MouseInputToEvents
	: MonoBehaviour
{
	public void Update()
	{
		SendMoveEvent();
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

	private Vector3 _lastMousePosition;
}
