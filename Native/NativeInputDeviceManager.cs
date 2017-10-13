using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Input.Utilities;
using UnityEngine.InputNew;
using UnityEngineInternal.Input;

namespace UnityEngine.Experimental.Input
{
    class NativeInputDeviceManager : ScriptableSettings<NativeInputDeviceManager>
    {
        [Serializable]
        public class NativeDeviceRecord
        {
            public NativeInputDeviceInfo deviceInfo;
            public bool deviceConnected;
        }

        [SerializeField]
        List<NativeDeviceRecord> m_DeviceRecords = new List<NativeDeviceRecord>();

        Dictionary<int, NativeDeviceRecord> m_DeviceRecordsByID = new Dictionary<int, NativeDeviceRecord>();

        public static event Action<NativeDeviceRecord> onDeviceDiscovered;
        public static event Action<int, bool> onDeviceConnectedDisconnected;

        void OnEnable()
        {
            m_DeviceRecordsByID.Clear();
            foreach (var record in m_DeviceRecords)
            {
                m_DeviceRecordsByID[record.deviceInfo.deviceId] = record;
            }
            NativeInputSystem.onDeviceDiscovered += OnDeviceDiscovered;
            NativeInputSystem.onEvents += OnReceiveEvents;
        }

        void OnDisable()
        {
            NativeInputSystem.onDeviceDiscovered -= OnDeviceDiscovered;
            NativeInputSystem.onEvents -= OnReceiveEvents;
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
            var deviceRecord = new NativeDeviceRecord { deviceInfo = deviceInfo, deviceConnected = true };
            m_DeviceRecords.Add(deviceRecord);
            m_DeviceRecordsByID[deviceInfo.deviceId] = deviceRecord;
            if (onDeviceDiscovered != null)
                onDeviceDiscovered(deviceRecord);
        }

        void OnReceiveEvents(int eventCount, IntPtr eventData)
        {
            // This could happen in NativeInputEventManager and connect/disconnect managed devices directly,
            // but that would require a significant refactor of the InputSystem.
            var currentDataPtr = eventData;
            for (var i = 0; i < eventCount; i++)
            {
                unsafe
                {
                    var eventPtr = (NativeInputEvent*)currentDataPtr;
                    var deviceID = eventPtr->deviceId;

                    if (m_DeviceRecordsByID.ContainsKey(deviceID))
                    {
                        var deviceRecord = m_DeviceRecordsByID[deviceID];
                        switch (eventPtr->type)
                        {
                            case NativeInputEventType.DeviceConnected:
                                deviceRecord.deviceConnected = true;
                                if (onDeviceConnectedDisconnected != null)
                                    onDeviceConnectedDisconnected(deviceID, true);
                                break;
                            case NativeInputEventType.DeviceDisconnected:
                                deviceRecord.deviceConnected = false;
                                if (onDeviceConnectedDisconnected != null)
                                    onDeviceConnectedDisconnected(deviceID, false);
                                break;
                        }
                        m_DeviceRecordsByID[deviceID] = deviceRecord;
                    }

                    currentDataPtr = new IntPtr(currentDataPtr.ToInt64() + eventPtr->sizeInBytes);
                }
            }
        }
    }
}
