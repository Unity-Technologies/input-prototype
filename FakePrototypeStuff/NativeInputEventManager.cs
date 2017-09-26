using System;
using System.Collections.Generic;
using UnityEngine.InputNew;
using UnityEngineInternal.Input;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Experimental.Input
{
	// Listens for native events and converts them into managed InputEvent instances.
	internal class NativeInputEventManager : MonoBehaviour
	{
		//internal IInputEventManager m_EventManager;
		//internal INativeInputDeviceManager m_NativeDeviceManager;
		//bool m_IsInitialized;

		//public Action onReceivedEvents { get; set; }

		//internal void Initialize(IInputEventManager eventManager, INativeInputDeviceManager nativeDeviceManager)
		//{
		//	if (m_IsInitialized)
		//		return;

		//	m_EventManager = eventManager;
		//	m_NativeDeviceManager = nativeDeviceManager;

		//	NativeInputSystem.onEvents += OnReceiveEvents;

		//	m_IsInitialized = true;
		//}

		//internal void Uninitialize()
		//{
		//	if (!m_IsInitialized)
		//		return;

		//	m_EventManager = null;
		//	m_NativeDeviceManager = null;
		//	NativeInputSystem.onEvents -= OnReceiveEvents;

		//	m_IsInitialized = false;
		//}

		void Start()
		{
			NativeInputSystem.onEvents += OnReceiveEvents;
		}

		public static readonly Dictionary<int, Dictionary<int, float>> values = new Dictionary<int, Dictionary<int, float>>();

		void OnGUI()
		{
			GUILayout.BeginHorizontal();
			foreach (var kvp in values)
			{
				GUILayout.BeginVertical();
				foreach (var val in kvp.Value)
				{
					GUILayout.Label(string.Format("Device {0} Control {1}: {2:f2}", kvp.Key, val.Key, val.Value));
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		// This method reads out NativeInputEvents directly from unmanaged memory
		// and turns them into InputEvent instances and puts them on the InputEventQueue.
		internal void OnReceiveEvents(int eventCount, IntPtr eventData)
		{
			//var queue = m_EventManager.queue;
			//var pool = m_EventManager.pool;
			var zeroTime = NativeInputSystem.zeroEventTime;
			var currentTime = Time.time;
#if UNITY_EDITOR
			var currentRealTime = EditorApplication.timeSinceStartup;
#else
            var currentRealTime = Time.realtimeSinceStartup;
#endif

			////TODO: disconnect/reconnect events
			////TODO: text events

			var currentDataPtr = eventData;
			for (var i = 0; i < eventCount; ++i)
			{
				unsafe
				{
					NativeInputEvent* eventPtr = (NativeInputEvent*)currentDataPtr;

					// In the editor, we have jumps in time progression as time will reset when going in and out of play mode.
					// This means that when adjusting from real time to game time here, we may end up with events that have happened
					// "before time started." We simply discard those events.

					var eventTime = eventPtr->time;
					var time = eventTime - zeroTime;
					var device = eventPtr->deviceId + 1;
					if (time >= 0.0)
					{
						switch (eventPtr->type)
						{
							case NativeInputEventType.Generic:
								{
									NativeGenericEvent* nativeGenericEvent = (NativeGenericEvent*)eventPtr;

									if (nativeGenericEvent->controlIndex == 8)
										Debug.Log(nativeGenericEvent->scaledValue);

									//Debug.Log(nativeGenericEvent->controlIndex);
									var controlIndex = ControlIndexToVRIndex(nativeGenericEvent->controlIndex);

									Dictionary<int, float> vals;
									if (!values.TryGetValue(device, out vals))
									{
										vals = new Dictionary<int, float>();
										values[device] = vals;
									}

									var inputEvent = InputSystem.CreateEvent<InputNew.GenericControlEvent>();
									//inputEvent.time = (float)time;
									inputEvent.deviceType = typeof(VRInputDevice);
									inputEvent.deviceIndex = device;
									inputEvent.controlIndex = controlIndex;
									inputEvent.value = (float)nativeGenericEvent->scaledValue;
									//inputEvent.rawValue = nativeGenericEvent->rawValue;
									vals[nativeGenericEvent->controlIndex] = inputEvent.value;

									if (controlIndex == -1)
										continue;

									//Debug.Log(inputEvent);
									InputSystem.QueueEvent(inputEvent);
								}
								break;
							default:
								//Debug.Log(eventPtr->type);
								break;
						}
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
				case 11:
					return (int)VRInputDevice.VRControl.Trigger2;
				case 12:
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