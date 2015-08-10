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

			// Update device state (if event actually goes to one of the devices we talk to).
			foreach ( var deviceState in _deviceStates )
				////FIXME: should refer to proper type
				if ( ( ( InputDevice ) deviceState.controlProvider ).ProcessEventIntoState( inputEvent, deviceState ) )
				{
					consumed = true;
					break;
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
					var deviceState = GetDeviceStateForDeviceType( source.deviceType );
					var value = deviceState[ source.controlIndex ].floatValue;

					if ( Mathf.Abs( value ) > Mathf.Abs( controlValue ) )
						controlValue = value;
				}

				////TODO: support button axes
				
				state.SetCurrentValue( entryIndex, controlValue );
			}

			return true;
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