namespace UnityEngine.InputNew
{
	public class VREvent
		: InputEvent
	{
		public Vector3 localPosition { get; set; }
		public Quaternion localRotation { get; set; }
	}
}
