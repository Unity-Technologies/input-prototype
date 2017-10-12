using System;
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
				    var device = InputSystem.GetDeviceFromNativeID(eventPtr->deviceId);

				    if (device != null)
				    {
                        switch (eventPtr->type)
                        {
                            case NativeInputEventType.Generic:
                            {
                                var xrDevice = device as TrackedInputDevice;
                                if (xrDevice == null)
                                {
                                    break;
                                }

                                var nativeGenericEvent = (NativeGenericEvent*)eventPtr;
                                var controlIndex = xrDevice.GenericControlIndexFromNative(nativeGenericEvent->controlIndex);
                                if (controlIndex == -1)
                                {
                                    break;
                                }

                                var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                                inputEvent.device = device;
                                inputEvent.controlIndex = controlIndex;
                                inputEvent.value = (float)nativeGenericEvent->scaledValue;

                                InputSystem.QueueEvent(inputEvent);
                            }
                                break;
                            case NativeInputEventType.Tracking:
                            {
                                var nativeTrackingEvent = (NativeTrackingEvent*)eventPtr;

                                var inputEvent = InputSystem.CreateEvent<TrackingEvent>();
                                inputEvent.device = device;
                                inputEvent.nodeId = nativeTrackingEvent->nodeId;
                                inputEvent.availableFields = (TrackingEvent.Flags)nativeTrackingEvent->availableFields;
                                inputEvent.localPosition = nativeTrackingEvent->localPosition;
                                inputEvent.localRotation = nativeTrackingEvent->localRotation;
                                inputEvent.acceleration = nativeTrackingEvent->acceleration;
                                inputEvent.angularAcceleration = nativeTrackingEvent->angularAcceleration;
                                inputEvent.velocity = nativeTrackingEvent->velocity;
                                inputEvent.angularVelocity = nativeTrackingEvent->angularVelocity;

                                InputSystem.QueueEvent(inputEvent);
                            }
                                break;
                        }
                    }

                    currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
				}
			}
		}
	}
}