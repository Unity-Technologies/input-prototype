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

			s_Devices.InitAfterProfiles();

			// Set up event tree.
			s_EventTree = new InputEventTree { name = "Root" };

			var remap = new InputEventTree
			{
				name = "Remap"
				, processInput = s_Devices.RemapEvent
			};
			s_EventTree.children.Add(remap);

			rewriterStack = new InputEventTree
			{
				name = "Rewriters"
				, isStack = true
			};
			s_EventTree.children.Add(rewriterStack);

			var state = new InputEventTree
			{
				name = "State"
				, processInput = s_Devices.ProcessEvent
				, beginNewFrame = s_Devices.BeginNewFrameEvent
			};
			s_EventTree.children.Add(state);
			
			consumerStack = new InputEventTree
			{
				name = "Consumers"
				, isStack = true
			};
			s_EventTree.children.Add(consumerStack);

			simulateMouseWithTouches = true;
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

		public static IEnumerable<SchemeInput> CreateAllPotentialPlayers(ActionMap actionMap, bool onlyOnePlayerPerScheme = false)
		{
			for (var i = 0; i < actionMap.schemes.Count; ++ i)
			{
				foreach (var instance in CreateAllPotentialPlayers(actionMap, i, onlyOnePlayerPerScheme))
				{
					yield return instance;
				}
			}
		}

		public static IEnumerable<SchemeInput> CreateAllPotentialPlayers(ActionMap actionMap, int controlSchemeIndex, bool onlyOnePlayerPerScheme = false)
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeUsedControlIndices = new Dictionary<Type, List<int>>();
			foreach (var entry in actionMap.entries)
			{
				if (entry.bindings == null || entry.bindings.Count <= controlSchemeIndex)
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

			if (!onlyOnePlayerPerScheme)
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

					yield return new SchemeInput(actionMap, controlSchemeIndex, deviceStates);
				}
			}
			else
			{
				var deviceStates = new List<InputState>();

				// Create device states for most recently used device of given types.
				foreach (var entry in perDeviceTypeUsedControlIndices)
				{
					var device = s_Devices.GetMostRecentlyUsedDevice(entry.Key);
					if (device == null)
					{
						yield break; // Can't satisfy this ActionMap; no available device of given type.
					}

					var state = new InputState(device, entry.Value);
					deviceStates.Add(state);
				}

				yield return new SchemeInput(actionMap, controlSchemeIndex, deviceStates);
			}
		}

		public static T CreatePlayer<T>(ActionMap actionMap) where T : PlayerInput
		{
			return (T)Activator.CreateInstance(typeof(T), new object[] { actionMap });
		}

		// This is for creating an instance of a control map that matches the same devices as another control map instance.
		// If the otherActionMapInstance listens to all devices, the new one will too.
		// If the otherActionMapInstance is bound to specific devies, the new one will be bound to same ones or a subset.
		public static T CreatePlayer<T>(ActionMap actionMap, PlayerInput otherPlayerInput) where T : PlayerInput
		{
			if (otherPlayerInput.autoSwitching)
				return (T)Activator.CreateInstance(typeof(T), new object[] { actionMap });
			
			SchemeInput schemeInput = CreateSchemeInput(actionMap, otherPlayerInput.currentScheme.GetUsedDevices());
			return (T)Activator.CreateInstance(typeof(T), new object[] { schemeInput });
		}

		// This is for having explicit control over what devices go into a ActionMapInstance,
		// and automatically determining the control scheme based on it.
		public static SchemeInput CreateSchemeInput(ActionMap actionMap, IEnumerable<InputDevice> devices)
		{
			int matchingControlSchemeIndex = -1;
			for (int scheme = 0; scheme < actionMap.schemes.Count; scheme++)
			{
				var types = actionMap.GetUsedDeviceTypes(scheme);
				bool matchesAll = true;
				foreach (var type in types)
				{
					bool foundMatch = false;
					foreach (var device in devices)
					{
						if (type.IsInstanceOfType(device))
						{
							foundMatch = true;
							break;
						}
					}
					
					if (!foundMatch)
					{
						matchesAll = false;
						break;
					}
				}
				
				if (matchesAll)
				{
					matchingControlSchemeIndex = scheme;
					break;
				}
			}
			
			if (matchingControlSchemeIndex == -1)
				return null;
			
			return CreateSchemeInput(actionMap, devices, matchingControlSchemeIndex);
		}

		// This is for having explicit control over what devices go into a ActionMapInstance.
		public static SchemeInput CreateSchemeInput(ActionMap actionMap, IEnumerable<InputDevice> devices, int controlSchemeIndex)
		{
			// Create state for every device.
			var deviceStates = new List<InputState>();
			foreach (var device in devices)
			{
				deviceStates.Add(new InputState(device));
			}
			
			// Create map instance.
			return new SchemeInput(actionMap, controlSchemeIndex, deviceStates);
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

		internal static void BeginNewFrame()
		{
			s_EventTree.BeginNewFrame(s_EventTree);
		}

		internal static void QueueNativeEvents(List<NativeInputEvent> nativeEvents)
		{
			////TODO

			nativeEvents.Clear();
		}

		private static bool SendSimulatedMouseEvents(InputEvent inputEvent)
		{
			////FIXME: should take actual touchdevice in inputEvent into account
			var touchEvent = inputEvent as TouchEvent;
			if (touchEvent != null)
				Touchscreen.current.SendSimulatedPointerEvents(touchEvent, UnityEngine.Cursor.lockState == CursorLockMode.Locked);
			return false;
		}

		#endregion

		#region Public Properties

		public static IInputConsumer eventTree
		{
			get { return s_EventTree; }
		}

		public static IInputConsumer consumerStack { get; private set; }
		public static IInputConsumer rewriterStack { get; private set; }

		public static IEnumerable<InputDevice> devices
		{
			get { return s_Devices.devices; }
		}
		
		public static TDevice GetMostRecentlyUsedDevice<TDevice>()
			where TDevice : InputDevice
		{
			return s_Devices.GetMostRecentlyUsedDevice<TDevice>();
		}
		
		public static List<InputDevice> leastToMostRecentlyUsedDevices
		{
			get { return s_Devices.leastToMostRecentlyUsedDevices; }
		}

		public static bool simulateMouseWithTouches
		{
			get { return s_SimulateMouseWithTouches; }
			set
			{
				if (value == s_SimulateMouseWithTouches)
					return;

				if (value)
				{
					if (s_SimulateMouseWithTouchesProcess == null)
						s_SimulateMouseWithTouchesProcess = new InputEventTree
						{
							name = "SimulateMouseWithTouches"
							, processInput = SendSimulatedMouseEvents
						};

					rewriterStack.children.Add(s_SimulateMouseWithTouchesProcess);
				}
				else
				{
					if (s_SimulateMouseWithTouchesProcess != null)
						rewriterStack.children.Remove(s_SimulateMouseWithTouchesProcess);
				}

				s_SimulateMouseWithTouches = value;
			}
		}

		#endregion

		#region Fields

		static InputDeviceManager s_Devices;
		static InputEventQueue s_EventQueue;
		static InputEventPool s_EventPool;
		static InputEventTree s_EventTree;
		static bool s_SimulateMouseWithTouches;
		static InputEventTree s_SimulateMouseWithTouchesProcess;

		#endregion
	}
}
