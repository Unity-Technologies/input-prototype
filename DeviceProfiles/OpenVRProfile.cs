using UnityEngine.InputNew;
using System.Collections;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
	public class OpenVRProfile : InputDeviceProfile
	{
		public OpenVRProfile()
		{
			AddDeviceName("VRInputDevice");
			AddSupportedPlatform("OpenVR");
			AddSupportedPlatform("Vive");
		}

		public override void Remap(InputEvent inputEvent)
		{
			Debug.Log("Remapping: " + inputEvent);

			var axisEvent = inputEvent as GenericControlEvent;
			if (axisEvent != null)
			{
				if (axisEvent.controlIndex == 2)
					axisEvent.controlIndex = (int)VRInputDevice.VRControl.Trigger1;
			}

		//throw new System.NotImplementedException();
		}
	}
}