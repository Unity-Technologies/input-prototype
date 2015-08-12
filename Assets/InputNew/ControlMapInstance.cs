using System;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class ControlMapInstance
		: InputControlProvider
	{
		#region Constructors

		public ControlMapInstance
			(
				  ControlMap controlMap
			    , int controlSchemeIndex
				, List< InputControlData > controls
				, List< InputState > deviceStates
			)
			: base( controls )
		{
			this.controlSchemeIndex = controlSchemeIndex;
			_deviceStates = deviceStates;
			_controlMap = controlMap;
		}

		#endregion

		#region Public Methods

		public void Activate()
		{
			var treeNode = new InputEventTree
			            {
				              name = "Map"
				            , processInput = ProcessEvent
			            };
			InputSystem.eventTree.children.Add( treeNode );
		}
		
		public override bool ProcessEvent( InputEvent inputEvent )
		{
			var consumed = false;

			////FIXME: this is BOGUS! we consume all events for a device regardless of whether they are bound by us

			// Update device state (if event actually goes to one of the devices we talk to).
			foreach ( var deviceState in _deviceStates )
			{
				////FIXME: should refer to proper type
				var device = ( InputDevice ) deviceState.controlProvider;

				// Skip state if event is not meant for device associated with it.
				var foundDevice = InputSystem.LookupDevice( inputEvent.deviceType, inputEvent.deviceIndex );
				if ( foundDevice != device )
					continue;

				// Give device a stab at converting the event into state.
				if ( device.ProcessEventIntoState( inputEvent, deviceState ) )
				{
					consumed = true;
					break;
				}
			}

			if ( !consumed )
				return false;

			////REVIEW: this probably needs to be done as a post-processing step after all events have been received
			// Synchronize the ControlMapInstance's own state.
			for ( var entryIndex = 0; entryIndex < _controlMap.entries.Count; ++ entryIndex)
			{
				var entry = _controlMap.entries[ entryIndex ];
				if ( entry.bindings == null || entry.bindings.Count == 0 )
					continue;

				var binding = entry.bindings[ controlSchemeIndex ];

				var controlValue = 0.0f;
				foreach ( var source in binding.sources )
				{
					var value = GetSourceValue( source );
					if ( Mathf.Abs( value ) > Mathf.Abs( controlValue ) )
						controlValue = value;
				}

				foreach ( var axis in binding.buttonAxisSources )
				{
					var negativeValue = GetSourceValue( axis.negative );
					var positiveValue = GetSourceValue( axis.positive );
					var value = positiveValue - negativeValue;
					if ( Mathf.Abs( value ) > Mathf.Abs( controlValue ) )
						controlValue = value;
				}

				state.SetCurrentValue( entryIndex, controlValue );
			}

			return true;
		}
		
		float GetSourceValue( InputControlDescriptor source )
		{
			var deviceState = GetDeviceStateForDeviceType( source.deviceType );
			return deviceState[ source.controlIndex ].floatValue;
		}

		#endregion

		#region Non-Public Methods

		private InputState GetDeviceStateForDeviceType( Type deviceType )
		{
			foreach ( var deviceState in _deviceStates )
				if ( deviceType.IsInstanceOfType( deviceState.controlProvider ) )
					return deviceState;
			throw new ArgumentException( "deviceType" );
		}

		#endregion

		#region Public Properties

		public int controlSchemeIndex { get; private set; }

		public InputControl this[ ControlMapEntry entry ]
		{
			get
			{
				return state[ entry.controlIndex ];
			}
		}
		
		#endregion

		#region Fields

		private readonly ControlMap _controlMap;
		private readonly List< InputState > _deviceStates;

		#endregion
	}
}
