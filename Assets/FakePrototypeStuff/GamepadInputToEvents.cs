using UnityEngine;
using UnityEngine.InputNew;

public class GamepadInputToEvents
	: MonoBehaviour
{
	#region Public Methods

	public void Update()
	{
		SendButtonEvents();
		SendAxisEvents();
	}

	#endregion

	#region Non-Public Methods

	// Fake gamepad has 10 axes (index 0 - 9) and 20 buttons (index 10 - 29).
	const int k_AxisCount = 10;
	const int k_ButtonCount = 20;
	private float[] m_LastValues = new float[k_AxisCount + k_ButtonCount];
	
	private void SendAxisEvents()
	{
		int first = 1;
		int last = 10;
		for ( int i = 0; i <= last - first; i++ )
		{
			var value = Input.GetAxis( "Analog" + (i + first) );
			SendEvent( 0, i, value );
		}
	}

	private void SendButtonEvents()
	{
		int first = (int)KeyCode.JoystickButton0;
		int last = (int)KeyCode.JoystickButton19;
		for (int i = 0; i <= last - first; i++)
		{
			if (Input.GetKeyDown( (KeyCode)(i + first)))
				SendEvent(0, k_AxisCount + i, 1.0f );
			if (Input.GetKeyUp( (KeyCode)(i + first)))
				SendEvent(0, k_AxisCount + i, 0.0f);
		}
	}

	private void SendEvent(int deviceIndex, int controlIndex, float value)
	{
		if (value == m_LastValues[controlIndex])
			return;
		m_LastValues[controlIndex] = value;
		
		var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
		inputEvent.deviceType = typeof(Gamepad);
		inputEvent.deviceIndex = deviceIndex;
		inputEvent.controlIndex = controlIndex;
		inputEvent.value = value;
		InputSystem.QueueEvent(inputEvent);
	}

	#endregion
}
