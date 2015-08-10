namespace UnityEngine.InputNew
{
	public class GenericControlEvent
		: InputEvent
	{
		#region Public Properties

		public int controlIndex { get; set; }
		public float value { get; set; }

		#endregion
	}
}