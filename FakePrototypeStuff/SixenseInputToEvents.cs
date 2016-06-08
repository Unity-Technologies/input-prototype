using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputNew;

public class SixenseInputToEvents : MonoBehaviour
{    
    public const uint kControllerCount = SixenseInput.MAX_CONTROLLERS;
	public const int kAxisCount = (int)VRInputDevice.VRControl.Analog9 + 1;
    private readonly float [,] m_LastAxisValues = new float[kControllerCount, kAxisCount];
	private readonly Vector3[] m_LastPositionValues = new Vector3[kControllerCount];
	private readonly Quaternion[] m_LastRotationValues = new Quaternion[kControllerCount];

	private void Awake()
	{
		if (!FindObjectOfType<SixenseInput>())
			gameObject.AddComponent<SixenseInput>();
	}

	private void Update()
    {
        if (!SixenseInput.IsBaseConnected(0))
            return;

        for (var i = 0; i < SixenseInput.MAX_CONTROLLERS; i++)
        {
            if (SixenseInput.Controllers[i] == null)
                continue;

            SendButtonEvents(i);
            SendAxisEvents(i);
            SendTrackingEvents(i);
        }
    }

    private float GetAxis(int deviceIndex, VRInputDevice.VRControl axis)
    {
        var controller = SixenseInput.Controllers[deviceIndex];
        if (controller != null)
        {
            switch (axis)
            {
                case VRInputDevice.VRControl.Trigger1:
                    return controller.Trigger;
                case VRInputDevice.VRControl.LeftStickX:
                    return controller.JoystickX;
                case VRInputDevice.VRControl.LeftStickY:
                    return controller.JoystickY;
            }
        }

        return 0f;
    }

	private void SendAxisEvents(int deviceIndex)
	{        
        for (var axis = 0; axis < kAxisCount; ++axis)
        {
            var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
            inputEvent.deviceType = typeof(VRInputDevice);
            inputEvent.deviceIndex = deviceIndex;
            inputEvent.controlIndex = axis;
            inputEvent.value = GetAxis(deviceIndex, (VRInputDevice.VRControl)axis);

			if (Mathf.Approximately(m_LastAxisValues[deviceIndex, axis], inputEvent.value))
				continue;

			m_LastAxisValues[deviceIndex, axis] = inputEvent.value;
            // Debug.Log("Axis event: " + inputEvent);

            InputSystem.QueueEvent(inputEvent);            
        }
	}

	private int GetButtonIndex(SixenseButtons button)
	{
		switch (button)
		{
			case SixenseButtons.ONE:
				return (int) VRInputDevice.VRControl.Action1;

			case SixenseButtons.TWO:
				return (int)VRInputDevice.VRControl.Action2;

			case SixenseButtons.THREE:
				return (int)VRInputDevice.VRControl.Action3;

			case SixenseButtons.FOUR:
				return (int)VRInputDevice.VRControl.Action4;

			case SixenseButtons.BUMPER:
				return (int)VRInputDevice.VRControl.Action5;

			case SixenseButtons.START:
				return (int)VRInputDevice.VRControl.Start;

			case SixenseButtons.JOYSTICK:
				return (int)VRInputDevice.VRControl.LeftStickButton;
		}

		// Not all buttons are currently mapped
		return -1;
	}

    private void SendButtonEvents(int deviceIndex)
    {
        var controller = SixenseInput.Controllers[deviceIndex];
        foreach (SixenseButtons button in Enum.GetValues(typeof(SixenseButtons)))
        {
            bool keyDown = controller.GetButtonDown(button);
            bool keyUp = controller.GetButtonUp(button);

            if (keyDown || keyUp)
            {
	            int buttonIndex = GetButtonIndex(button);
	            if (buttonIndex >= 0)
	            {
		            var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
		            inputEvent.deviceType = typeof(VRInputDevice);
		            inputEvent.deviceIndex = deviceIndex;
		            inputEvent.controlIndex = buttonIndex;
		            inputEvent.value = keyDown ? 1.0f : 0.0f;

		            // Debug.Log(string.Format("event: {0}; button: {1}", inputEvent, button));

		            InputSystem.QueueEvent(inputEvent);
	            }
            }
        }
    }

    private void SendTrackingEvents(int deviceIndex)
	{
        var controller = SixenseInput.Controllers[deviceIndex];

        var inputEvent = InputSystem.CreateEvent<VREvent>();
		inputEvent.deviceType = typeof (VRInputDevice);
		inputEvent.deviceIndex = deviceIndex;
        inputEvent.localPosition = controller.Position;
        inputEvent.localRotation = controller.Rotation;

		if (inputEvent.localPosition == m_LastPositionValues[deviceIndex] &&
			inputEvent.localRotation == m_LastRotationValues[deviceIndex])
			return;

		m_LastPositionValues[deviceIndex] = inputEvent.localPosition;
		m_LastRotationValues[deviceIndex] = inputEvent.localRotation;

		InputSystem.QueueEvent(inputEvent);
	}
}