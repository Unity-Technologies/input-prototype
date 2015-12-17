using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.InputNew
{
	public class PlayerHandle
	{
		public int index;
		public bool autoSwitching;
		public List<PlayerDeviceAssignment> assignments = new List<PlayerDeviceAssignment>();
		public List<ActionMapInput> maps = new List<ActionMapInput>();

		// For single-player will always succeed.
		// For multi-player will succeed if ActionMap uses device types already assigned to this player
		// or if buttons are pressed on applicable devices that are not already assigned.
		public T AssignActions<T>(ActionMap actionMap) where T : ActionMapInput
		{
			return (T)AssignActions(actionMap, typeof(T));
		}

		public T GetActions<T>() where T : ActionMapInput
		{
			// If already contains actionMap if this type, return that.
			for (int i = 0; i < maps.Count; i++)
				if (maps[i].GetType() == typeof(T))
					return (T)maps[i];
			return null;
		}

		public void Destroy()
		{
			foreach (var map in maps)
				map.active = false;
			for (int i = assignments.Count - 1; i >= 0; i--)
				assignments[i].Unassign();
		}

		internal ActionMapInput AssignActions(ActionMap actionMap, Type customActionMapType)
		{
			// If already contains actionMap if this type, return that.
			for (int i = 0; i < maps.Count; i++)
				if (maps[i].GetType() == customActionMapType)
					return maps[i];

			if (autoSwitching)
			{
				ActionMapInput map = (ActionMapInput)Activator.CreateInstance(customActionMapType, new object[] { actionMap });
				maps.Add(map);
				return map;
				// Do not track device assignments for auto switching player.
			}
			else
			{
				// Try to create ControlSchemeInput from devices already assigned to player.
				List<InputDevice> devices = assignments.Select(e => e.device).ToList();
				List<InputDevice> dummyList = null;
				ControlSchemeInput controlSchemeInput = CreateControlSchemeInput(actionMap, null, devices, out dummyList);
				if (controlSchemeInput != null)
					return (ActionMapInput)Activator.CreateInstance(customActionMapType, new object[] { controlSchemeInput });

				// If a player pressed a button on an unassigned device,
				// and this device fit one of the control schemes, assign that device and control scheme.
				List<InputDevice> availableDevices = InputSystem.leastToMostRecentlyUsedDevices
					.Where(e => e.assignments.Count == 0).ToList();
				for (int i = availableDevices.Count - 1; i >= 0; i--)
				{
					InputDevice joinedDevice = availableDevices[i];
					if (joinedDevice.assignments.Count == 0 && (joinedDevice.anyButton != null) && joinedDevice.anyButton.buttonDown)
					{
						List<InputDevice> foundDevices;
						ControlSchemeInput schemeInput = CreateControlSchemeInput(actionMap, joinedDevice, availableDevices, out foundDevices);
						if (schemeInput != null)
						{
							ActionMapInput map = (ActionMapInput)Activator.CreateInstance(customActionMapType, new object[] { schemeInput });
							for (int j = 0; j < foundDevices.Count; j++)
								AssignDevice(foundDevices[j], true, false);
							maps.Add(map);
							return map;
						}
					}
				}
			}
			
			return null;
		}

		private static ControlSchemeInput CreateControlSchemeInput(ActionMap actionMap, InputDevice requiredDevice, List<InputDevice> availableDevices, out List<InputDevice> foundDevices)
		{
			ControlScheme matchingControlScheme = null;
			foundDevices = new List<InputDevice>();
			for (int scheme = 0; scheme < actionMap.controlSchemes.Count; scheme++)
			{
				foundDevices.Clear();
				var types = actionMap.controlSchemes[scheme].GetUsedDeviceTypes().ToList();

				if (requiredDevice != null)
				{
					bool matchesRequired = false;
					foreach (var type in types)
					{
						if (type.IsInstanceOfType(requiredDevice))
						{
							foundDevices.Add(requiredDevice);
							types.Remove(type);
							matchesRequired = true;
							break;
						}
					}
					if (!matchesRequired)
						continue;
				}

				bool matchesAll = true;
				foreach (var type in types)
				{
					bool foundMatch = false;
					foreach (var device in availableDevices)
					{
						if (type.IsInstanceOfType(device))
						{
							foundDevices.Add(device);
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
				if (!matchesAll)
					continue;

				// If we reach this point we know that control scheme both matches required and matches all.
				matchingControlScheme = actionMap.controlSchemes[scheme];
				break;
			}
			
			if (matchingControlScheme == null)
			{
				foundDevices = null;
				return null;
			}
			
			return new ControlSchemeInput(actionMap, matchingControlScheme, foundDevices);
		}

		private bool AssignDevice(InputDevice device, bool assign, bool allowShared = false)
		{
			if (assign)
			{
				if (device.assignments.Count > 0)
				{
					// If assigned to other player and not sharable, fail assignment.
					if (!allowShared && device.assignments[0].player != this)
						return false;

					for (int i = 0; i < device.assignments.Count; i++)
					{
						// Already assigned to that player - accept as success.
						if (device.assignments[i].player != this)
							return true;
						// Assigned to other player and not sharable.
						if (!device.assignments[i].shareable)
							return false;
					}
				}

				var assignment = new PlayerDeviceAssignment(this, device, allowShared);
				assignment.Assign();

				return true;
			}
			else
			{
				for (int i = 0; i < device.assignments.Count; i++)
				{
					if (device.assignments[i].player == this)
					{
						var assignment = device.assignments[i];
						assignment.device.assignments.Remove(assignment);
						assignment.player.assignments.Remove(assignment);
						return true;
					}
				}
				return false;
			}
		}
	}
	
}
