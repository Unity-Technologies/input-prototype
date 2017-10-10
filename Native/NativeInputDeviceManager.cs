using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Input.Utilities;
using UnityEngine.InputNew;
using UnityEngineInternal.Input;

namespace UnityEngine.Experimental.Input
{
    class NativeInputDeviceManager : ScriptableSettings<NativeInputDeviceManager>
    {
        [SerializeField]
        List<NativeInputDeviceInfo> m_DeviceRecords = new List<NativeInputDeviceInfo>();

        public static event Action<NativeInputDeviceInfo> onDeviceDiscovered;

        void OnEnable()
        {
            NativeInputSystem.onDeviceDiscovered += OnDeviceDiscovered;
        }

        void OnDisable()
        {
            NativeInputSystem.onDeviceDiscovered -= OnDeviceDiscovered;
        }

        internal static void ReregisterDevices()
        {
            foreach (var deviceRecord in instance.m_DeviceRecords)
            {
                InputSystem.RegisterDevice(deviceRecord);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeInstanceOnRuntimeLoad()
        {
            // Ensure that we start listening for devices as soon as possible.
            var initializedInstance = instance;
        }

        void OnDeviceDiscovered(NativeInputDeviceInfo deviceInfo)
        {
            m_DeviceRecords.Add(deviceInfo);
            if (onDeviceDiscovered != null)
                onDeviceDiscovered(deviceInfo);
        }
    }
}
