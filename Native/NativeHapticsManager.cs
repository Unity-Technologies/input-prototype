using System.Runtime.InteropServices;
using UnityEngine.Experimental.Input.Utilities;
using UnityEngineInternal.Input;

namespace UnityEngine.Experimental.Input
{
    /// <summary>
    /// Sends haptic output events to native devices.
    /// </summary>
    public static class NativeHapticsManager
    {
        enum HapticEventType
        {
            None = 0,
            EnqueueRumble = 1,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct HapticEvent
        {
            int m_Type;
            float m_Intensity;
            int m_Channel;

            public HapticEvent(float intensity, int channel)
            {
                m_Type = (int)HapticEventType.EnqueueRumble;
                m_Intensity = intensity;
                m_Channel = channel;
            }
        }

        static readonly FourCharacterCode k_OutputVRConstantRumble = new FourCharacterCode("0VCR");

        /// <summary>
        /// Sets haptic output to the device with the given native ID.
        /// </summary>
        /// <param name="nativeDeviceID">Native ID of the device to receive feedback.</param>
        /// <param name="intensity">Intensity of haptic feedback, ranging from 0 to 1.</param>
        public static void SendHapticOutput(int nativeDeviceID, float intensity)
        {
            var rumbleEvent = new HapticEvent(intensity, 0);
            NativeInputSystem.SendOutput(nativeDeviceID, k_OutputVRConstantRumble, rumbleEvent);
        }
    }
}
