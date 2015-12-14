using System;
using System.Collections.Generic;
using UnityEngine;

//// - solve mapping of device type names from control maps to device types at runtime

namespace UnityEngine.InputNew
{
	public static class InputSystem
	{
		public delegate bool BindingListener(InputControl control);
		
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
				, beginFrame = s_Devices.BeginFrameEvent
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

		public static IEnumerable<ControlSchemeInput> CreateAllPotentialPlayers(ActionMap actionMap)
		{
			for (var i = 0; i < actionMap.controlSchemes.Count; ++ i)
			{
				foreach (var instance in CreateAllPotentialPlayersForControlScheme(actionMap, actionMap.controlSchemes[i]))
				{
					yield return instance;
				}
			}
		}

		private static IEnumerable<ControlSchemeInput> CreateAllPotentialPlayersForControlScheme(ActionMap actionMap, ControlScheme controlScheme)
		{
			// Gather a mapping of device types to list of bindings that use the given type.
			var perDeviceTypeUsedControlIndices = new Dictionary<Type, List<int>>();
			controlScheme.ExtractDeviceTypesAndControlIndices(perDeviceTypeUsedControlIndices);

			////REVIEW: what to do about disconnected devices here? skip? include? make parameter?

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
			for (var i = 0; i < minDeviceCountOfType; i++)
			{
				var deviceStates = new List<InputState>();

				foreach (var entry in perDeviceTypeUsedControlIndices)
				{
					// Take i-th device of current type.
					var device = deviceTypesToAvailableDevices[entry.Key][i];
					var state = new InputState(device, entry.Value);
					deviceStates.Add(state);
				}

				yield return new ControlSchemeInput(actionMap, controlScheme, deviceStates);
			}
		}

		// This is for creating a PlayerInput (from an ActionMap) that matches the same devices as another PlayerInput.
		// If the otherPlayerInput listens to all devices, the new one will too.
		// If the otherPlayerInput is bound to specific devies, the new one will be bound to same ones or a subset.
		public static T CreatePlayer<T>(ActionMap actionMap, ActionMapInput otherPlayerInput) where T : ActionMapInput
		{
			if (otherPlayerInput.autoSwitching)
				return (T)Activator.CreateInstance(typeof(T), new object[] { actionMap });
			
			ControlSchemeInput schemeInput = CreateControlSchemeInput(actionMap, otherPlayerInput.currentControlScheme.GetUsedDevices());
			return (T)Activator.CreateInstance(typeof(T), new object[] { schemeInput });
		}

		// This is for having explicit control over what devices go into a SchemeInput,
		// and automatically determining the control scheme based on it.
		public static ControlSchemeInput CreateControlSchemeInput(ActionMap actionMap, IEnumerable<InputDevice> devices)
		{
			ControlScheme matchingControlScheme = null;
			for (int scheme = 0; scheme < actionMap.controlSchemes.Count; scheme++)
			{
				var types = actionMap.controlSchemes[scheme].GetUsedDeviceTypes();
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
					matchingControlScheme = actionMap.controlSchemes[scheme];
					break;
				}
			}
			
			if (matchingControlScheme == null)
				return null;
			
			return CreateControlSchemeInput(actionMap, devices, matchingControlScheme);
		}

		// This is for having explicit control over what devices go into a ActionMapInstance.
		public static ControlSchemeInput CreateControlSchemeInput(ActionMap actionMap, IEnumerable<InputDevice> devices, ControlScheme controlScheme)
		{
			// Create state for every device.
			var deviceStates = new List<InputState>();
			foreach (var device in devices)
			{
				deviceStates.Add(new InputState(device));
			}
			
			// Create map instance.
			return new ControlSchemeInput(actionMap, controlScheme, deviceStates);
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

		internal static void BeginFrame()
		{
			s_EventTree.BeginFrame();
		}

		internal static void EndFrame()
		{
			s_EventTree.EndFrame();
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
		
		public static void ListenForBinding (BindingListener listener)
		{
			s_BindingListeners.Add(listener);
		}
		
		internal static void RegisterBinding(InputControl control)
		{
			for (int i = s_BindingListeners.Count - 1; i >= 0; i--)
			{
				if (s_BindingListeners[i] == null)
				{
					s_BindingListeners.RemoveAt(i);
					continue;
				}
				bool used = s_BindingListeners[i](control);
				if (used)
				{
					s_BindingListeners.RemoveAt(i);
					break;
				}
			}
		}

		#endregion

		#region Public Properties

		public static IInputConsumer eventTree
		{
			get { return s_EventTree; }
		}

		public static IInputConsumer consumerStack { get; private set; }
		public static IInputConsumer rewriterStack { get; private set; }

		public static bool listeningForBinding
		{
			get { return s_BindingListeners.Count > 0; }
		}

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
		static List<BindingListener> s_BindingListeners = new List<BindingListener>();

		#endregion
	}
}
