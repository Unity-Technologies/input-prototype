using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngineInternal.Input;

//// - solve mapping of device type names from control maps to device types at runtime

namespace UnityEngine.InputNew
{
	public static class InputSystem
	{
        struct NativeDeviceDescriptor
        {
            public string @interface;
            public string type;
            public string product;
            public string manufacturer;
            public string version;
            public string serial;
        }

        // For now, initialize prototype stuff here.
        // This should not be included here in final code.
        static InputSystem()
		{
			s_Devices = new InputDeviceManager();

			GameObject go = new GameObject("Input Prototype Controller");
			go.hideFlags = HideFlags.HideAndDontSave;

			go.AddComponent<InputManager>();
			go.AddComponent<InputManagerEndFrame>();
			go.AddComponent<MouseInputToEvents>();
			go.AddComponent<TouchInputToEvents>();
			go.AddComponent<NativeInputEventManager>();
			go.AddComponent<ExecuteAllEvents>();

			InputDeviceProfile[] profiles = new InputDeviceProfile[]
			{
				new Xbox360MacProfile(),
				new Xbox360WinProfile(),
				new OpenVRProfile(), 
			};
			s_EventQueue = new InputEventQueue();
			s_EventPool = new InputEventPool();

			foreach (var profile in profiles)
			{
				RegisterProfile(profile);
			}

			s_Devices.InitAfterProfiles();
		    NativeInputDeviceManager.onDeviceDiscovered -= RegisterDevice;
		    NativeInputDeviceManager.onDeviceConnectedDisconnected -= OnNativeDeviceConnectedDisconnected;
		    NativeInputDeviceManager.onDeviceDiscovered += RegisterDevice;
		    NativeInputDeviceManager.onDeviceConnectedDisconnected += OnNativeDeviceConnectedDisconnected;

            // We could still have device records retained through serialization, even though we lost the
            // managed device references.
            NativeInputDeviceManager.ReregisterDevices();

			// Set up event tree.
			s_EventTree = new InputConsumerNode();

			s_EventTree.children.Add(new InputConsumerCallback { processEvent = s_Devices.RemapEvent });

			rewriters = new InputConsumerNode();
			s_EventTree.children.Add(rewriters);

			s_EventTree.children.Add(s_Devices);

			consumers = new InputConsumerNode();
			s_EventTree.children.Add(consumers);

			assignedPlayers = new InputConsumerNode();
			consumers.children.Add(assignedPlayers);

			// Global consumers should be processed last.
			globalPlayers = new InputConsumerNode();
			consumers.children.Add(globalPlayers);

			simulateMouseWithTouches = true;
		}

	    internal static void RegisterDevice(NativeInputDeviceManager.NativeDeviceRecord deviceRecord)
        {
            var deviceInfo = deviceRecord.deviceInfo;
            var descriptor = JsonUtility.FromJson<NativeDeviceDescriptor>(deviceInfo.deviceDescriptor);
            var deviceString = string.Format("product:[{0}] manufacturer:[{1}] interface:[{2}] type:[{3}] version:[{4}]",
                    descriptor.product, descriptor.manufacturer, descriptor.@interface, descriptor.type, descriptor.version);

            if (Regex.IsMatch(deviceString, ".*Oculus.*Touch.*Controller.*", RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                var touchController = new OculusTouchController();
                touchController.DeriveHandednessFromDescriptor(deviceInfo.deviceDescriptor);
                RegisterDevice(touchController, deviceInfo.deviceId, deviceRecord.deviceConnected);
            }
            else if (Regex.IsMatch(
                deviceString,
                "^(?=.*product:(?=.*OpenVR.*Controller))(?=.*interface:.*\\[VR\\])(?=.*type:.*Controller.*).*$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                var openVRController = new OpenVRController();
                openVRController.DeriveHandednessFromDescriptor(deviceInfo.deviceDescriptor);
                RegisterDevice(openVRController, deviceInfo.deviceId, deviceRecord.deviceConnected);
            }
        }

	    static void RegisterDevice(InputDevice device, int nativeDeviceID, bool connected)
	    {
	        device.nativeID = nativeDeviceID;
	        s_NativeIDsToDevices[nativeDeviceID] = device;
            s_Devices.RegisterDevice(device);
	        if (connected)
            {
                s_Devices.ConnectDisconnectDevice(device, true);
	        }
        }

	    static void OnNativeDeviceConnectedDisconnected(int nativeDeviceID, bool connected)
	    {
	        var device = GetDeviceFromNativeID(nativeDeviceID);
	        if (device == null)
	            return;

            s_Devices.ConnectDisconnectDevice(device, connected);
	    }

        public delegate bool BindingListener(InputControl control);

	    public static event Action<InputDevice> onDeviceRegistered
	    {
            add { s_Devices.onDeviceRegistered += value; }
            remove { s_Devices.onDeviceRegistered -= value; }
	    }

        public static event Action<InputDevice, bool> onDeviceConnectedDisconnected
        {
            add { s_Devices.onDeviceConnectedDisconnected += value; }
            remove { s_Devices.onDeviceConnectedDisconnected -= value; }
        }

        #region Public Methods

        /// <summary>
        /// Returns the InputDevice with the given native ID.
        /// </summary>
        /// <param name="nativeDeviceID">Native device integer ID</param>
        public static InputDevice GetDeviceFromNativeID(int nativeDeviceID)
        {
            return s_NativeIDsToDevices.ContainsKey(nativeDeviceID) ? s_NativeIDsToDevices[nativeDeviceID] : null;
        }

        public static void RegisterProfile(InputDeviceProfile profile)
		{
			s_Devices.RegisterProfile(profile);
		}

		public static InputDevice LookupDevice(Type deviceType, int deviceIndex)
		{
            var list = s_Devices.LookupDevices(deviceType);
            if (list == null || deviceIndex >= list.Count)
                return null;

            return list[deviceIndex];
		}

	    public static InputDevice LookupDeviceWithTagIndex(Type deviceType, int tagIndex, bool checkConnected = false)
	    {
	        var list = s_Devices.LookupDevices(deviceType);
	        return list == null ? null : list.FirstOrDefault(
                device => device.tagIndex == tagIndex && (!checkConnected || device.connected));
	    }

		public static void QueueEvent(InputEvent inputEvent)
		{
			s_EventQueue.Queue(inputEvent);
		}

		public static bool ExecuteEvent(InputEvent inputEvent)
		{
			var wasConsumed = s_EventTree.ProcessEvent(inputEvent);
			s_EventPool.Return(inputEvent);
			return wasConsumed;
		}

		public static TEvent CreateEvent<TEvent>()
			where TEvent : InputEvent, new()
		{
			var newEvent = s_EventPool.ReuseOrCreate<TEvent>();
			newEvent.time = Time.time;
			return newEvent;
		}

		public static void ListenForBinding (BindingListener listener)
		{
			s_BindingListeners.Add(listener);
		}

		#endregion

		#region Non-Public Methods

		internal static void ExecuteEvents()
		{
			var currentTime = Time.time;
			InputEvent nextEvent;
			while (s_EventQueue.Dequeue(currentTime, out nextEvent))
			{
				ExecuteEvent(nextEvent);
			}
		}

		internal static void BeginFrame()
		{
			s_EventTree.BeginFrame();
		}

		internal static void EndFrame()
		{
			s_EventTree.EndFrame();
		}

		private static bool SendSimulatedMouseEvents(InputEvent inputEvent)
		{
			////FIXME: should take actual touchdevice in inputEvent into account
			var touchEvent = inputEvent as TouchEvent;
			if (touchEvent != null)
				Touchscreen.current.SendSimulatedPointerEvents(touchEvent, UnityEngine.Cursor.lockState == CursorLockMode.Locked);
			return false;
		}
		
		internal static void RegisterBinding(InputControl control)
		{
			for (int i = s_BindingListeners.Count - 1; i >= 0; i--)
			{
				if (s_BindingListeners[i] == null)
				{
					s_BindingListeners.RemoveAt(i);
					continue;
				}
				bool used = s_BindingListeners[i](control);
				if (used)
				{
					s_BindingListeners.RemoveAt(i);
					break;
				}
			}
		}

		#endregion

		#region Public Properties

		public static IInputConsumer eventTree
		{
			get { return s_EventTree; }
		}

		public static InputConsumerNode consumers { get; private set; }
		public static InputConsumerNode globalPlayers { get; private set; }
		public static InputConsumerNode assignedPlayers { get; private set; }
		public static InputConsumerNode rewriters { get; private set; }

		public static bool listeningForBinding
		{
			get { return s_BindingListeners.Count > 0; }
		}

		public static List<InputDevice> devices
		{
			get { return s_Devices.devices; }
		}
		
		public static TDevice GetMostRecentlyUsedDevice<TDevice>()
			where TDevice : InputDevice
		{
			return s_Devices.GetMostRecentlyUsedDevice<TDevice>();
		}

		internal static int GetNewDeviceIndex(InputDevice device)
		{
			return s_Devices.GetNewDeviceIndex(device);
		}

		public static bool simulateMouseWithTouches
		{
			get { return s_SimulateMouseWithTouches; }
			set
			{
				if (value == s_SimulateMouseWithTouches)
					return;

				if (value)
				{
					if (s_SimulateMouseWithTouchesProcess == null)
						s_SimulateMouseWithTouchesProcess = new InputConsumerCallback
						{
							processEvent = SendSimulatedMouseEvents
						};

					rewriters.children.Add(s_SimulateMouseWithTouchesProcess);
				}
				else
				{
					if (s_SimulateMouseWithTouchesProcess != null)
						rewriters.children.Remove(s_SimulateMouseWithTouchesProcess);
				}

				s_SimulateMouseWithTouches = value;
			}
		}

		#endregion

		#region Fields

		static InputDeviceManager s_Devices;
		static InputEventQueue s_EventQueue;
		static InputEventPool s_EventPool;
		static InputConsumerNode s_EventTree;
		static bool s_SimulateMouseWithTouches;
		static IInputConsumer s_SimulateMouseWithTouchesProcess;
		static List<BindingListener> s_BindingListeners = new List<BindingListener>();
        static Dictionary<int, InputDevice> s_NativeIDsToDevices = new Dictionary<int, InputDevice>();

        #endregion
    }
}
