using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class ControlMapCombinedInstance : ControlMapInstance
	{
		public ControlMapCombinedInstance (ControlMap controlMap)
		{
			m_ControlMap = controlMap;
			Rebind();
		}
		
		public void Rebind ()
		{
			m_MapInstances = new List<ControlMapInstance>(InputSystem.BindInputs(m_ControlMap));
			
			// Record which control schemes use which device types.
			m_DeviceTypeToControlSchemeIndex.Clear();
			for (int i = 0; i < m_MapInstances.Count; i++)
			{
				ControlMapInstance instance = m_MapInstances[i];
				var devices = m_ControlMap.GetUsedDeviceTypes(instance.controlSchemeIndex);
				foreach (var device in devices)
				{
					m_DeviceTypeToControlSchemeIndex[device] = instance.controlSchemeIndex;
				}
			}
			
			// Find control scheme with most recently used device.
			int controlSchemeIndex = 0;
			List<InputDevice> leastToMost = InputSystem.leastToMostRecentlyUsedDevices;
			for (int i = leastToMost.Count - 1; i >= 0; i--)
			{
				Type type = leastToMost[i].GetType();
				bool stop = false;
				while (type != typeof(InputDevice))
				{
					if (m_DeviceTypeToControlSchemeIndex.ContainsKey(type))
					{
						controlSchemeIndex = m_DeviceTypeToControlSchemeIndex[type];
						stop = true;
						break;
					}
					type = type.BaseType;
				}
				if (stop)
					break;
			}
			
			Setup (m_ControlMap, controlSchemeIndex, m_MapInstances[controlSchemeIndex].GetDeviceStates());
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
		
		private Dictionary<Type, int> m_DeviceTypeToControlSchemeIndex = new Dictionary<Type, int>();
		private List<ControlMapInstance> m_MapInstances;
	}
}
