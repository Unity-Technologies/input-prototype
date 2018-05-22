#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputNew;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class JoystickInputToEvents
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

	const string k_AxisFormatString = "Analog{0}_Joy{1}";
	const int k_FirstAxis = 1;
	const int k_LastAxis = axisCount;
	const int k_NumAxes = k_LastAxis - k_FirstAxis;
	const int k_FirstButton = (int)KeyCode.Joystick1Button0;
	const int k_LastButton = (int)KeyCode.Joystick1Button19;
	const int k_NumButtons = k_LastButton - k_FirstButton;

	// Fake gamepad has 10 axes (index 0 - 9) and 20 buttons (index 10 - 29).
	public const int axisCount = 10;
	public const int buttonCount = 20;
	public const int joystickCount = 10;
	float[,] m_LastValues = new float[joystickCount, axisCount + buttonCount];

	static string[] s_Axes;

	static JoystickInputToEvents()
	{
		s_Axes = new string[k_NumAxes * joystickCount + 1];
		for (var device = 0; device < joystickCount; device++)
		{
			var deviceIndex = device + 1;
			for (var i = 0; i <= k_NumAxes; i++)
			{
				var index = device * k_NumAxes + i;
				s_Axes[index] = string.Format(k_AxisFormatString, i + k_FirstAxis, deviceIndex);
			}
		}
	}
	
	void SendAxisEvents()
	{
		for (var device = 0; device < joystickCount; device++)
		{
			var deviceIndex = device * k_NumAxes;
			for (var i = 0; i <= k_NumAxes; i++)
			{
				var index = deviceIndex + i;
				var value = Input.GetAxis(s_Axes[index]);
				SendEvent(device, i, value);
			}
		}
	}

	void SendButtonEvents()
	{
		for (int device = 0; device < joystickCount; device++)
		{
			var first = k_FirstButton + device * 20;

			for (var i = 0; i <= k_NumButtons; i++)
			{
				if (Input.GetKeyDown((KeyCode)(i + first)))
					SendEvent(device, axisCount + i, 1.0f);
				if (Input.GetKeyUp((KeyCode)(i + first)))
					SendEvent(device, axisCount + i, 0.0f);
			}
		}
	}

	void SendEvent(int deviceIndex, int controlIndex, float value)
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
