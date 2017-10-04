using System;
using UnityEngine.InputNew;
using UnityEngineInternal.Input;

namespace UnityEngine.Experimental.Input
{
	// Listens for native events and converts them into managed InputEvent instances.
	class NativeInputEventManager : MonoBehaviour
	{
		// TODO: Register native devices
		OculusTouchController tempController;

		void Start()
		{
			tempController = new OculusTouchController();
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
					var device = eventPtr->deviceId + 1;

					switch (eventPtr->type)
					{
						case NativeInputEventType.Generic:
						{
							var nativeGenericEvent = (NativeGenericEvent*)eventPtr;

							var controlIndex = tempController.GenericControlIndexFromNative(nativeGenericEvent->controlIndex);
							if (controlIndex == -1)
							{
								currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
								continue;
							}

							var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
							inputEvent.deviceType = typeof(XRInputDevice);
							inputEvent.deviceIndex = device;
							inputEvent.controlIndex = controlIndex;
							inputEvent.value = (float)nativeGenericEvent->scaledValue;

							InputSystem.QueueEvent(inputEvent);
						}
							break;
						case NativeInputEventType.Tracking:
						{
							var nativeTrackingEvent = (NativeTrackingEvent*)eventPtr;
							var localPosition = nativeTrackingEvent->localPosition;
							var localRotation = nativeTrackingEvent->localRotation;

							var skip = device != 3 && device != 4;

							if (skip)
							{
								currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
								continue;
							}

							var inputEvent = InputSystem.CreateEvent<TrackingEvent>();
							inputEvent.deviceType = typeof(XRInputDevice);
							inputEvent.deviceIndex = device;
							inputEvent.nodeId = nativeTrackingEvent->nodeId;
							inputEvent.availableFields = (TrackingEvent.Flags)nativeTrackingEvent->availableFields;
							inputEvent.localPosition = localPosition;
							inputEvent.localRotation = localRotation;
							inputEvent.acceleration = nativeTrackingEvent->acceleration;
							inputEvent.angularAcceleration = nativeTrackingEvent->angularAcceleration;
							inputEvent.velocity = nativeTrackingEvent->velocity;
							inputEvent.angularVelocity = nativeTrackingEvent->angularVelocity;

							InputSystem.QueueEvent(inputEvent);
						}
							break;
					}
					currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
				}
			}
		}
	}
}