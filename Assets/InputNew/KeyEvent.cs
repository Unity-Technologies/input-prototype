namespace UnityEngine.InputNew
{
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
}