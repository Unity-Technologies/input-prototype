using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class PlayerInput : MonoBehaviour
	{
		public enum DeviceAssignmentMethod
		{
			AnyAvailable, // single player or local multi-player
			PressedByPlayer, // local multi-player
		}

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

		public PlayerHandle handle;

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

			status = DeviceAssignmentStatus.Pending;

			if (actionMaps.Count == 0 || actionMaps[0] == null)
				return;
			
			ActionMap actionMap = actionMaps[0];

			handle = InputSystem.CreatePlayerHandle(actionMap, false);

			if (handle != null)
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
