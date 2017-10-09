using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Experimental.Input.Utilities;
using UnityEngine.InputNew;
using UnityEngineInternal.Input;

namespace UnityEngine.Experimental.Input
{
    class NativeInputDeviceManager : ScriptableSettings<NativeInputDeviceManager>
    {
        struct NativeDeviceDescriptor
        {
            public string @interface;
            public string type;
            public string product;
            public string manufacturer;
            public string version;
            public string serial;
        }

        [SerializeField]
        List<NativeInputDeviceInfo> m_DeviceRecords = new List<NativeInputDeviceInfo>();

        Dictionary<int, InputDevice> m_NativeIDsToDevices = new Dictionary<int, InputDevice>();

        void OnEnable()
        {
            // Reregister any devices that still retain device records through serialization,
            // even though we lost the managed device reference.
            // ReregisterDevices could end up calling the InputSystem static constructor. This is problematic if it
            // happens on the serialization thread since the InputSystem tries to instantiate a game object and add
            // components to it. For this reason, we have to delay the call to ReregisterDevices in the editor.
#if UNITY_EDITOR
            EditorApplication.delayCall += ReregisterDevices;
#else
            ReregisterDevices();
#endif
            NativeInputSystem.onDeviceDiscovered += RegisterDeviceFromNativeInfo;
        }

        void OnDisable()
        {
            NativeInputSystem.onDeviceDiscovered -= RegisterDeviceFromNativeInfo;
        }

        /// <summary>
        /// Returns the InputDevice with the given native ID.
        /// </summary>
        /// <param name="nativeDeviceID">Native device integer ID</param>
        public static InputDevice GetDeviceFromNativeID(int nativeDeviceID)
        {
            return instance.m_NativeIDsToDevices.ContainsKey(nativeDeviceID) ?
                instance.m_NativeIDsToDevices[nativeDeviceID] : null;
        }

        [RuntimeInitializeOnLoadMethod]
        static void InitializeInstanceOnRuntimeLoad()
        {
            // Ensure that we start listening for devices as soon as possible.
            var initializedInstance = instance;
        }

        static void ReregisterDevices()
        {
            var retainedDeviceRecords = new List<NativeInputDeviceInfo>(instance.m_DeviceRecords);
            instance.m_DeviceRecords.Clear();
            instance.m_NativeIDsToDevices.Clear();
            foreach (var deviceRecord in retainedDeviceRecords)
            {
                instance.RegisterDeviceFromNativeInfo(deviceRecord);
            }
        }

        void RegisterDeviceFromNativeInfo(NativeInputDeviceInfo deviceInfo)
        {
            var descriptor = JsonUtility.FromJson<NativeDeviceDescriptor>(deviceInfo.deviceDescriptor);
            if (descriptor.product.Contains("Oculus") && descriptor.product.Contains("Touch"))
            {
                var touchController = new OculusTouchController();
                if (descriptor.product.Contains("Left"))
                {
                    touchController.Hand = TrackedController.Handedness.Left;
                }
                else if (descriptor.product.Contains("Right"))
                {
                    touchController.Hand = TrackedController.Handedness.Right;
                }
                RegisterDevice(touchController, deviceInfo);
            }
        }

        void RegisterDevice(InputDevice device, NativeInputDeviceInfo deviceInfo)
        {
            device.nativeID = deviceInfo.deviceId;
            m_DeviceRecords.Add(deviceInfo);
            m_NativeIDsToDevices[deviceInfo.deviceId] = device;
            InputSystem.RegisterDevice(device);
        }
    }
}
