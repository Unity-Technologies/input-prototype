using UnityEngine;
using UnityEngine.InputNew;

// GENERATED FILE - DO NOT EDIT MANUALLY
public class FirstPersonControls : ActionMapInput {
	public FirstPersonControls (ActionMap actionMap) : base (actionMap) { }
	public FirstPersonControls (ControlSchemeInput controlSchemeInput) : base (controlSchemeInput) { }
	
	public AxisInputControl @moveX { get { return (AxisInputControl)this[0]; } }
	public AxisInputControl @moveY { get { return (AxisInputControl)this[1]; } }
	public Vector2InputControl @move { get { return (Vector2InputControl)this[2]; } }
	public AxisInputControl @lookX { get { return (AxisInputControl)this[3]; } }
	public AxisInputControl @lookY { get { return (AxisInputControl)this[4]; } }
	public Vector2InputControl @look { get { return (Vector2InputControl)this[5]; } }
	public ButtonInputControl @fire { get { return (ButtonInputControl)this[6]; } }
	public ButtonInputControl @menu { get { return (ButtonInputControl)this[7]; } }
	public ButtonInputControl @lockCursor { get { return (ButtonInputControl)this[8]; } }
	public ButtonInputControl @unlockCursor { get { return (ButtonInputControl)this[9]; } }
	public ButtonInputControl @reconfigure { get { return (ButtonInputControl)this[10]; } }
}
