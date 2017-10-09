using System;
using System.Collections.Generic;
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

        [SerializeField] // <-This breaks it
        List<NativeInputDeviceInfo> m_DeviceRecords = new List<NativeInputDeviceInfo>();

        //Dictionary<int, InputDevice> m_NativeIDsToDevices = new Dictionary<int, InputDevice>();

        void OnEnable()
        {
            // Rebuild any devices that still retain device records through serialization, even though we lost the device reference
            var retainedDeviceRecords = new List<NativeInputDeviceInfo>(m_DeviceRecords);
            m_DeviceRecords.Clear();
            //m_NativeIDsToDevices.Clear();
            foreach (var deviceRecord in retainedDeviceRecords)
            {
                CreateNativeInputDevice(deviceRecord);
            }
            NativeInputSystem.onDeviceDiscovered += CreateNativeInputDevice;
        }

        void OnDisable()
        {
            NativeInputSystem.onDeviceDiscovered -= CreateNativeInputDevice;
        }

        void CreateNativeInputDevice(NativeInputDeviceInfo deviceInfo)
        {
            var descriptor = JsonUtility.FromJson<NativeDeviceDescriptor>(deviceInfo.deviceDescriptor);
            if (descriptor.product.Contains("Oculus") && descriptor.product.Contains("Touch"))
            {
                //var touchController = new OculusTouchController();
                //if (descriptor.product.Contains("Left"))
                //{
                //    touchController.Hand = TrackedController.Handedness.Left;
                //}
                //else if (descriptor.product.Contains("Right"))
                //{
                //    touchController.Hand = TrackedController.Handedness.Right;
                //}
                //RegisterDevice(touchController, deviceInfo);
                m_DeviceRecords.Add(deviceInfo);
            }
        }

        //void RegisterDevice(InputDevice device, NativeInputDeviceInfo deviceInfo)
        //{
        //    m_DeviceRecords.Add(deviceInfo);
        //    m_NativeIDsToDevices[deviceInfo.deviceId] = device;
        //}
    }
}
