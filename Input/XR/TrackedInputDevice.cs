using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
    public abstract class TrackedInputDevice : XRInputDevice
    {
        public enum TrackedInputDeviceControl
        {
            PositionX = 0,
            PositionY,
            PositionZ,

            RotationX,
            RotationY,
            RotationZ,
            RotationW,

            // Compound controls
            Position,
            Rotation,

            Max = Rotation
        }

        public AxisInputControl positionX { get { return (AxisInputControl)this[(int)TrackedInputDeviceControl.PositionX]; } }
        public AxisInputControl positionY { get { return (AxisInputControl)this[(int)TrackedInputDeviceControl.PositionY]; } }
        public AxisInputControl positionZ { get { return (AxisInputControl)this[(int)TrackedInputDeviceControl.PositionZ]; } }

        public AxisInputControl rotationX { get { return (AxisInputControl)this[(int)TrackedInputDeviceControl.RotationX]; } }
        public AxisInputControl rotationY { get { return (AxisInputControl)this[(int)TrackedInputDeviceControl.RotationY]; } }
        public AxisInputControl rotationZ { get { return (AxisInputControl)this[(int)TrackedInputDeviceControl.RotationZ]; } }
        public AxisInputControl rotationW { get { return (AxisInputControl)this[(int)TrackedInputDeviceControl.RotationW]; } }

        // Compound controls
        public Vector3InputControl position { get { return (Vector3InputControl)this[(int)TrackedInputDeviceControl.Position]; } }
        public QuaternionInputControl rotation { get { return (QuaternionInputControl)this[(int)TrackedInputDeviceControl.Rotation]; } }

        protected TrackedInputDevice()
            : this("TrackedInputDevice", null) { }

        protected TrackedInputDevice(string deviceName, List<InputControlData> additionalControls)
        {
            this.deviceName = deviceName;
            var numControls = EnumHelpers.GetValueCount<TrackedInputDeviceControl>();
            var controls = Enumerable.Repeat(new InputControlData(), numControls).ToList();

            controls[(int)TrackedInputDeviceControl.PositionX] = new InputControlData { name = "Position X", controlType = typeof(AxisInputControl) };
            controls[(int)TrackedInputDeviceControl.PositionY] = new InputControlData { name = "Position Y", controlType = typeof(AxisInputControl) };
            controls[(int)TrackedInputDeviceControl.PositionZ] = new InputControlData { name = "Position Z", controlType = typeof(AxisInputControl) };

            controls[(int)TrackedInputDeviceControl.RotationX] = new InputControlData { name = "Rotation X", controlType = typeof(AxisInputControl) };
            controls[(int)TrackedInputDeviceControl.RotationY] = new InputControlData { name = "Rotation Y", controlType = typeof(AxisInputControl) };
            controls[(int)TrackedInputDeviceControl.RotationZ] = new InputControlData { name = "Rotation Z", controlType = typeof(AxisInputControl) };
            controls[(int)TrackedInputDeviceControl.RotationW] = new InputControlData { name = "Rotation W", controlType = typeof(AxisInputControl) };

            controls[(int)TrackedInputDeviceControl.Position] = new InputControlData
            {
                name = "Position",
                controlType = typeof(Vector3InputControl),
                componentControlIndices = new[] { (int)TrackedInputDeviceControl.PositionX, (int)TrackedInputDeviceControl.PositionY, (int)TrackedInputDeviceControl.PositionZ }
            };
            controls[(int)TrackedInputDeviceControl.Rotation] = new InputControlData
            {
                name = "Rotation",
                controlType = typeof(QuaternionInputControl),
                componentControlIndices = new[] { (int)TrackedInputDeviceControl.RotationX, (int)TrackedInputDeviceControl.RotationY, (int)TrackedInputDeviceControl.RotationZ, (int)TrackedInputDeviceControl.RotationW }
            };

            if (additionalControls != null)
                controls.AddRange(additionalControls);

            SetControls(controls);
        }

        public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
        {
            if (base.ProcessEventIntoState(inputEvent, intoState))
                return true;

            var consumed = false;

            var trackingEvent = inputEvent as TrackingEvent;
            if (trackingEvent != null)
            {
                if ((trackingEvent.availableFields & TrackingEvent.Flags.PositionAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)TrackedInputDeviceControl.PositionX, trackingEvent.localPosition.x);
                    consumed |= intoState.SetCurrentValue((int)TrackedInputDeviceControl.PositionY, trackingEvent.localPosition.y);
                    consumed |= intoState.SetCurrentValue((int)TrackedInputDeviceControl.PositionZ, trackingEvent.localPosition.z);
                }

                if ((trackingEvent.availableFields & TrackingEvent.Flags.OrientationAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)TrackedInputDeviceControl.RotationX, trackingEvent.localRotation.x);
                    consumed |= intoState.SetCurrentValue((int)TrackedInputDeviceControl.RotationY, trackingEvent.localRotation.y);
                    consumed |= intoState.SetCurrentValue((int)TrackedInputDeviceControl.RotationZ, trackingEvent.localRotation.z);
                    consumed |= intoState.SetCurrentValue((int)TrackedInputDeviceControl.RotationW, trackingEvent.localRotation.w);
                }
            }

            return consumed;
        }
    }
}
