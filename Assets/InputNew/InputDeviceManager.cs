using System;
using System.Collections.Generic;
using System.Linq;

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

			RegisterDevice( typeof( Mouse ), mouse );
			RegisterDevice( typeof( Keyboard ), keyboard );
			RegisterDevice( typeof( Touchscreen ), touchscreen );
		}

		#endregion

		#region Public Methods

		public void RegisterDevice( Type deviceType, InputDevice device )
		{
			////TODO: typecheck device

			List< InputDevice > list;
			if ( !_devices.TryGetValue( deviceType, out list ) )
			{
				list = new List< InputDevice >();
				_devices[ deviceType ] = list;
			}

			list.Add( device );

			var baseType = deviceType.BaseType;
			if ( baseType != typeof( InputDevice ) )
				RegisterDevice( baseType, device );
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

		#region Fields

		private Dictionary< Type, List< InputDevice > > _devices = new Dictionary< Type, List< InputDevice > >();
		//private Dictionary< Type, InputDevice > _mostRecentlyUsedDevice = new Dictionary< Type, InputDevice >(); 
		private List< InputDevice > _leastToMostRecentlyUsedDevices = new List< InputDevice >();

		#endregion
	}
}