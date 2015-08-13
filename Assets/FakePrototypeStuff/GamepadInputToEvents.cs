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

	void SendAxisEvents()
	{
		var leftThumbstickX_1 = Input.GetAxis("LeftThumbstickX_1");
		if (leftThumbstickX_1 != _lastLeftThumbstickX[0])
			SendEvent(0, GamepadControl.LeftThumbstickX, leftThumbstickX_1);
		var leftThumbstickY_1 = Input.GetAxis("LeftThumbstickY_1");
		if (leftThumbstickY_1 != _lastLeftThumbstickY[0])
			SendEvent(0, GamepadControl.LeftThumbstickY, leftThumbstickY_1);

		var rightThumbstickX_1 = Input.GetAxis("RightThumbstickX_1");
		if (rightThumbstickX_1 != _lastRightThumbstickX[0])
			SendEvent(0, GamepadControl.RightThumbstickX, rightThumbstickX_1);
		var rightThumbstickY_1 = Input.GetAxis("RightThumbstickY_1");
		if (rightThumbstickY_1 != _lastRightThumbstickY[0])
			SendEvent(0, GamepadControl.LeftThumbstickY, rightThumbstickY_1);

		////FIXME: for some reason, LeftTrigger_1 responds to both left and right trigger; Unity bug??
		var leftTrigger_1 = Input.GetAxis("LeftTrigger_1");
		if (leftTrigger_1 != _lastLeftTrigger[0])
			SendEvent(0, GamepadControl.LeftTrigger, leftTrigger_1);
		var rightTrigger_1 = Input.GetAxis("RightTrigger_1");
		if (rightTrigger_1 != _lastRightTrigger[0])
			SendEvent(0, GamepadControl.RightTrigger, rightTrigger_1);

		_lastLeftThumbstickX[0] = leftThumbstickX_1;
		_lastLeftThumbstickY[0] = leftThumbstickY_1;
		_lastRightThumbstickX[0] = rightThumbstickX_1;
		_lastRightThumbstickY[0] = rightThumbstickY_1;
		_lastLeftTrigger[0] = leftTrigger_1;
		_lastRightTrigger[0] = rightTrigger_1;
	}

	void SendButtonEvents()
	{
		if (Input.GetKeyDown(KeyCode.JoystickButton0))
			SendEvent(0, GamepadControl.ButtonA, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton0))
			SendEvent(0, GamepadControl.ButtonA, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton1))
			SendEvent(0, GamepadControl.ButtonB, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton1))
			SendEvent(0, GamepadControl.ButtonB, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton2))
			SendEvent(0, GamepadControl.ButtonX, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton2))
			SendEvent(0, GamepadControl.ButtonX, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton3))
			SendEvent(0, GamepadControl.ButtonY, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton3))
			SendEvent(0, GamepadControl.ButtonY, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton4))
			SendEvent(0, GamepadControl.LeftShoulder, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton4))
			SendEvent(0, GamepadControl.LeftShoulder, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton5))
			SendEvent(0, GamepadControl.RightShoulder, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton5))
			SendEvent(0, GamepadControl.RightShoulder, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton6))
			SendEvent(0, GamepadControl.Back, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton6))
			SendEvent(0, GamepadControl.Back, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton7))
			SendEvent(0, GamepadControl.Start, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton7))
			SendEvent(0, GamepadControl.Start, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton8))
			SendEvent(0, GamepadControl.LeftThumbstickPress, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton8))
			SendEvent(0, GamepadControl.LeftThumbstickPress, 0.0f);
		if (Input.GetKeyDown(KeyCode.JoystickButton9))
			SendEvent(0, GamepadControl.RightThumbstickPress, 1.0f);
		if (Input.GetKeyUp(KeyCode.JoystickButton9))
			SendEvent(0, GamepadControl.RightThumbstickPress, 0.0f);
	}

	void SendEvent(int deviceIndex, GamepadControl controlIndex, float value)
	{
		var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
		inputEvent.deviceType = typeof(Gamepad);
		inputEvent.deviceIndex = deviceIndex;
		inputEvent.controlIndex = (int)controlIndex;
		inputEvent.value = value;
		Debug.Log(inputEvent);
		InputSystem.QueueEvent(inputEvent);
	}

	#endregion

	#region Fields

	readonly float[] _lastLeftThumbstickX = new float[8];
	readonly float[] _lastLeftThumbstickY = new float[8];
	readonly float[] _lastRightThumbstickX = new float[8];
	readonly float[] _lastRightThumbstickY = new float[8];
	readonly float[] _lastLeftTrigger = new float[8];
	readonly float[] _lastRightTrigger = new float[8];

	#endregion
}
