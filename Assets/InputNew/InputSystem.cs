using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputNew;

//// - solve mapping of device type names from control maps to device types at runtime

namespace UnityEngine.InputNew
{
	public static class InputSystem
	{
		#region Constructors

		static InputSystem()
		{
			_devices = new InputDeviceManager();
			_eventQueue = new InputEventQueue();
			_eventPool = new InputEventPool();

			// Set up event tree.
			_eventTree = new InputEventTree { name = "Root" };

			var remap = new InputEventTree
			            {
				            name = "Remap"
				            , processInput = _devices.RemapEvent
			            };
			_eventTree.children.Add( remap );

			var state = new InputEventTree
			            {
				            name = "State"
				            , processInput = _devices.ProcessEvent
			            };
			_eventTree.children.Add( state );
		}

		#endregion

		#region Public Methods

		public static InputDevice LookupDevice( Type deviceType, int deviceIndex )
		{
			throw new NotImplementedException();
		}

		public static void QueueEvent( InputEvent inputEvent )
		{
			_eventQueue.Queue( inputEvent );
		}

		public static bool ExecuteEvent( InputEvent inputEvent )
		{
			var wasConsumed = _eventTree.ProcessEvent( inputEvent );
			_eventPool.Return( inputEvent );
			return wasConsumed;
		}

		public static TEvent CreateEvent< TEvent >()
			where TEvent : InputEvent, new()
		{
			var newEvent = _eventPool.ReuseOrCreate< TEvent >();
			newEvent.time = Time.time;
			return newEvent;
		}

		public static IEnumerable< ControlMapInstance > BindInputs( ControlMap controlMap, bool localMultiplayer = false )
		{
			for ( var i = 0; i < controlMap.schemes.Count; ++ i )
			{
				foreach ( var instance in BindInputs( controlMap, localMultiplayer, i ) )
					yield return instance;
			}
		}

		public static IEnumerable< ControlMapInstance > BindInputs( ControlMap controlMap, bool localMultiplayer, int controlSchemeIndex )
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeMapEntries = new Dictionary< Type, List< ControlMapEntry > >();
			foreach ( var entry in controlMap.entries )
			{
				if ( entry.bindings == null || entry.bindings.Count == 0 )
					continue;

				foreach ( var control in entry.bindings[ controlSchemeIndex ].sources )
				{
					List< ControlMapEntry > entries;
					if ( !perDeviceTypeMapEntries.TryGetValue( control.deviceType, out entries ) )
					{
						entries = new List< ControlMapEntry >();
						perDeviceTypeMapEntries[ control.deviceType ] = entries;
					}

					entries.Add( entry );
				}
			}

			////REVIEW: what to do about disconnected devices here? skip? include? make parameter?
 
			// Create list of controls from InputMap.
			var controls = new List< InputControlData >();
			foreach ( var entry in controlMap.entries )
			{
				var control = new InputControlData
				{
					  name = entry.controlData.name
					, controlType = entry.controlData.controlType
				};
				controls.Add( control );
			}

			if ( localMultiplayer )
			{
				// Gather available devices for each type of device.
				var deviceTypesToAvailableDevices = new Dictionary< Type, List< InputDevice > >();
				var minDeviceCountOfType = Int32.MaxValue;
				foreach ( var deviceType in perDeviceTypeMapEntries.Keys )
				{
					var availableDevicesOfType = _devices.GetDevicesOfType( deviceType );
					if ( availableDevicesOfType != null )
						deviceTypesToAvailableDevices[ deviceType ] = availableDevicesOfType;

					minDeviceCountOfType = Mathf.Min( minDeviceCountOfType, availableDevicesOfType != null ? availableDevicesOfType.Count : 0 );
				}

				// Create map instances according to available devices.
				for ( var i = 0; i < minDeviceCountOfType; ++ i )
				{
					var deviceStates = new List< InputState >();

					foreach ( var entry in perDeviceTypeMapEntries )
					{
						// Take i-th device of current type.
						var device = deviceTypesToAvailableDevices[ entry.Key ][ i ];
						var state = new InputState( device );
						deviceStates.Add( state );
					}
					
					yield return new ControlMapInstance( controlMap, controls, deviceStates );
				}
			}
			else
			{
				var deviceStates = new List< InputState >();

				// Create device states for most recently used device of given types.
				foreach ( var entry in perDeviceTypeMapEntries )
				{
					var device = _devices.GetMostRecentlyUsedDevice( entry.Key );
					if ( device == null )
						yield break; // Can't satisfy this ControlMap; no available device of given type.

					var state = new InputState( device );
					deviceStates.Add( state );
				}
				
				yield return new ControlMapInstance( controlMap, controls, deviceStates );
			}
		}

		// This is for having explicit control over what devices go into a ControlMapInstance.
		public static ControlMapInstance BindInputs( ControlMap controlMap, string controlScheme, IEnumerable< InputDevice > devices )
		{
			// Create state for every device.
			var deviceStates = new List< InputState >();
			foreach ( var device in devices )
				deviceStates.Add( new InputState( device ) );

			// Create list of controls from InputMap.
			var controls = new List< InputControlData >();
			foreach ( var entry in controlMap.entries )
			{
				var control = new InputControlData
				{
					  name = entry.controlData.name
					, controlType = entry.controlData.controlType
				};
				controls.Add( control );
			}

			// Create map instance.
			return new ControlMapInstance( controlMap, controls, deviceStates );
		}

		#endregion

		#region Non-Public Methods

		internal static void ExecuteEvents()
		{
			var currentTime = Time.time;
			InputEvent nextEvent;
			while ( _eventQueue.Dequeue( currentTime, out nextEvent ) )
			{
				ExecuteEvent( nextEvent );
			}
		}

		internal static void QueueNativeEvents( List< NativeInputEvent > nativeEvents )
		{
			////TODO

			nativeEvents.Clear();
		}

		#endregion

		#region Public Properties

		public static IInputConsumer eventTree
		{
			get { return _eventTree; }
		}

		public static Pointer pointer
		{
			get { return _devices.pointer; }
		}

		public static Keyboard keyboard
		{
			get { return _devices.keyboard; }
		}

		public static Mouse mouse
		{
			get { return _devices.mouse; }
		}

		public static Touchscreen touchscreen
		{
			get { return _devices.touchscreen; }
		}

		public static IEnumerable< InputDevice > devices
		{
			get { return _devices.devices; }
		}

		#endregion

		#region Fields

		private static InputDeviceManager _devices;
		private static InputEventQueue _eventQueue;
		private static InputEventPool _eventPool;
		private static InputEventTree _eventTree;

		#endregion
	}
}
