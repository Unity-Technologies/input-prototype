using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputNew;
using Valve.VR;

public class ViveInputToEvents
	: MonoBehaviour
{
    private enum Handedness { Left, Right }
    private enum XorY { X, Y }

    public void Update() {
        for (Handedness hand = Handedness.Left; (int) hand <= (int) Handedness.Right; hand++) {
            var deviceIdx =
                SteamVR_Controller.GetDeviceIndex(hand == Handedness.Left
                    ? SteamVR_Controller.DeviceRelation.Leftmost
                    : SteamVR_Controller.DeviceRelation.Rightmost);
            if (deviceIdx == -1)
                continue;
            SendButtonEvents(deviceIdx);
            SendAxisEvents(deviceIdx);
            SendTrackingEvents(deviceIdx);
        }
    }

	public const int controllerCount = 10;
	public const int buttonCount = 64;
	public const int axisCount = 10; // 5 axes in openVR, each with X and Y.
	private float [,] m_LastAxisValues = new float[controllerCount,axisCount];
	private Vector3[] m_LastPositionValues = new Vector3[controllerCount];
	private Quaternion[] m_LastRotationValues = new Quaternion[controllerCount];

	private void SendAxisEvents(int deviceIdx)
	{
        int a = 0;
        for (int axis = (int)EVRButtonId.k_EButton_Axis0; axis <= (int)EVRButtonId.k_EButton_Axis4; ++axis) {
            Vector2 axisVec = SteamVR_Controller.Input(deviceIdx).GetAxis((EVRButtonId)axis);
            for (XorY xy = XorY.X; (int) xy <= (int) XorY.Y; xy++, a++)
            {
                var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                inputEvent.deviceType = typeof(VRInputDevice);
                inputEvent.deviceIndex = deviceIdx;
                inputEvent.controlIndex = a;
                inputEvent.value = xy == XorY.X ? axisVec.x : axisVec.y;

                if (Mathf.Approximately(m_LastAxisValues[deviceIdx, a], inputEvent.value)) {
                    continue;
                }
                m_LastAxisValues[deviceIdx, a] = inputEvent.value;
                // Debug.Log("Axis event: " + inputEvent);

                InputSystem.QueueEvent(inputEvent);
            }
        }
	}

    private void SendButtonEvents(int deviceIdx) {
        foreach (EVRButtonId button in Enum.GetValues(typeof(EVRButtonId))) {
            bool keyDown = SteamVR_Controller.Input(deviceIdx).GetPressDown(button);
            bool keyUp = SteamVR_Controller.Input(deviceIdx).GetPressUp(button);

            if (keyDown || keyUp) {
                var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                inputEvent.deviceType = typeof(VRInputDevice);
                inputEvent.deviceIndex = deviceIdx;
                inputEvent.controlIndex = axisCount + (int)button;
                inputEvent.value = keyDown ? 1.0f : 0.0f;

                // Debug.Log(string.Format("event: {0}; button: {1}", inputEvent, button));

                InputSystem.QueueEvent(inputEvent);
            }
        }
    }

    private void SendTrackingEvents(int deviceIdx)
	{
		var inputEvent = InputSystem.CreateEvent<VREvent>();
		inputEvent.deviceType = typeof (VRInputDevice);
		inputEvent.deviceIndex = deviceIdx;
        var pose = new SteamVR_Utils.RigidTransform(SteamVR_Controller.Input(deviceIdx).GetPose().mDeviceToAbsoluteTracking);
        inputEvent.localPosition = pose.pos;
        inputEvent.localRotation = pose.rot;

		if (inputEvent.localPosition == m_LastPositionValues[deviceIdx] &&
			inputEvent.localRotation == m_LastRotationValues[deviceIdx])
			return;

		m_LastPositionValues[deviceIdx] = inputEvent.localPosition;
		m_LastRotationValues[deviceIdx] = inputEvent.localRotation;

		InputSystem.QueueEvent(inputEvent);
	}
}