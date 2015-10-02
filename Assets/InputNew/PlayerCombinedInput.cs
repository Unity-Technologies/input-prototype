using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class PlayerCombinedInput : PlayerInput
	{
		private int m_ControlSchemeIndex = 0;
		private ActionMap m_ActionMap;
		private Dictionary<Type, int> m_DeviceTypeToControlSchemeIndex = new Dictionary<Type, int>();
		private List<PlayerInput> m_MapInstances;

		public override List<InputControlData> controls { get { return m_MapInstances[m_ControlSchemeIndex].controls; } }
		public override InputState state { get { return m_MapInstances[m_ControlSchemeIndex].state; } }
		public override ActionMap actionMap { get { return m_MapInstances[m_ControlSchemeIndex].actionMap; } }
		public override int controlSchemeIndex { get { return m_ControlSchemeIndex; } }
		protected override List<InputState> deviceStates { get { return m_MapInstances[m_ControlSchemeIndex].GetDeviceStates(); } }

		public PlayerCombinedInput(ActionMap actionMap)
		{
			m_ActionMap = actionMap;
			Rebind();
		}

		public void Rebind()
		{
			m_MapInstances = InputSystem.CreateAllPotentialPlayers(m_ActionMap, true).ToList();
			
			// Record which control schemes use which device types.
			m_DeviceTypeToControlSchemeIndex.Clear();
			for (int i = 0; i < m_MapInstances.Count; i++)
			{
				PlayerInput instance = m_MapInstances[i];
				var devices = actionMap.GetUsedDeviceTypes(instance.controlSchemeIndex);
				foreach (var device in devices)
				{
					m_DeviceTypeToControlSchemeIndex[device] = instance.controlSchemeIndex;
				}
			}
			
			// Find control scheme with most recently used device.
			m_ControlSchemeIndex = 0;
			List<InputDevice> leastToMost = InputSystem.leastToMostRecentlyUsedDevices;
			for (int i = leastToMost.Count - 1; i >= 0; i--)
			{
				Type type = leastToMost[i].GetType();
				bool stop = false;
				while (type != typeof(InputDevice))
				{
					if (m_DeviceTypeToControlSchemeIndex.ContainsKey(type))
					{
						m_ControlSchemeIndex = m_DeviceTypeToControlSchemeIndex[type];
						stop = true;
						break;
					}
					type = type.BaseType;
				}
				if (stop)
					break;
			}
		}

		public override bool ProcessEvent(InputEvent inputEvent)
		{
			bool consumed = base.ProcessEvent(inputEvent);
			if (consumed)
				return true;
			
			// Check if it could have been used with another control scheme.
			for (var type = inputEvent.deviceType; type != typeof(InputDevice) || type == null; type = type.BaseType)
			{
				int otherControlSchemeIndex = -1;
				if (m_DeviceTypeToControlSchemeIndex.TryGetValue(type, out otherControlSchemeIndex))
				{
						////TODO: prevent from constantly toggling
					//if (otherControlSchemeIndex != controlSchemeIndex)
					{
						// Try to switch to other control scheme and process event again.
						Rebind();
						return base.ProcessEvent(inputEvent);
					}
				}
			}
			
			return false;
		}
	}
}
