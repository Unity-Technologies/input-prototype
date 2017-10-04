using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
	public class XRInputDevice
		: InputDevice
	{
		public enum XRControl
		{
			// Standardized.
			Analog0,

			LeftStickX = Analog0,
			LeftStickY,

			Trigger1,
			Trigger2,

            Analog4,
			Analog5,
			Analog6,
			Analog7,
			Analog8,
			Analog9,

			LocalPositionX,
			LocalPositionY,
			LocalPositionZ,

			LocalRotationX,
			LocalRotationY,
			LocalRotationZ,
			LocalRotationW,

			Action1,
			Action2,
			Action3,
			Action4,
            Action5,

		    LeftStickButton,

			// Compound controls.

			LocalPosition,
			LocalRotation,
			LeftStick,

			// Not standardized, but provided for convenience.

			Back,
			Start,
			//Select,
			//Pause,
			//Menu,
			//Share,
			//View,
			//Options
		}

	    private static readonly string[] kTags = { "Left", "Right"};
	    public static string[] Tags
	    {
	        get
	        {
	            return kTags;
	        }
	    }

	    public enum Handedness { Left, Right }

	    private Handedness? hand = null;
	    public Handedness? Hand
	    {
            get { return hand; }
            set { hand = value; }
	    }

		public override int tagIndex
		{
			get
			{
				if (hand.HasValue)
					return (int) hand.Value;
				return -1;
			}
		}

	    public XRInputDevice()
			: this("XRInputDevice", null) {}

		public XRInputDevice(string deviceName, List<InputControlData> additionalControls)
		{
			this.deviceName = deviceName;
			var controlCount = EnumHelpers.GetValueCount<XRControl>();
			var controls = Enumerable.Repeat(new InputControlData(), controlCount).ToList();

			// Compounds.
			controls[(int)XRControl.LeftStick] = new InputControlData
			{
				name = "Left Stick"
				, controlType = typeof(Vector2InputControl)
				, componentControlIndices = new[] { (int)XRControl.LeftStickX, (int)XRControl.LeftStickY }
			};
			controls[(int)XRControl.LocalPosition] = new InputControlData
			{
				name = "Local Position"
				, controlType = typeof(Vector3InputControl)
				, componentControlIndices = new[] { (int)XRControl.LocalPositionX, (int)XRControl.LocalPositionY, (int)XRControl.LocalPositionZ }
			};
			controls[(int)XRControl.LocalRotation] = new InputControlData
			{
				name = "Local Rotation"
				, controlType = typeof(QuaternionInputControl)
				, componentControlIndices = new[] { (int)XRControl.LocalRotationX, (int)XRControl.LocalRotationY, (int)XRControl.LocalRotationZ, (int)XRControl.LocalRotationW }
			};
			////TODO: dpad (more complicated as the source is buttons which need to be translated into a vector)

			// Buttons.
			controls[(int)XRControl.Action1] = new InputControlData { name = "Action 1", controlType = typeof(ButtonInputControl) };
			controls[(int)XRControl.Action2] = new InputControlData { name = "Action 2", controlType = typeof(ButtonInputControl) };
			controls[(int)XRControl.Action3] = new InputControlData { name = "Action 3", controlType = typeof(ButtonInputControl) };
			controls[(int)XRControl.Action4] = new InputControlData { name = "Action 4", controlType = typeof(ButtonInputControl) };
            controls[(int)XRControl.Action5] = new InputControlData { name = "Action 5", controlType = typeof(ButtonInputControl) };

            controls[(int)XRControl.Start] = new InputControlData { name = "Start", controlType = typeof(ButtonInputControl) };
			controls[(int)XRControl.Back] = new InputControlData { name = "Back", controlType = typeof(ButtonInputControl) };
			controls[(int)XRControl.LeftStickButton] = new InputControlData { name = "Left Stick Button", controlType = typeof(ButtonInputControl) };

			// Axes.
			controls[(int)XRControl.LeftStickX] = new InputControlData { name = "Left Stick X", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LeftStickY] = new InputControlData { name = "Left Stick Y", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LocalPositionX] = new InputControlData { name = "Local Position X", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LocalPositionY] = new InputControlData { name = "Local Position Y", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LocalPositionZ] = new InputControlData { name = "Local Position Z", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LocalRotationX] = new InputControlData { name = "Local Rotation X", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LocalRotationY] = new InputControlData { name = "Local Rotation Y", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LocalRotationZ] = new InputControlData { name = "Local Rotation Z", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.LocalRotationW] = new InputControlData { name = "Local Rotation W", controlType = typeof(AxisInputControl) };

			controls[(int)XRControl.Trigger1] = new InputControlData() { name = "Trigger 1", controlType = typeof(AxisInputControl) };
			controls[(int)XRControl.Trigger2] = new InputControlData() { name = "Trigger 2", controlType = typeof(AxisInputControl) };

			if (additionalControls != null)
				controls.AddRange(additionalControls);

			SetControls(controls);
		}

		public AxisInputControl leftStickX { get { return (AxisInputControl)this[(int)XRControl.LeftStickX]; } }
		public AxisInputControl leftStickY { get { return (AxisInputControl)this[(int)XRControl.LeftStickY]; } }
		public ButtonInputControl leftStickButton { get { return (ButtonInputControl)this[(int)XRControl.LeftStickButton]; } }

		public ButtonInputControl action1 { get { return (ButtonInputControl)this[(int)XRControl.Action1]; } }
		public ButtonInputControl action2 { get { return (ButtonInputControl)this[(int)XRControl.Action2]; } }
		public ButtonInputControl action3 { get { return (ButtonInputControl)this[(int)XRControl.Action3]; } }
		public ButtonInputControl action4 { get { return (ButtonInputControl)this[(int)XRControl.Action4]; } }
        public ButtonInputControl action5 { get { return (ButtonInputControl)this[(int)XRControl.Action5]; } }

        public AxisInputControl trigger1 { get { return (AxisInputControl)this[(int)XRControl.Trigger1]; } }
		public AxisInputControl trigger2 { get { return (AxisInputControl)this[(int)XRControl.Trigger2]; } }

		public AxisInputControl localPositionX { get { return (AxisInputControl)this[(int)XRControl.LocalPositionX]; } }
		public AxisInputControl localPositionY { get { return (AxisInputControl)this[(int)XRControl.LocalPositionY]; } }
		public AxisInputControl localPositionZ { get { return (AxisInputControl)this[(int)XRControl.LocalPositionZ]; } }

		public AxisInputControl localRotationX { get { return (AxisInputControl)this[(int)XRControl.LocalRotationX]; } }
		public AxisInputControl localRotationY { get { return (AxisInputControl)this[(int)XRControl.LocalRotationY]; } }
		public AxisInputControl localRotationZ { get { return (AxisInputControl)this[(int)XRControl.LocalRotationZ]; } }
		public AxisInputControl localRotationW { get { return (AxisInputControl)this[(int)XRControl.LocalRotationW]; } }

		// Compound controls.

		public Vector2InputControl leftStick { get { return (Vector2InputControl)this[(int)XRControl.LeftStick]; } }
		public Vector3InputControl localPosition { get { return (Vector3InputControl)this[(int)XRControl.LocalPosition]; } }
		public QuaternionInputControl localRotation { get { return (QuaternionInputControl)this[(int)XRControl.LocalRotation]; } }

		// Not standardized, but provided for convenience.

		public ButtonInputControl back { get { return (ButtonInputControl)this[(int)XRControl.Back]; } }
		public ButtonInputControl start { get { return (ButtonInputControl)this[(int)XRControl.Start]; } }

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{

			if (base.ProcessEventIntoState(inputEvent, intoState))
				return true;

			var consumed = false;

			var axisEvent = inputEvent as GenericControlEvent;
			if (axisEvent != null)
				consumed |= intoState.SetCurrentValue((int)XRControl.Analog0 + (int)axisEvent.controlIndex, axisEvent.value);
            
			var trackingEvent = inputEvent as TrackingEvent;
			if (trackingEvent != null)
			{
				consumed |= intoState.SetCurrentValue((int)XRControl.LocalPositionX, trackingEvent.localPosition.x);
				consumed |= intoState.SetCurrentValue((int)XRControl.LocalPositionY, trackingEvent.localPosition.y);
				consumed |= intoState.SetCurrentValue((int)XRControl.LocalPositionZ, trackingEvent.localPosition.z);

				consumed |= intoState.SetCurrentValue((int)XRControl.LocalRotationX, trackingEvent.localRotation.x);
				consumed |= intoState.SetCurrentValue((int)XRControl.LocalRotationY, trackingEvent.localRotation.y);
				consumed |= intoState.SetCurrentValue((int)XRControl.LocalRotationZ, trackingEvent.localRotation.z);
				consumed |= intoState.SetCurrentValue((int)XRControl.LocalRotationW, trackingEvent.localRotation.w);
			}

			return consumed;
		}
	}
}
