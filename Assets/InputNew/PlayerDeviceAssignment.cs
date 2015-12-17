namespace UnityEngine.InputNew
{
	public class PlayerDeviceAssignment
	{
		public readonly PlayerHandle player;
		public readonly InputDevice device;
		public readonly bool shareable;

		public PlayerDeviceAssignment(PlayerHandle playerHandle, InputDevice device, bool allowShared)
		{
			this.device = device;
			this.player = playerHandle;
			this.shareable = allowShared;
		}

		public void Assign()
		{
			player.assignments.Add(this);
			device.assignments.Add(this);
		}

		public void Unassign()
		{
			player.assignments.Remove(this);
			device.assignments.Remove(this);
		}
	}
}
