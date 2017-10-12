namespace UnityEngine.InputNew
{
	public class OpenVRProfile : InputDeviceProfile
	{
		public const int kViveAxisCount = 10; // 5 axes in openVR, each with X and Y.
		public OpenVRProfile()
		{
			AddDeviceName("XRInputDevice");
			AddSupportedPlatform("OpenVR");
			AddSupportedPlatform("Vive");
		}

		public override void Remap(InputEvent inputEvent)
		{
		}
	}

}