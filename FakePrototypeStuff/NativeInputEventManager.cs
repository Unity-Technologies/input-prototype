using System;
using System.Collections.Generic;
using UnityEngine.InputNew;
using UnityEngineInternal.Input;
namespace UnityEngine.Experimental.Input
{
	// Listens for native events and converts them into managed InputEvent instances.
	class NativeInputEventManager : MonoBehaviour
	{
		void Start()
		{
			NativeInputSystem.onEvents += OnEvent;
		}

		internal void OnEvent(int eventCount, IntPtr eventData)
		{
			var currentDataPtr = eventData;
			for (var i = 0; i < eventCount; i++)
			{
				unsafe
				{
					var eventPtr = (NativeInputEvent*)currentDataPtr;
					var time = (float)eventPtr->time;
					var device = eventPtr->deviceId + 1;

					switch (eventPtr->type)
					{
						case NativeInputEventType.Generic:
						{
							var nativeGenericEvent = (NativeGenericEvent*)eventPtr;

							var controlIndex = ControlIndexToVRIndex(nativeGenericEvent->controlIndex);
							if (controlIndex == -1)
							{
								currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
								continue;
							}

							var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
							inputEvent.deviceType = typeof(VRInputDevice);
							inputEvent.deviceIndex = device;
							inputEvent.controlIndex = controlIndex;
							//inputEvent.time = time;
							inputEvent.value = (float)nativeGenericEvent->scaledValue;

							InputSystem.QueueEvent(inputEvent);
						}
							break;
						case NativeInputEventType.Tracking:
						{
							NativeTrackingEvent* nativeTrackingEvent = (NativeTrackingEvent*)eventPtr;
							var localPosition = nativeTrackingEvent->localPosition;
							var localRotation = nativeTrackingEvent->localRotation;

							//if (localPosition == m_LastPositionValues[controller] && localRotation == m_LastRotationValues[controller])
							//	return;

							var inputEvent = InputSystem.CreateEvent<VREvent>();
							inputEvent.deviceType = typeof(VRInputDevice);
							inputEvent.deviceIndex = device;
							//inputEvent.time = time;
							inputEvent.localPosition = localPosition;
							inputEvent.localRotation = localRotation;

							//m_LastPositionValues[controller] = inputEvent.localPosition;
							//m_LastRotationValues[controller] = inputEvent.localRotation;

							InputSystem.QueueEvent(inputEvent);
							}
							break;
					}
					currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
				}
			}
		}

		static int ControlIndexToVRIndex(int axisIndex)
		{
			switch (axisIndex)
			{
				case 0:
					return (int)VRInputDevice.VRControl.LeftStickX;
				case 1:
					return (int)VRInputDevice.VRControl.LeftStickY;
				case 3:
					return (int)VRInputDevice.VRControl.LeftStickX;
				case 4:
					return (int)VRInputDevice.VRControl.LeftStickY;

				case 8:
					return (int)VRInputDevice.VRControl.Trigger1;
				case 9:
					return (int)VRInputDevice.VRControl.Trigger1;
				case 10:
					return (int)VRInputDevice.VRControl.Trigger2;
				case 11:
					return (int)VRInputDevice.VRControl.Trigger2;

				case 28:
					return (int)VRInputDevice.VRControl.Action1;
				case 29:
					return (int)VRInputDevice.VRControl.Action2;
				case 30:
					return (int)VRInputDevice.VRControl.Action1;
				case 31:
					return (int)VRInputDevice.VRControl.Action2;

				case 36:
					return (int)VRInputDevice.VRControl.LeftStickButton;
				case 37:
					return (int)VRInputDevice.VRControl.LeftStickButton;
				default:
					return -1;
			}
		}
	}
}