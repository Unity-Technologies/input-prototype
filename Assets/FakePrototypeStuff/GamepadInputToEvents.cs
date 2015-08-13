using System;
using System.Collections.Generic;
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
		var leftThumbstickX1 = Input.GetAxis("LeftThumbstickX_1");
		if (leftThumbstickX1 != m_LastLeftThumbstickX[0])
			SendEvent(0, GamepadControl.LeftThumbstickX, leftThumbstickX1);
		var leftThumbstickY1 = Input.GetAxis("LeftThumbstickY_1");
		if (leftThumbstickY1 != m_LastLeftThumbstickY[0])
			SendEvent(0, GamepadControl.LeftThumbstickY, leftThumbstickY1);

		var rightThumbstickX1 = Input.GetAxis("RightThumbstickX_1");
		if (rightThumbstickX1 != m_LastRightThumbstickX[0])
			SendEvent(0, GamepadControl.RightThumbstickX, rightThumbstickX1);
		var rightThumbstickY1 = Input.GetAxis("RightThumbstickY_1");
		if (rightThumbstickY1 != m_LastRightThumbstickY[0])
			SendEvent(0, GamepadControl.LeftThumbstickY, rightThumbstickY1);

		////FIXME: for some reason, LeftTrigger_1 responds to both left and right trigger; Unity bug??
		var leftTrigger1 = Input.GetAxis("LeftTrigger_1");
		if (leftTrigger1 != m_LastLeftTrigger[0])
			SendEvent(0, GamepadControl.LeftTrigger, leftTrigger1);
		var rightTrigger1 = Input.GetAxis("RightTrigger_1");
		if (rightTrigger1 != m_LastRightTrigger[0])
			SendEvent(0, GamepadControl.RightTrigger, rightTrigger1);

		m_LastLeftThumbstickX[0] = leftThumbstickX1;
		m_LastLeftThumbstickY[0] = leftThumbstickY1;
		m_LastRightThumbstickX[0] = rightThumbstickX1;
		m_LastRightThumbstickY[0] = rightThumbstickY1;
		m_LastLeftTrigger[0] = leftTrigger1;
		m_LastRightTrigger[0] = rightTrigger1;
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

	readonly float[] m_LastLeftThumbstickX = new float[8];
	readonly float[] m_LastLeftThumbstickY = new float[8];
	readonly float[] m_LastRightThumbstickX = new float[8];
	readonly float[] m_LastRightThumbstickY = new float[8];
	readonly float[] m_LastLeftTrigger = new float[8];
	readonly float[] m_LastRightTrigger = new float[8];

	#endregion
}
