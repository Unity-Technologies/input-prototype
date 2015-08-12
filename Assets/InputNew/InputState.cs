using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class InputState
	{
		#region Constructors

		public InputState( InputControlProvider controlProvider ) : this( controlProvider, null ) {}

		public InputState( InputControlProvider controlProvider, List< int > usedControlIndices )
		{
			_controlProvider = controlProvider;

			var controlCount = controlProvider.controls.Count;
			_currentStates = new float[ controlCount ];
			_previousStates = new float[ controlCount ];

			_enabled = new bool[ controlCount ];
			if (usedControlIndices == null)
			{
				SetAllControlsEnabled( true );
			}
			else
			{
				for (int i = 0; i < usedControlIndices.Count; i++)
					_enabled[usedControlIndices[i]] = true;
			}
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

		public bool SetCurrentValue( int index, bool value )
		{
			return SetCurrentValue( index, value ? 1.0f : 0.0f );
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
				var controls = controlProvider.controls;
				for ( var i = 0; i < controls.Count; ++ i )
					if ( controls[ i ].name == controlName )
						return this[ i ];

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
}