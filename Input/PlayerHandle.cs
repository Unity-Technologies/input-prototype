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

		private bool m_Global = false;
		private InputEventTree treeNode { get; set; }

		public delegate void ChangeEvent();
		public static ChangeEvent onChange;

		internal PlayerHandle(int index)
		{
			this.index = index;

			treeNode = new InputEventTree
			{
				name = "Player "+index,
				processInput = ProcessEvent,
				beginFrame = BeginFrameEvent,
				endFrame = EndFrameEvent
			};
			InputSystem.consumerStack.children.Add(treeNode);

			if (onChange != null)
				onChange.Invoke();
		}

		public bool global
		{
			get { return m_Global; }
			set
			{
				if (value == m_Global)
					return;

				m_Global = value;
				if (value)
				{
					InputSystem.consumerStack.children.Remove(treeNode);
					InputSystem.globalConsumerStack.children.Add(treeNode);
				}
				else
				{
					InputSystem.globalConsumerStack.children.Remove(treeNode);
					InputSystem.consumerStack.children.Add(treeNode);
				}

				if (onChange != null)
					onChange.Invoke();
			}
		}

		public T GetActions<T>() where T : ActionMapInput
		{
			// If already contains ActionMapInput if this type, return that.
			for (int i = 0; i < maps.Count; i++)
				if (maps[i].GetType() == typeof(T))
					return (T)maps[i];
			return null;
		}

		public ActionMapInput GetActions(ActionMap actionMap)
		{
			// If already contains ActionMapInput based on this ActionMap, return that.
			for (int i = 0; i < maps.Count; i++)
				if (maps[i].actionMap == actionMap)
					return maps[i];
			return null;
		}

		public void Destroy()
		{
			foreach (var map in maps)
				map.active = false;

			for (int i = assignments.Count - 1; i >= 0; i--)
				assignments[i].Unassign();
			
			InputSystem.consumerStack.children.Remove(treeNode);
			treeNode = null;

			InputSystem.RemovePlayerHandle(this);
			if (onChange != null)
				onChange.Invoke();
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

		bool ProcessEvent(InputEvent inputEvent)
		{
			if (!global && (inputEvent.device.assignment == null || inputEvent.device.assignment.player != this))
				return false;

			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].active && maps[i].ProcessEvent(inputEvent))
					return true;
			}

			return false;
		}

		void BeginFrameEvent()
		{
			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].active)
					maps[i].BeginFrame();
			}
		}
		
		void EndFrameEvent()
		{
			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].active)
					maps[i].EndFrame();
			}
		}
	}
	
}
