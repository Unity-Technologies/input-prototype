namespace UnityEngine.InputNew
{
	public class PointerEvent
		: InputEvent
	{
		#region Public Properties

		public Vector3 position { get; set; }
		public float pressure { get; set; }
		public float tilt { get; set; }
		public float rotation { get; set; }
		public int displayIndex { get; set; }

		#endregion
	}
}