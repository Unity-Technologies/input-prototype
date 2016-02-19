using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class PlayerInput : MonoBehaviour
	{
		public enum DeviceAssignmentStatus
		{
			Disabled,
			Pending,
			Enabled,
		}

		public List<ActionMap> actionMaps = new List<ActionMap>();
		// Should this player handle request assignment of an input device as soon as the component awakes?
		public bool autoSinglePlayerAssign;

		public DeviceAssignmentStatus status { get; private set; }
		public PlayerHandle handle { get; set; }


		void Awake()
		{
			if (autoSinglePlayerAssign)
				RequestAssign();
		}

		void Update()
		{
			if (autoSinglePlayerAssign)
				RequestAssign();
		}

		void RequestAssign()
		{
			if (status == DeviceAssignmentStatus.Enabled)
				return;
			
			handle = InputSystem.GetNewPlayerHandle();
			handle.global = true;
			foreach (ActionMap actionMap in actionMaps)
			{
				ActionMapInput actionMapInput = ActionMapInput.Create(actionMap);
				actionMapInput.TryInitializeControlSchemeGlobal();
				actionMapInput.active = true;
				handle.maps.Add(actionMapInput);
			}
			status = DeviceAssignmentStatus.Enabled;
		}

		public T GetActions<T>() where T : ActionMapInput
		{
			if (handle == null)
				return null;
			return handle.GetActions<T>();
		}
	}
}
