using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	// ------------------------------------------------------------------------
	//	Events.
	// ------------------------------------------------------------------------

	public abstract class InputEvent
	{
		#region Public Properties

		public float time { get; set; }
		public Type deviceType { get; set; }
		public int deviceIndex { get; set; }

		#endregion
	}

	// Approach A
	public class PointerEvent
		: InputEvent
	{
		#region Public Properties

		public Vector3 position { get; set; }
		public float pressure { get; set; }
		public float tilt { get; set; }
		public float rotation { get; set; }
		public int displayIndex { get; set; }

		#endregion
	}
	public class PointerMoveEvent
		: PointerEvent
	{
		#region Public Properties

		public Vector3 delta { get; set; }

		#endregion
	}
	public class PointerClickEvent
		: PointerEvent
	{
		#region Public Properties

		public int clickCount { get; set; }

		#endregion
	}

	public class KeyEvent
		: InputEvent
	{
		#region Public Properties

		public KeyControl rawKey { get; set; }
		public KeyControl localizedKey { get; set; }
		public bool isPress { get; private set; }
		public bool isRelease { get; private set; }
		public bool isRepeat { get; private set; }

		#endregion
	}

	public class TextEvent
		: InputEvent
	{
		public char text { get; set; }
	}

	public class GenericButtonEvent
		: InputEvent
	{
		#region Public Properties

		public int buttonIndex { get; set; }

		#endregion
	}

	public class GenericAxisEvent
		: InputEvent
	{
	}

	public class GenericControlEvent
		: InputEvent
	{
		#region Public Properties

		public int controlIndex { get; set; }
		public float value { get; set; }

		#endregion
	}

	public class TouchEvent
		: InputEvent
	{
	}

	public class GestureEvent
		: InputEvent
	{
	}

	internal class InputEventQueue
	{
		#region Public Methods

		public void Queue( InputEvent inputEvent )
		{
			_list.Add( inputEvent.time, inputEvent );
		}

		public bool Dequeue( float targetTime, out InputEvent inputEvent )
		{
			if ( _list.Count == 0 )
			{
				inputEvent = null;
				return false;
			}

			var nextEvent = _list.Values[ 0 ];
			if ( nextEvent.time > targetTime )
			{
				inputEvent = null;
				return false;
			}

			_list.RemoveAt( 0 );
			inputEvent = nextEvent;
			return true;
		}

		#endregion

		#region Fields

		private SortedList< float, InputEvent > _list = new SortedList< float, InputEvent >();

		#endregion
	}

	internal class InputEventPool
	{
		#region Public Methods

		public TEvent ReuseOrCreate< TEvent >()
			where TEvent : InputEvent, new()
		{
			////TODO
			return new TEvent();
		}

		public void Return( InputEvent inputEvent )
		{
			////TODO
		}

		#endregion
	}

	////REVIEW: i don't like this whole thing

	public delegate bool ProcessInputDelegate( InputEvent inputEvent );

	public interface IInputConsumer
	{
		string name { get; }

		IList< IInputConsumer > children { get; }

		ProcessInputDelegate processInput { get; set; }
	}

	internal class InputEventTree
		: IInputConsumer
	{
		#region Public Methods

		public bool ProcessEvent( InputEvent inputEvent )
		{
			return ProcessEventRecursive( this, inputEvent );
		}

		#endregion

		#region Non-Public Methods

		protected static bool ProcessEventRecursive( IInputConsumer consumer, InputEvent inputEvent )
		{
			var callback = consumer.processInput;
			if ( callback != null )
			{
				if ( callback( inputEvent ) )
					return true;
			}

			foreach ( var child in consumer.children )
				if ( ProcessEventRecursive( child, inputEvent ) )
					return true;

			return false;
		}

		#endregion

		#region Public Properties

		public string name { get; set; }

		public IList< IInputConsumer > children
		{
			get { return _children; }
		}

		public ProcessInputDelegate processInput { get; set; }

		#endregion

		#region Fields

		private List< IInputConsumer > _children = new List< IInputConsumer >();

		#endregion
	}

	// ------------------------------------------------------------------------
	//	Native Events.
	// ------------------------------------------------------------------------
	
	internal enum NativeEventType
	{
		GenericButton,
		GenericAxis,
		KeyDown,
		KeyUp,
		TextInput,
		PointerMove,
	}

	internal struct NativeInputEvent
	{
		public NativeEventType eventType;
		public int deviceIndex;
		public float time;
	}

	// ------------------------------------------------------------------------
	//	Controls.
	// ------------------------------------------------------------------------
	
	public enum InputControlType
	{
		Button,
		RelativeAxis,
		AbsoluteAxis,
		Vector2,
		Vector3,
		Vector4,
	}
	
	public struct InputControl
	{
		#region Constructors

		internal InputControl( int index, InputState state )
		{
			_index = index;
			_state = state;
		}

		#endregion

		#region Public Properties

		public int index
		{
			get { return _index; }
		}

		public bool boolValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float floatValue
		{
			get { return _state.GetCurrentValue( _index ); }
		}

		public Vector3 vector3Value
		{
			get
			{
				var controlData = _state.controlProvider.controls[ _index ];
				////TODO: typecheck control type; convert if necessary
				return new Vector3(
					  _state.GetCurrentValue( controlData.componentControlIndices[ 0 ] )
					, _state.GetCurrentValue( controlData.componentControlIndices[ 1 ] )
					, _state.GetCurrentValue( controlData.componentControlIndices[ 2 ] )
				);
			}
		}

		public bool isEnabled
		{
			get { return _state.IsControlEnabled( _index ); }
		}

		#endregion

		#region Fields

		private int _index;
		private InputState _state;

		#endregion
	}

	[ Serializable ]
	public struct InputControlDescriptor
	{
		public Type deviceType;
		public int controlIndex;
	}

	public struct InputControlData
	{
		public string name;
		public InputControlType controlType;
		public int controlIndex;
		public int[] componentControlIndices;
	}

	/// <summary>
	/// An object that exposes a specific configuration of input controls.
	/// </summary>
	public interface IInputControlProvider
	{
		////REVIEW: this should be readonly but not sure ReadOnlyCollection from .NET 2 is good enough
		IList< InputControlData > controls { get; }
	}

	public class InputControlProvider
		: IInputControlProvider
	{
		#region Constructors

		public InputControlProvider( List< InputControlData > controls )
		{
			_controls = controls;
			_state = new InputState( this );
		}

		#endregion

		#region Public Methods

		public virtual bool ProcessEvent( InputEvent inputEvent )
		{
			return false;
		}

		#endregion

		#region Public Properties

		public InputState state 
		{
			get { return _state; }
		}

		////REVIEW: this view should be immutable
		public IList< InputControlData > controls
		{
			get { return _controls; }
		}

		#endregion

		#region Fields

		private InputState _state;
		private List< InputControlData > _controls;

		#endregion
	}

	// ------------------------------------------------------------------------
	//	States.
	// ------------------------------------------------------------------------
	
	public class InputState
	{
		#region Constructors

		public InputState( InputControlProvider controlProvider )
		{
			_controlProvider = controlProvider;

			var controlCount = controlProvider.controls.Count;
			_currentStates = new float[ controlCount ];
			_previousStates = new float[ controlCount ];

			_enabled = new bool[ controlCount ];
			SetAllControlsEnabled( true );
		}

		#endregion

		#region Public Methods

		public float GetCurrentValue( int index )
		{
			return _currentStates[ index ];
		}

		public float GetPreviousValue( int index )
		{
			return _previousStates[ index ];
		}

		public bool SetCurrentValue( int index, float value )
		{
			if ( !IsControlEnabled( index ) )
				return false;

			////FIXME: need to copy current into previous whenever update starts; this thing here doesn't work

			_previousStates[ index ] = _currentStates[ index ];
			_currentStates[ index ] = value;

			return true;
		}

		public bool IsControlEnabled( int index )
		{
			return _enabled[ index ];
		}

		public void SetControlEnabled( int index, bool enabled )
		{
			_enabled[ index ] = enabled;
		}

		public void SetAllControlsEnabled( bool enabled )
		{
			for ( var i = 0; i < _enabled.Length; ++ i )
				_enabled[ i ] = enabled;
		}

		#endregion

		#region Public Properties

		public InputControlProvider controlProvider
		{
			get { return _controlProvider; }
		}

		public InputControl this[ int index ]
		{
			get { return new InputControl( index, this ); }
		}

		public InputControl this[ string controlName ]
		{
			get
			{
				foreach ( var control in controlProvider.controls )
					if ( control.name == controlName )
						return this[ control.controlIndex ];

				throw new KeyNotFoundException( controlName );
			}
		}

		#endregion

		#region Fields

		private float[] _currentStates;
		private float[] _previousStates;
		private bool[] _enabled;
		private InputControlProvider _controlProvider;

		#endregion
	}

	// ------------------------------------------------------------------------
	//	Devices.
	// ------------------------------------------------------------------------

	public abstract class InputDevice
		: InputControlProvider
	{
		#region Constructors

		protected InputDevice( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public virtual bool RemapEvent( InputEvent inputEvent )
		{
			////TODO: implement remapping
			return false;
		}

		#endregion
	}

	public enum KeyControl
	{
		A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
		Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,

		LeftShift,
		RightShift,
		LeftControl, LeftCommand = LeftControl,
		RightControl, RightCommand = RightControl,
		LeftAlt,
		RightAlt, AltGr = RightAlt,
		LeftMeta,
		RightMeta,
		ContextMenu,

		Space,
		Backspace,
		Tab,
		Return,
	}

	public class Keyboard
		: InputDevice
	{
		#region Constructors

		public Keyboard( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public static Keyboard CreateDefault()
		{
			var controls = new List< InputControlData >();
			return new Keyboard( controls );
		}

		#endregion
	}

	public enum GamepadControl
	{
		LeftThumbstick, // Compound control (vector2)
		LeftThumbstickX,
		LeftThumbstickY,

		RightThumbstick, // Compound control (vector2)
		RightThumbstickX,
		RightThumbstickY,

		Dpad, // Compound control (vector2)
		DpadUp,
		DpadDown,
		DpadLeft,
		DpadRight,

		LeftTrigger,
		RightTrigger,

		LeftShoulder,
		RightShoulder,

		ButtonA,
		ButtonB,
		ButtonX,
		ButtonY,

		Start,
		Back,

		// -- Optional:

		DpadPress,
		LeftThumbstickPress,
		RightThumbstickPress,
	}

	public class Gamepad
		: InputDevice
	{
		#region Constructors

		public Gamepad( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion
	}

	public enum PointerControl
	{
		Position,
		PositionX,
		PositionY,
		PositionZ,

		Pressure,
		Tilt,
		Rotation,

		LeftButton,
		RightButton,
		MiddleButton,
		
		ScrollWheel,
		ScrollWheelX,
		ScrollWheelY,
		////REVIEW: have Z for ScrollWheel, too?

		ForwardButton,
		BackButton,
	}

	/// <summary>
	/// A device that can point at and click on things.
	/// </summary>
	public abstract class Pointer
		: InputDevice
	{
		#region Constructors

		protected Pointer( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public override bool ProcessEvent( InputEvent inputEvent )
		{
			if ( base.ProcessEvent( inputEvent ) )
				return true;

			var consumed = false;

			var moveEvent = inputEvent as PointerMoveEvent;
			if ( moveEvent != null )
			{
				consumed |= state.SetCurrentValue( ( int ) PointerControl.PositionX, moveEvent.position.x );
				consumed |= state.SetCurrentValue( ( int ) PointerControl.PositionY, moveEvent.position.y );
				consumed |= state.SetCurrentValue( ( int ) PointerControl.PositionZ, moveEvent.position.z );

				return consumed;
			}

			return false;
		}

		#endregion

		#region Non-Public Methods

		protected static List< InputControlData > CreateDefaultControls()
		{
			var controls = new List< InputControlData >();

			// Compounds.
			controls.Add( new InputControlData {
				  name = "Position"
				, controlIndex = ( int ) PointerControl.Position
				, controlType = InputControlType.Vector3
				, componentControlIndices = new int[ 3 ] { ( int ) PointerControl.PositionX, ( int ) PointerControl.PositionY, ( int ) PointerControl.PositionZ }
			} );

			// Primitives.
			controls.Add( new InputControlData { name = "PositionX", controlIndex = ( int ) PointerControl.PositionX, controlType = InputControlType.RelativeAxis } );
			controls.Add( new InputControlData { name = "PositionY", controlIndex = ( int ) PointerControl.PositionY, controlType = InputControlType.RelativeAxis } );
			controls.Add( new InputControlData { name = "PositionZ", controlIndex = ( int ) PointerControl.PositionZ, controlType = InputControlType.RelativeAxis } );

			return controls;
		}

		#endregion

		#region Public Properties

		public Vector3 position
		{
			get { return state[ ( int ) PointerControl.Position ].vector3Value; }
		}

		public float pressure
		{
			get { return state[ ( int ) PointerControl.Pressure ].floatValue; }
		}

		#endregion
	}

	public class Mouse
		: Pointer
	{
		#region Constructors

		public Mouse( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public static Mouse CreateDefault()
		{
			var controls = CreateDefaultControls();
			return new Mouse( controls );
		}

		#endregion
	}

	public struct Touch
	{
	}

	public class Touchscreen
		: Pointer
	{
		#region Constructors

		public Touchscreen( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion

		#region Public Methods

		public static Touchscreen CreateDefault()
		{
			var controls = CreateDefaultControls();
			return new Touchscreen( controls );
		}

		#endregion
	}

	public abstract class Sensor
		: InputDevice
	{
		#region Constructors

		protected Sensor( List< InputControlData > controls )
			: base( controls )
		{
		}

		#endregion
	}

	internal class InputDeviceManager
	{
		#region Constructors

		public InputDeviceManager()
		{
			RegisterDevice( typeof( Mouse ), _mouse );
			RegisterDevice( typeof( Keyboard ), _keyboard );
			RegisterDevice( typeof( Touchscreen ), _touchscreen );

			////REVIEW: probably shouldn't be using a concrete device but rather a synthetic one that is fed from all pointer-like devices available
			///   the synthetic one should be fed from the other pointer devices using a node in the event tree
			_pointer = _mouse;
			RegisterDevice( typeof( Pointer ), _pointer );
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
			get { return _pointer; }
		}

		public Mouse mouse
		{
			get { return _mouse; }
		}

		public Keyboard keyboard
		{
			get { return _keyboard; }
		}

		public Touchscreen touchscreen
		{
			get { return _touchscreen; }
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
		private Pointer _pointer;
		private Mouse _mouse = Mouse.CreateDefault();
		private Keyboard _keyboard = Keyboard.CreateDefault();
		private Touchscreen _touchscreen = Touchscreen.CreateDefault();

		#endregion
	}

	// ------------------------------------------------------------------------
	//	Bindings.
	// ------------------------------------------------------------------------
	
	////NOTE: this needs to be proper asset stuff; can't be done in script code only
	
	[ Serializable ]
	public class InputBinding
	{
		public string name;
		public List< InputControlDescriptor > controls;
	}
	
	public class InputMap
		: ScriptableObject
	{
		public List< InputBinding > bindings;
	}

	internal class InputMapInstance
		: InputControlProvider
	{
		#region Constructors

		public InputMapInstance
			(
			 	  InputMap inputMap
				, List< InputControlData > controls
				, List< InputState > deviceStates
			)
			: base( controls )
		{
			_deviceStates = deviceStates;
		}

		#endregion

		#region Public Methods

		public override bool ProcessEvent( InputEvent inputEvent )
		{
			////TODO
			return false;
		}

		#endregion

		#region Fields

		private InputMap _inputMap;
		private List< InputState > _deviceStates;

		#endregion
	}

	// ------------------------------------------------------------------------
	//	System.
	// ------------------------------------------------------------------------

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

		public static InputControlProvider BindInputs( InputMap inputMap )
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeBindings = new Dictionary< Type, List< InputBinding > >();
			foreach ( var binding in inputMap.bindings )
			{
				foreach ( var control in binding.controls )
				{
					List< InputBinding > bindings;
					if ( !perDeviceTypeBindings.TryGetValue( control.deviceType, out bindings ) )
					{
						bindings = new List< InputBinding >();
						perDeviceTypeBindings[ control.deviceType ] = bindings;
					}

					bindings.Add( binding );
				}
			}

			// Map the device types to actual devices we have available and create
			// InputStates for each of them.
			var deviceStates = new List< InputState >();
			foreach ( var deviceType in perDeviceTypeBindings.Keys )
			{
				var device = _devices.LookupDevice( deviceType, 0 ); ////TODO: needs to be more flexible
				if ( device != null )
					deviceStates.Add( new InputState( device ) );
			}

			// Create list of controls from InputMap.
			var controls = new List< InputControlData >();
			foreach ( var binding in inputMap.bindings )
			{
				var control = new InputControlData
				{
					  name = binding.name
					, controlType = InputControlType.Button ////TODO
					, controlIndex = controls.Count
				};
				controls.Add( control );
			}

			return new InputMapInstance( inputMap, controls, deviceStates );
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

		#endregion

		#region Fields

		private static InputDeviceManager _devices;
		private static InputEventQueue _eventQueue;
		private static InputEventPool _eventPool;
		private static InputEventTree _eventTree;

		#endregion
	}
}

