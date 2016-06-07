using UnityEngine.InputNew;
using System.Collections;
using Assets.Utilities;
using Valve.VR;

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

			var controlEvent = inputEvent as GenericControlEvent;

            if (controlEvent != null)
			{
                Debug.Log("Remapping: " + inputEvent + " Control Index: " + controlEvent.controlIndex);

                if (controlEvent.controlIndex == (int)EVRButtonId.k_EButton_SteamVR_Trigger)
					controlEvent.controlIndex = (int)VRInputDevice.VRControl.Trigger1;
			}

		//throw new System.NotImplementedException();
		}
	}
}