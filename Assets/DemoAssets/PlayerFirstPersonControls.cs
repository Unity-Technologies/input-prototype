using UnityEngine;
using System.Collections;

namespace UnityEngine.InputNew
{
	// Example of class we would need to auto-generate.
	public class PlayerFirstPersonControls : PlayerInput
	{
		public PlayerFirstPersonControls(ActionMap actionMap) : base(actionMap) { }
		public PlayerFirstPersonControls(SchemeInput schemeInput) : base(schemeInput) { }
		
		public InputControl moveX { get { return this[0]; } }
		public InputControl moveY { get { return this[1]; } }
		public InputControl move { get { return this[2]; } }
		
		public InputControl lookX { get { return this[3]; } }
		public InputControl lookY { get { return this[4]; } }
		public InputControl look { get { return this[5]; } }
		
		public InputControl fire { get { return this[6]; } }
		public InputControl menu { get { return this[7]; } }
		public InputControl lockCursor { get { return this[8]; } }
		public InputControl unlockCursor { get { return this[9]; } }
	}
}