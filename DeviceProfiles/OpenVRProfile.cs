using UnityEngine.InputNew;
using System.Collections;
using Assets.Utilities;
using Valve.VR;

namespace UnityEngine.InputNew
{
	public class OpenVRProfile : InputDeviceProfile
	{
		public const int k_viveAxisCount = 10; // 5 axes in openVR, each with X and Y.
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
                    // Axes
                    case 0: // Axis 0.X / trackpad X
                        controlEvent.controlIndex = (int) VRInputDevice.VRControl.LeftStickX;
                        break;
                    case 1: // Axis 0.Y / trackpad Y
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.LeftStickY;
                        break;
                    case 2: // Trigger (Axis 1.X)
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.Trigger1;
                        break;
                    // Buttons
                    case k_viveAxisCount + (int)EVRButtonId.k_EButton_SteamVR_Trigger:
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.Trigger1;
                        break;
                    case k_viveAxisCount + (int)EVRButtonId.k_EButton_Grip:
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.Trigger2;
                        break;
                    case k_viveAxisCount + (int)EVRButtonId.k_EButton_SteamVR_Touchpad:
                        controlEvent.controlIndex = (int) VRInputDevice.VRControl.Action1;
                        break;
                    case k_viveAxisCount + (int)EVRButtonId.k_EButton_ApplicationMenu:
                        controlEvent.controlIndex = (int)VRInputDevice.VRControl.Action2;
                        break;
                }
            }
		}
	}

}