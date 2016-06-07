using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputNew;
using UnityEngine.VR;
using Valve.VR;

public class ViveInputToEvents
	: MonoBehaviour
{
    private enum Handedness { Left, Right }

    public void Update()
	{
		SendButtonEvents();
		//SendAxisEvents();
		//SendTrackingEvents();
	}

	public const int controllerCount = 10;
	public const int buttonCount = 64;
	public const int axisCount = 10;
	private float [,] m_LastAxisValues = new float[controllerCount,axisCount];
	private Vector3[] m_LastPositionValues = new Vector3[controllerCount];
	private Quaternion[] m_LastRotationValues = new Quaternion[controllerCount];

	private void SendAxisEvents()
	{
		for (int device = 0; device < controllerCount; ++device)
		{
			for (int axis = 0; axis < axisCount; ++axis)
			{
				var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
				inputEvent.deviceType = typeof (VRInputDevice);
				inputEvent.deviceIndex = device;
				inputEvent.controlIndex = axis;
                // TODO: Replace with the SteamVR equivalent
                //inputEvent.value = UnityEngine.VR.InputTracking.GetAxis(device, axis);

				if (Mathf.Approximately(m_LastAxisValues[device, axis], inputEvent.value))
					continue;
				m_LastAxisValues[device, axis] = inputEvent.value;

				Debug.Log("Axis event: " + inputEvent);

				InputSystem.QueueEvent(inputEvent);
			}
		}
	}

    private void SendButtonEvents() {
        for (Handedness hand = Handedness.Left; (int)hand <= (int)Handedness.Right; hand++) {
            int b = 0;
            foreach (EVRButtonId button in Enum.GetValues(typeof(EVRButtonId))) {
                var deviceIdx = SteamVR_Controller.GetDeviceIndex(hand == Handedness.Left ? SteamVR_Controller.DeviceRelation.Leftmost : SteamVR_Controller.DeviceRelation.Rightmost);
                bool keyDown = SteamVR_Controller.Input(deviceIdx).GetPressDown(button);
                bool keyUp = SteamVR_Controller.Input(deviceIdx).GetPressUp(button);

                if (keyDown || keyUp) {
                    var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                    inputEvent.deviceType = typeof(VRInputDevice);
                    inputEvent.deviceIndex = deviceIdx;
                    inputEvent.controlIndex = axisCount + ++b;
                    inputEvent.value = keyDown ? 1.0f : 0.0f;

                    Debug.Log(string.Format("event: {0}; button: {1}", inputEvent, button));

                    InputSystem.QueueEvent(inputEvent);
                }
            }
        }
    }

    private void SendTrackingEvents()
	{
		for (int device = 0; device < controllerCount; ++device)
		{
			var inputEvent = InputSystem.CreateEvent<VREvent>();
			inputEvent.deviceType = typeof (VRInputDevice);
			inputEvent.deviceIndex = device;
            // TODO: Replace with the SteamVR equivalent
            //inputEvent.localPosition = UnityEngine.VR.InputTracking.GetLocalPosition((VRNode) device);
            //inputEvent.localRotation = UnityEngine.VR.InputTracking.GetLocalRotation((VRNode) device);

			if (inputEvent.localPosition == m_LastPositionValues[device] &&
				inputEvent.localRotation == m_LastRotationValues[device])
				continue;

			m_LastPositionValues[device] = inputEvent.localPosition;
			m_LastRotationValues[device] = inputEvent.localRotation;

			InputSystem.QueueEvent(inputEvent);
		}
	}
}