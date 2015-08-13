using System;
using System.Collections.Generic;
using UnityEngine;

//// - solve mapping of device type names from control maps to device types at runtime

namespace UnityEngine.InputNew
{
	public static class InputSystem
	{
		#region Public Methods

		public static void Initialize(InputDeviceProfile[] profiles)
		{
			s_Devices = new InputDeviceManager();
			s_EventQueue = new InputEventQueue();
			s_EventPool = new InputEventPool();

			foreach (var profile in profiles)
			{
				RegisterProfile(profile);
			}

			// Set up event tree.
			s_EventTree = new InputEventTree { name = "Root" };

			var remap = new InputEventTree
			{
				name = "Remap"
				, processInput = s_Devices.RemapEvent
			};
			s_EventTree.children.Add(remap);

			var state = new InputEventTree
			{
				name = "State"
				, processInput = s_Devices.ProcessEvent
			};
			s_EventTree.children.Add(state);
		}

		public static void RegisterProfile(InputDeviceProfile profile)
		{
			s_Devices.RegisterProfile(profile);
		}

		public static InputDevice LookupDevice(Type deviceType, int deviceIndex)
		{
			return s_Devices.LookupDevice(deviceType, deviceIndex);
		}

		public static void QueueEvent(InputEvent inputEvent)
		{
			s_EventQueue.Queue(inputEvent);
		}

		public static bool ExecuteEvent(InputEvent inputEvent)
		{
			var wasConsumed = s_EventTree.ProcessEvent(inputEvent);
			s_EventPool.Return(inputEvent);
			return wasConsumed;
		}

		public static TEvent CreateEvent<TEvent>()
			where TEvent : InputEvent, new()
		{
			var newEvent = s_EventPool.ReuseOrCreate<TEvent>();
			newEvent.time = Time.time;
			return newEvent;
		}

		public static IEnumerable<ControlMapInstance> BindInputs(ControlMap controlMap, bool localMultiplayer = false)
		{
			for (var i = 0; i < controlMap.schemes.Count; ++ i)
			{
				foreach (var instance in BindInputs(controlMap, localMultiplayer, i))
				{
					yield return instance;
				}
			}
		}

		static void ExtractDeviceTypeAndControlIndexFromSource(Dictionary<Type, List<int>> perDeviceTypeMapEntries, InputControlDescriptor control)
		{
			List<int> entries;
			if (!perDeviceTypeMapEntries.TryGetValue(control.deviceType, out entries))
			{
				entries = new List<int>();
				perDeviceTypeMapEntries[control.deviceType] = entries;
			}

			entries.Add(control.controlIndex);
		}

		public static IEnumerable<ControlMapInstance> BindInputs(ControlMap controlMap, bool localMultiplayer, int controlSchemeIndex)
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeUsedControlIndices = new Dictionary<Type, List<int>>();
			foreach (var entry in controlMap.entries)
			{
				if (entry.bindings == null || entry.bindings.Count == 0)
					continue;

				foreach (var control in entry.bindings[controlSchemeIndex].sources)
				{
					ExtractDeviceTypeAndControlIndexFromSource(perDeviceTypeUsedControlIndices, control);
				}

				foreach (var axis in entry.bindings[controlSchemeIndex].buttonAxisSources)
				{
					ExtractDeviceTypeAndControlIndexFromSource(perDeviceTypeUsedControlIndices, axis.negative);
					ExtractDeviceTypeAndControlIndexFromSource(perDeviceTypeUsedControlIndices, axis.positive);
				}
			}

			////REVIEW: what to do about disconnected devices here? skip? include? make parameter?

			// Create list of controls from InputMap.
			var controls = new List<InputControlData>();
			foreach (var entry in controlMap.entries)
			{
				var control = new InputControlData
				{
					name = entry.controlData.name
					, controlType = entry.controlData.controlType
				};
				controls.Add(control);
			}

			if (localMultiplayer)
			{
				// Gather available devices for each type of device.
				var deviceTypesToAvailableDevices = new Dictionary<Type, List<InputDevice>>();
				var minDeviceCountOfType = Int32.MaxValue;
				foreach (var deviceType in perDeviceTypeUsedControlIndices.Keys)
				{
					var availableDevicesOfType = s_Devices.GetDevicesOfType(deviceType);
					if (availableDevicesOfType != null)
						deviceTypesToAvailableDevices[deviceType] = availableDevicesOfType;

					minDeviceCountOfType = Mathf.Min(minDeviceCountOfType, availableDevicesOfType != null ? availableDevicesOfType.Count : 0);
				}

				// Create map instances according to available devices.
				for (var i = 0; i < minDeviceCountOfType; ++ i)
				{
					var deviceStates = new List<InputState>();

					foreach (var entry in perDeviceTypeUsedControlIndices)
					{
						// Take i-th device of current type.
						var device = deviceTypesToAvailableDevices[entry.Key][i];
						var state = new InputState(device, entry.Value);
						deviceStates.Add(state);
					}

					yield return new ControlMapInstance(controlMap, controlSchemeIndex, controls, deviceStates);
				}
			}
			else
			{
				////TODO: make ControlMapInstance hook into MRU device change event and respond by updating its device states

				var deviceStates = new List<InputState>();

				// Create device states for most recently used device of given types.
				foreach (var entry in perDeviceTypeUsedControlIndices)
				{
					var device = s_Devices.GetMostRecentlyUsedDevice(entry.Key);
					if (device == null)
						yield break; // Can't satisfy this ControlMap; no available device of given type.

					var state = new InputState(device, entry.Value);
					deviceStates.Add(state);
				}

				yield return new ControlMapInstance(controlMap, controlSchemeIndex, controls, deviceStates);
			}
		}

		// This is for having explicit control over what devices go into a ControlMapInstance.
		public static ControlMapInstance BindInputs(ControlMap controlMap, int controlSchemeIndex, IEnumerable<InputDevice> devices)
		{
			// Create state for every device.
			var deviceStates = new List<InputState>();
			foreach (var device in devices)
			{
				deviceStates.Add(new InputState(device));
			}

			// Create list of controls from InputMap.
			var controls = new List<InputControlData>();
			foreach (var entry in controlMap.entries)
			{
				var control = new InputControlData
				{
					name = entry.controlData.name
					, controlType = entry.controlData.controlType
				};
				controls.Add(control);
			}

			// Create map instance.
			return new ControlMapInstance(controlMap, controlSchemeIndex, controls, deviceStates);
		}

		#endregion

		#region Non-Public Methods

		internal static void ExecuteEvents()
		{
			var currentTime = Time.time;
			InputEvent nextEvent;
			while (s_EventQueue.Dequeue(currentTime, out nextEvent))
			{
				ExecuteEvent(nextEvent);
			}
		}

		internal static void QueueNativeEvents(List<NativeInputEvent> nativeEvents)
		{
			////TODO

			nativeEvents.Clear();
		}

		#endregion

		#region Public Properties

		public static IInputConsumer eventTree
		{
			get { return s_EventTree; }
		}

		public static Pointer pointer
		{
			get { return s_Devices.pointer; }
		}

		public static Keyboard keyboard
		{
			get { return s_Devices.keyboard; }
		}

		public static Mouse mouse
		{
			get { return s_Devices.mouse; }
		}

		public static Touchscreen touchscreen
		{
			get { return s_Devices.touchscreen; }
		}

		public static IEnumerable<InputDevice> devices
		{
			get { return s_Devices.devices; }
		}

		#endregion

		#region Fields

		static InputDeviceManager s_Devices;
		static InputEventQueue s_EventQueue;
		static InputEventPool s_EventPool;
		static InputEventTree s_EventTree;

		#endregion
	}
}
