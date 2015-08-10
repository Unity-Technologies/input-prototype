using System.Collections.Generic;

namespace UnityEngine.InputNew
{
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
}