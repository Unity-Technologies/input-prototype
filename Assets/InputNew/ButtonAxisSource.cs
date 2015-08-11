using System;

namespace UnityEngine.InputNew
{
	[ Serializable ]
	public class ButtonAxisSource
	{
		public InputControlDescriptor negative;
		public InputControlDescriptor positive;
		public ButtonAxisSource( InputControlDescriptor negative, InputControlDescriptor positive )
		{
			this.negative = negative;
			this.positive = positive;
		}
	}
}