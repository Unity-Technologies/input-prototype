using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.InputNew
{
	public class PlayerHandle
	{
		public readonly int index;
		public List<PlayerDeviceAssignment> assignments = new List<PlayerDeviceAssignment>();
		public List<ActionMapInput> maps = new List<ActionMapInput>();

		internal PlayerHandle(int index)
		{
			this.index = index;
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

		public bool AssignDevice(InputDevice device, bool assign)
		{
			if (assign)
			{
				if (device.assignment != null)
				{
					// If already assigned to this player, accept as success. Otherwise, fail.
					if (device.assignment.player == this)
						return true;
					else
						return false;
				}

				var assignment = new PlayerDeviceAssignment(this, device);
				assignment.Assign();

				return true;
			}
			else
			{
				if (device.assignment.player == this)
				{
					device.assignment.Unassign();
					return true;
				}
				return false;
			}
		}
	}
	
}
