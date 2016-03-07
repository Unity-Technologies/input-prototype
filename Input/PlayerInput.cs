using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

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

		public List<ActionMapSlot> actionMaps = new List<ActionMapSlot>();
		// Should this player handle request assignment of an input device as soon as the component awakes?
		[FormerlySerializedAs("autoSinglePlayerAssign")]
		public bool autoAssignGlobal;

		public DeviceAssignmentStatus status { get; private set; }
		public PlayerHandle handle { get; set; }


		void Awake()
		{
			if (autoAssignGlobal)
				RequestAssign();
		}

		void RequestAssign()
		{
			if (status == DeviceAssignmentStatus.Enabled)
				return;
			
			handle = InputSystem.GetNewPlayerHandle();
			handle.global = true;
			foreach (ActionMapSlot actionMapSlot in actionMaps)
			{
				ActionMapInput actionMapInput = ActionMapInput.Create(actionMapSlot.actionMap);
				actionMapInput.TryInitializeControlSchemeGlobal();
				actionMapInput.active = actionMapSlot.active;
				actionMapInput.blockSubsequent = actionMapSlot.blockSubsequent;
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
