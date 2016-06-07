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
                // Debug.Log("Remapping: " + inputEvent + " Control Index: " + controlEvent.controlIndex);

                switch (controlEvent.controlIndex) {
                    case ViveInputToEvents.axisCount + (int)EVRButtonId.k_EButton_SteamVR_Trigger:
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.Trigger1;
                        break;
                    case ViveInputToEvents.axisCount + (int)EVRButtonId.k_EButton_Grip:
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.Trigger2;
                        break;
                    case ViveInputToEvents.axisCount + (int)EVRButtonId.k_EButton_SteamVR_Touchpad:
                        controlEvent.controlIndex = (int) VRInputDevice.VRControl.Action1;
                        break;
                    case ViveInputToEvents.axisCount + (int)EVRButtonId.k_EButton_ApplicationMenu:
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.Action2;
                        break;
                }
            }
		}
	}

}