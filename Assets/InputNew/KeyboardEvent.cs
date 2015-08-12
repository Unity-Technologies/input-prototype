namespace UnityEngine.InputNew
{
	public class KeyboardEvent
		: InputEvent
	{
		#region Public Methods
		
		#endregion

		#region Public Properties

		public KeyControl key { get; set; }
		public bool isDown { get; set; }
		public bool isRepeat { get; set; }
		public int modifiers { get; set; } 
		
		#endregion
	}
}