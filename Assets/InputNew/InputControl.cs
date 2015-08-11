using System;

namespace UnityEngine.InputNew
{
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
				var currentValue = _state.GetCurrentValue( _index );
				if ( currentValue > 0.001f )
					return true;
				return false;
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
}