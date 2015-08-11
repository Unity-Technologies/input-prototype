namespace UnityEngine.InputNew
{
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
}