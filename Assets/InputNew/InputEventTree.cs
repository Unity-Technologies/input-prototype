using System.Collections.Generic;

namespace UnityEngine.InputNew
{
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
}