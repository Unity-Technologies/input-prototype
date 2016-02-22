using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	/*
	Things to test for action map / control schemes.

	- When pressing e.g. mouse button or gamepad trigger in one action map creates a new action map
	  based on the same device, the new action map should not immediately have wasJustPressed be true.
	  Hence the state in the newly created control scheme should be initialized to the state
	  of the devices it's based on.
	
	- When pressing e.g. mouse button or gamepad trigger and it causes a switch in control scheme
	  within an existing action map, the new control scheme *should* immediately have wasJustPressed be true.
	  Hence the state in the newly created control scheme should not be initialized to the state
	  of the devices it's based on.

	*/
	public class ActionMapInput : IInputControlProvider
	{
		private ActionMap m_ActionMap;
		public ActionMap actionMap { get { return m_ActionMap; } }

		bool m_Active;
		public bool active {
			get {
				return m_Active;
			}
			set {
				m_Active = value;
				if (m_ControlSchemeInput != null)
					m_ControlSchemeInput.Reset(value);
			}
		}

		private ControlSchemeInput m_ControlSchemeInput = null;
		public ControlSchemeInput controlSchemeInput { get { return m_ControlSchemeInput; } }

		public List<InputControlData> controlDataList { get { return controlSchemeInput.controlDataList; } }
		public InputState state { get { return controlSchemeInput.state; } }

		public static ActionMapInput Create(ActionMap actionMap)
		{
			ActionMapInput map =
				(ActionMapInput)Activator.CreateInstance(actionMap.customActionMapType, new object[] { actionMap });
			return map;
		}

		protected ActionMapInput(ActionMap actionMap)
		{
			m_ActionMap = actionMap;
		}

		public bool TryInitializeControlScheme(InputDevice inputDevice)
		{
			if (inputDevice.assignment == null)
				return TryInitializeControlSchemeGlobal();
			else
				return TryInitializeControlSchemeForPlayer(inputDevice.assignment.player);
		}

		public bool TryInitializeControlSchemeGlobal()
		{
			var devices = InputSystem.leastToMostRecentlyUsedDevices.Where(e => e.assignment == null).Reverse().ToList();
			return Assign(devices);
		}

		public bool TryInitializeControlSchemeForPlayer(PlayerHandle player)
		{
			var devices = player.assignments.Select(e => e.device).ToList();
			return Assign(devices);
		}

		public bool Assign(List<InputDevice> availableDevices)
		{
			int bestScheme = -1;
			List<InputDevice> bestFoundDevices = null;
			float mostRecentTime = -1;

			List<InputDevice> foundDevices = new List<InputDevice>();
			for (int scheme = 0; scheme < actionMap.controlSchemes.Count; scheme++)
			{
				float timeForScheme = -1;
				foundDevices.Clear();
				var types = actionMap.controlSchemes[scheme].GetUsedDeviceTypes().ToList();
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
							timeForScheme = Mathf.Max(timeForScheme, device.lastEventTime);
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
				if (timeForScheme > mostRecentTime)
				{
					bestScheme = scheme;
					bestFoundDevices = new List<InputDevice>(foundDevices);
					mostRecentTime = timeForScheme;
				}
			}

			if (bestScheme == -1)
				return false;
			
			ControlScheme matchingControlScheme = actionMap.controlSchemes[bestScheme];
			Assign(new ControlSchemeInput(matchingControlScheme, bestFoundDevices));
			return true;
		}

		private void Assign(ControlSchemeInput controlSchemeInput)
		{
			if (controlSchemeInput.actionMap != actionMap)
				throw new Exception(string.Format("ControlSchemeInput doesn't match ActionMap {0}.", actionMap.name));
			m_ControlSchemeInput = controlSchemeInput;
			m_ControlSchemeInput.Reset();
		}

		public InputControl this[int index]
		{
			get { return controlSchemeInput[index]; }
		}

		internal bool ProcessEvent(InputEvent inputEvent)
		{
			if (controlSchemeInput.ProcessEvent(inputEvent))
				return true;

			if (controlSchemeInput.GetDeviceStates().Any(e => e.controlProvider == inputEvent.device))
				return false;

			TryInitializeControlScheme(inputEvent.device);

			// When changing control scheme, we do not want to init control scheme to device states
			// like we normally want, so do a hard reset here, before processing the new event.
			m_ControlSchemeInput.Reset(false);

			return controlSchemeInput.ProcessEvent(inputEvent);
		}

		internal void BeginFrameEvent()
		{
			controlSchemeInput.BeginFrame();
		}
		
		internal void EndFrameEvent()
		{
			controlSchemeInput.EndFrame();
		}
	}
}
