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
	private float[,] m_LastValues = new float[2, k_AxisCount + k_ButtonCount];
	
	private void SendAxisEvents()
	{
		int first = 1;
		int last = 10;
		for (int device = 0; device < 2; device++)
		{
			for (int i = 0; i <= last - first; i++)
			{
				var value = Input.GetAxis("Analog" + (i + first) + "_Joy" + (device + 1));
				SendEvent(device, i, value);
			}
		}
	}

	private void SendButtonEvents()
	{
		
		for (int device = 0; device < 2; device++)
		{
			int first = 0;
			int last = 0;
			switch (device)
			{
			case 0:
				first = (int)KeyCode.Joystick1Button0;
				last = (int)KeyCode.Joystick1Button19;
				break;
			case 1:
				first = (int)KeyCode.Joystick2Button0;
				last = (int)KeyCode.Joystick2Button19;
				break;
			}
			
			for (int i = 0; i <= last - first; i++)
			{
				if (Input.GetKeyDown((KeyCode)(i + first)))
					SendEvent(device, k_AxisCount + i, 1.0f);
				if (Input.GetKeyUp((KeyCode)(i + first)))
					SendEvent(device, k_AxisCount + i, 0.0f);
			}
		}
	}

	private void SendEvent(int deviceIndex, int controlIndex, float value)
	{
		if (value == m_LastValues[deviceIndex, controlIndex])
			return;
		m_LastValues[deviceIndex, controlIndex] = value;
		
		var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
		inputEvent.deviceType = typeof(Gamepad);
		inputEvent.deviceIndex = deviceIndex;
		inputEvent.controlIndex = controlIndex;
		inputEvent.value = value;
		InputSystem.QueueEvent(inputEvent);
	}

	#endregion
}
