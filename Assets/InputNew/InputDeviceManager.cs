using System;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	internal class InputDeviceManager
	{
		#region Constructors

		public InputDeviceManager()
		{
			// In the prototype, just create a set of default devices. In the real thing, these would be registered
			// and configured by the platform layer according to what's really available on the system.
			var mouse = Mouse.CreateDefault();
			var keyboard = Keyboard.CreateDefault();
			var touchscreen = Touchscreen.CreateDefault();
			var gamepad = Gamepad.CreateDefault();

			RegisterDevice( mouse );
			RegisterDevice( keyboard );
			RegisterDevice( touchscreen );
			RegisterDevice( gamepad );
		}

		#endregion

		#region Public Methods

		public void RegisterDevice( InputDevice device )
		{
			AssignDeviceProfile (device);
			RegisterDeviceInternal( device.GetType(), device );
			HandleDeviceConnectDisconnect( device, true );
		}

		public void RegisterProfile( InputDeviceProfile profile )
		{
			_profiles.Add( profile );
		}

		public InputDevice GetMostRecentlyUsedDevice( Type deviceType )
		{
			for ( var i = _leastToMostRecentlyUsedDevices.Count - 1; i >= 0; -- i )
			{
				var device = _leastToMostRecentlyUsedDevices[ i ];
				if ( deviceType.IsInstanceOfType( device ) )
					return device;
			}

			return null;
		}

		public TDevice GetMostRecentlyUsedDevice< TDevice >()
			where TDevice : InputDevice
		{
			return ( TDevice ) GetMostRecentlyUsedDevice( typeof ( TDevice ) );	
		}

		public List< InputDevice > GetDevicesOfType( Type deviceType )
		{
			List< InputDevice > list;
			_devices.TryGetValue( deviceType, out list );
			return list;
		}

		public InputDevice LookupDevice( Type deviceType, int deviceIndex )
		{
			List< InputDevice > list;
			if (    !_devices.TryGetValue( deviceType, out list )
			     || deviceIndex >= list.Count )
				return null;

			return list[ deviceIndex ];
		}

		////REVIEW: an alternative to these two methods is to hook every single device into the event tree independently; may be better

		public bool ProcessEvent( InputEvent inputEvent )
		{
			// Look up device.
			var device = LookupDevice( inputEvent.deviceType, inputEvent.deviceIndex );
			if ( device == null )
				return false;

			// Ignore event on device if device is disconnected.
			if ( !device.connected )
				return false;

			// Update most-recently-used status.
			for ( var currentType = inputEvent.deviceType; currentType != typeof( InputDevice ); currentType = currentType.BaseType )
			{
				var mostRecentlyUsedDeviceOfType = GetMostRecentlyUsedDevice( currentType );
				if ( mostRecentlyUsedDeviceOfType != device )
				{
					_leastToMostRecentlyUsedDevices.Remove( device );
					_leastToMostRecentlyUsedDevices.Add( device );

					//fire event

					break;
				}
			}

			// Let device process event.
			return device.ProcessEvent( inputEvent );
		}

		public bool RemapEvent( InputEvent inputEvent )
		{
			// Look up device.
			var device = LookupDevice( inputEvent.deviceType, inputEvent.deviceIndex );
			if ( device == null )
				return false;

			// Let device remap event.
			return device.RemapEvent( inputEvent );
		}

		public void DisconnectDevice( InputDevice device )
		{
			if ( !device.connected )
				return;

			HandleDeviceConnectDisconnect( device, false );
		}

		public void ReconnectDevice( InputDevice device )
		{
			if ( device.connected )
				return;

			HandleDeviceConnectDisconnect( device, true );
		}

		#endregion

		#region Non-Public Methods

		private void AssignDeviceProfile( InputDevice device )
		{
			device.profile = FindDeviceProfile( device );
		}

		private InputDeviceProfile FindDeviceProfile( InputDevice device )
		{
			foreach ( var profile in _profiles )
			{
				if ( profile.deviceNames != null )
				{
					foreach ( var deviceName in profile.deviceNames )
						if ( string.Compare( deviceName, device.deviceName, StringComparison.InvariantCulture ) == 0 )
							return profile;
				}
			}
			return null;
		}

		private void RegisterDeviceInternal( Type deviceType, InputDevice device )
		{
			List< InputDevice > list;
			if ( !_devices.TryGetValue( deviceType, out list ) )
			{
				list = new List< InputDevice >();
				_devices[ deviceType ] = list;
			}

			list.Add( device );
			_leastToMostRecentlyUsedDevices.Add( device );

			var baseType = deviceType.BaseType;
			if ( baseType != typeof( InputDevice ) )
				RegisterDeviceInternal( baseType, device );
		}

		private void HandleDeviceConnectDisconnect( InputDevice device, bool connected )
		{
			// Sync state.
			device.connected = connected;

			// Fire event.
			var handler = deviceConnectedDisconnected;
			if ( handler != null )
				handler( device, connected );
		}

		#endregion

		#region Public Properties

		public Pointer pointer
		{
			get { return GetMostRecentlyUsedDevice< Pointer >(); }
		}

		public Mouse mouse
		{
			get { return GetMostRecentlyUsedDevice< Mouse >(); }
		}

		public Keyboard keyboard
		{
			get { return GetMostRecentlyUsedDevice< Keyboard >(); }
		}

		public Touchscreen touchscreen
		{
			get { return GetMostRecentlyUsedDevice< Touchscreen >(); }
		}

		public IEnumerable< InputDevice > devices
		{
			get
			{
				foreach ( var list in _devices.Values )
					foreach ( var device in list )
						yield return device;
			}
		}

		#endregion

		#region Public Events

		public DeviceConnectDisconnectEvent deviceConnectedDisconnected;
		
		#endregion

		#region Fields

		private readonly Dictionary< Type, List< InputDevice > > _devices = new Dictionary< Type, List< InputDevice > >();
		private readonly List< InputDevice > _leastToMostRecentlyUsedDevices = new List< InputDevice >();
		private readonly List< InputDeviceProfile > _profiles = new List< InputDeviceProfile >();

		#endregion

		#region Inner Types

		public delegate void DeviceConnectDisconnectEvent( InputDevice device, bool connected );

		#endregion
	}
}