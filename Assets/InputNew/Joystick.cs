using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	// Must be different from Gamepad as the standardized controls for Gamepads don't
	// work for joysticks.
	public class Joystick
		: InputDevice
	{
		#region Constructors

		public Joystick(string deviceName, List<InputControlData> controls)
			: base(deviceName, controls) { }

		#endregion

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			var consumed = false;

			var genericEvent = inputEvent as GenericControlEvent;
			if (genericEvent != null)
			{
				consumed |= state.SetCurrentValue(genericEvent.controlIndex, genericEvent.value);
			}

			if (consumed)
				return true;

			return base.ProcessEventIntoState(inputEvent, intoState);
		}
	}
}
