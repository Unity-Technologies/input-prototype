using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	class InputDeviceManager
	{
		#region Inner Types

		public delegate void DeviceConnectDisconnectEvent(InputDevice device, bool connected);

		#endregion

		#region Public Events

		public DeviceConnectDisconnectEvent deviceConnectedDisconnected;

		#endregion

		#region Constructors

		public InputDeviceManager()
		{
		}
		
		public void InitAfterProfiles()
		{
			// In the prototype, just create a set of default devices. In the real thing, these would be registered
			// and configured by the platform layer according to what's really available on the system.
			var mouseDevice = new Mouse();
			var keyboardDevice = new Keyboard();
			var touchscreenDevice = new Touchscreen();
			var gamepadDevice1 = new Gamepad();
			var gamepadDevice2 = new Gamepad();
			var virtualJoystickDevice = new VirtualJoystick();

			RegisterDevice(touchscreenDevice); // Register before mouse; we don't have code yet to handle MRU correctly for ControlMaps
			RegisterDevice(mouseDevice);
			RegisterDevice(keyboardDevice);
			RegisterDevice(gamepadDevice1);
			RegisterDevice(gamepadDevice2);
			RegisterDevice(virtualJoystickDevice);
		}

		#endregion

		#region Public Methods

		public void RegisterDevice(InputDevice device)
		{
			AssignDeviceProfile(device);
			RegisterDeviceInternal(device.GetType(), device);
			HandleDeviceConnectDisconnect(device, true);
		}

		public void RegisterProfile(InputDeviceProfile profile)
		{
			m_Profiles.Add(profile);
		}

		public InputDevice GetMostRecentlyUsedDevice(Type deviceType)
		{
			for (var i = m_LeastToMostRecentlyUsedDevices.Count - 1; i >= 0; -- i)
			{
				var device = m_LeastToMostRecentlyUsedDevices[i];
				if (deviceType.IsInstanceOfType(device))
					return device;
			}

			return null;
		}

		public TDevice GetMostRecentlyUsedDevice<TDevice>()
			where TDevice : InputDevice
		{
			return (TDevice)GetMostRecentlyUsedDevice(typeof(TDevice));
		}

		public List<InputDevice> GetDevicesOfType(Type deviceType)
		{
			List<InputDevice> list;
			m_Devices.TryGetValue(deviceType, out list);
			return list;
		}

		public InputDevice LookupDevice(Type deviceType, int deviceIndex)
		{
			List<InputDevice> list;
			if (!m_Devices.TryGetValue(deviceType, out list)
				|| deviceIndex >= list.Count)
				return null;

			return list[deviceIndex];
		}

		////REVIEW: an alternative to these two methods is to hook every single device into the event tree independently; may be better

		public bool ProcessEvent(InputEvent inputEvent)
		{
			// Look up device.
			var device = inputEvent.device;

			// Ignore event on device if device is absent or disconnected.
			if (device == null || !device.connected)
				return false;

			// Update most-recently-used status.
			m_LeastToMostRecentlyUsedDevices.Remove(device);
			m_LeastToMostRecentlyUsedDevices.Add(device);

			//fire event

			// Let device process event.
			return device.ProcessEvent(inputEvent);
		}

		public bool RemapEvent(InputEvent inputEvent)
		{
			// Look up device.
			var device = inputEvent.device;
			if (device == null)
				return false;

			// Let device remap event.
			return device.RemapEvent(inputEvent);
		}

		public void DisconnectDevice(InputDevice device)
		{
			if (!device.connected)
				return;

			HandleDeviceConnectDisconnect(device, false);
		}

		public void ReconnectDevice(InputDevice device)
		{
			if (device.connected)
				return;

			HandleDeviceConnectDisconnect(device, true);
		}

		#endregion

		#region Non-Public Methods

		void AssignDeviceProfile(InputDevice device)
		{
			device.profile = FindDeviceProfile(device);
		}

		InputDeviceProfile FindDeviceProfile(InputDevice device)
		{
			foreach (var profile in m_Profiles)
			{
				if (profile.deviceNames != null)
				{
					foreach (var deviceName in profile.deviceNames)
					{
						if (string.Compare(deviceName, device.deviceName, StringComparison.InvariantCulture) == 0)
							return profile;
					}
				}
			}
			return null;
		}

		public bool IsProfileSupportedOnThisPlatform (InputDeviceProfile profile)
		{
			if (profile.supportedPlatforms == null || profile.supportedPlatforms.Length == 0)
				return true;
			
			foreach (var platform in profile.supportedPlatforms)
			{
				if (m_Platform.Contains(platform.ToUpper()))
					return true;
			}
			
			return false;
		}

		void RegisterDeviceInternal(Type deviceType, InputDevice device)
		{
			List<InputDevice> list;
			if (!m_Devices.TryGetValue(deviceType, out list))
			{
				list = new List<InputDevice>();
				m_Devices[deviceType] = list;
			}

			list.Add(device);
			m_LeastToMostRecentlyUsedDevices.Remove(device);
			m_LeastToMostRecentlyUsedDevices.Add(device);

			var baseType = deviceType.BaseType;
			if (baseType != typeof(InputDevice))
				RegisterDeviceInternal(baseType, device);
		}

		void HandleDeviceConnectDisconnect(InputDevice device, bool connected)
		{
			// Sync state.
			device.connected = connected;

			// Fire event.
			var handler = deviceConnectedDisconnected;
			if (handler != null)
				handler(device, connected);
		}

		internal void BeginNewFrameEvent ()
		{
			foreach (var device in devices)
				device.state.BeginNewFrame ();
		}

		#endregion

		#region Public Properties

		public IEnumerable<InputDevice> devices
		{
			get
			{
				foreach (var list in m_Devices.Values)
				{
					foreach (var device in list)
					{
						yield return device;
					}
				}
			}
		}
		
		public List<InputDevice> leastToMostRecentlyUsedDevices
		{
			get { return m_LeastToMostRecentlyUsedDevices; }
		}

		#endregion

		#region Fields

		readonly Dictionary<Type, List<InputDevice>> m_Devices = new Dictionary<Type, List<InputDevice>>();
		readonly List<InputDevice> m_LeastToMostRecentlyUsedDevices = new List<InputDevice>();
		readonly List<InputDeviceProfile> m_Profiles = new List<InputDeviceProfile>();
		readonly string m_Platform = (SystemInfo.operatingSystem + " " + SystemInfo.deviceModel).ToUpper();

		#endregion
	}
}
