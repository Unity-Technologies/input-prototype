using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
    public class OpenVRController : TrackedController
    {
        public enum OpenVRControllerControl
        {
            Min = Trigger,

            Trigger = TrackedControllerControl.Max + 1,
            TriggerTouch,
            Grip,

            TrackpadPress,
            TrackpadTouch,
            TrackpadX,
            TrackpadY,

            VelocityX,
            VelocityY,
            VelocityZ,
            AngularVelocityX,
            AngularVelocityY,
            AngularVelocityZ,

            // Compound controls
            Trackpad,
            Velocity,
            AngularVelocity,

            Max = AngularVelocity
        }

        public ButtonInputControl trigger { get { return (ButtonInputControl)this[(int)OpenVRControllerControl.Trigger]; } }
        public ButtonInputControl triggerTouch { get { return (ButtonInputControl)this[(int)OpenVRControllerControl.TriggerTouch]; } }
        public ButtonInputControl grip { get { return (ButtonInputControl)this[(int)OpenVRControllerControl.Grip]; } }

        public ButtonInputControl trackpadPress { get { return (ButtonInputControl)this[(int)OpenVRControllerControl.TrackpadPress]; } }
        public ButtonInputControl trackpadTouch { get { return (ButtonInputControl)this[(int)OpenVRControllerControl.TrackpadTouch]; } }
        public AxisInputControl trackpadX { get { return (AxisInputControl)this[(int)OpenVRControllerControl.TrackpadX]; } }
        public AxisInputControl trackpadY { get { return (AxisInputControl)this[(int)OpenVRControllerControl.TrackpadY]; } }

        public AxisInputControl velocityX { get { return (AxisInputControl)this[(int)OpenVRControllerControl.VelocityX]; } }
        public AxisInputControl velocityY { get { return (AxisInputControl)this[(int)OpenVRControllerControl.VelocityY]; } }
        public AxisInputControl velocityZ { get { return (AxisInputControl)this[(int)OpenVRControllerControl.VelocityZ]; } }
        public AxisInputControl angularVelocityX { get { return (AxisInputControl)this[(int)OpenVRControllerControl.AngularVelocityX]; } }
        public AxisInputControl angularVelocityY { get { return (AxisInputControl)this[(int)OpenVRControllerControl.AngularVelocityY]; } }
        public AxisInputControl angularVelocityZ { get { return (AxisInputControl)this[(int)OpenVRControllerControl.AngularVelocityZ]; } }

        // Compound controls
        public Vector2InputControl trackpad { get { return (Vector2InputControl)this[(int)OpenVRControllerControl.Trackpad]; } }
        public Vector3InputControl velocity { get { return (Vector3InputControl)this[(int)OpenVRControllerControl.Velocity]; } }
        public Vector3InputControl angularVelocity { get { return (Vector3InputControl)this[(int)OpenVRControllerControl.AngularVelocity]; } }

        public OpenVRController()
            : this("OpenVRController", null) { }

        public OpenVRController(string deviceName, List<InputControlData> additionalControls)
            : base(deviceName, GetControls(additionalControls)) { }

        public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
        {
            var genericEvent = inputEvent as GenericControlEvent;
            if (genericEvent != null)
            {
                if (genericEvent.controlIndex == (int)OpenVRControllerControl.TrackpadY)
                {
                    // Invert trackpad y so that up is positive and down is negative.
                    if (intoState.SetCurrentValue(genericEvent.controlIndex, -genericEvent.value))
                        return true;
                }
            }

            var consumed = false;

            var trackingEvent = inputEvent as TrackingEvent;
            if (trackingEvent != null)
            {
                if ((trackingEvent.availableFields & TrackingEvent.Flags.VelocityAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)OpenVRControllerControl.VelocityX, trackingEvent.velocity.x);
                    consumed |= intoState.SetCurrentValue((int)OpenVRControllerControl.VelocityY, trackingEvent.velocity.y);
                    consumed |= intoState.SetCurrentValue((int)OpenVRControllerControl.VelocityZ, trackingEvent.velocity.z);
                }

                if ((trackingEvent.availableFields & TrackingEvent.Flags.AngularVelocityAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)OpenVRControllerControl.AngularVelocityX, trackingEvent.angularVelocity.x);
                    consumed |= intoState.SetCurrentValue((int)OpenVRControllerControl.AngularVelocityY, trackingEvent.angularVelocity.y);
                    consumed |= intoState.SetCurrentValue((int)OpenVRControllerControl.AngularVelocityZ, trackingEvent.angularVelocity.z);
                }
            }

            consumed |= base.ProcessEventIntoState(inputEvent, intoState);
            return consumed;
        }

        public override int GenericControlIndexFromNative(int nativeControlIndex)
        {
            switch (nativeControlIndex)
            {
                case 8:
                case 9:
                    return (int)OpenVRControllerControl.Trigger;
                case 42:
                case 43:
                    return (int)OpenVRControllerControl.TriggerTouch;
                case 10:
                case 11:
                    return (int)OpenVRControllerControl.Grip;

                case 36:
                case 37:
                    return (int)OpenVRControllerControl.TrackpadPress;
                case 44:
                case 45:
                    return (int)OpenVRControllerControl.TrackpadTouch;
                case 0:
                case 3:
                    return (int)OpenVRControllerControl.TrackpadX;
                case 1:
                case 4:
                    return (int)OpenVRControllerControl.TrackpadY;

                default:
                    return base.GenericControlIndexFromNative(nativeControlIndex);
            }
        }

        static List<InputControlData> GetControls(List<InputControlData> additionalControls)
        {
            var numControls = EnumHelpers.GetValueCount<OpenVRControllerControl>();
            var controls = Enumerable.Repeat(new InputControlData(), numControls).ToList();

            controls[IndexInControlEnum(OpenVRControllerControl.Trigger)] = new InputControlData { name = "Trigger", controlType = typeof(ButtonInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.TriggerTouch)] = new InputControlData { name = "Trigger Touch", controlType = typeof(ButtonInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.Grip)] = new InputControlData { name = "Grip", controlType = typeof(ButtonInputControl) };

            controls[IndexInControlEnum(OpenVRControllerControl.TrackpadPress)] = new InputControlData { name = "Trackpad Press", controlType = typeof(ButtonInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.TrackpadTouch)] = new InputControlData { name = "Trackpad Touch", controlType = typeof(ButtonInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.TrackpadX)] = new InputControlData { name = "Trackpad X", controlType = typeof(AxisInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.TrackpadY)] = new InputControlData { name = "Trackpad Y", controlType = typeof(AxisInputControl) };

            controls[IndexInControlEnum(OpenVRControllerControl.VelocityX)] = new InputControlData { name = "Velocity X", controlType = typeof(AxisInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.VelocityY)] = new InputControlData { name = "Velocity Y", controlType = typeof(AxisInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.VelocityZ)] = new InputControlData { name = "Velocity Z", controlType = typeof(AxisInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.AngularVelocityX)] = new InputControlData { name = "Angular Velocity X", controlType = typeof(AxisInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.AngularVelocityY)] = new InputControlData { name = "Angular Velocity Y", controlType = typeof(AxisInputControl) };
            controls[IndexInControlEnum(OpenVRControllerControl.AngularVelocityZ)] = new InputControlData { name = "Angular Velocity Z", controlType = typeof(AxisInputControl) };

            controls[IndexInControlEnum(OpenVRControllerControl.Trackpad)] = new InputControlData
            {
                name = "Trackpad",
                controlType = typeof(Vector2InputControl),
                componentControlIndices = new[] { IndexInControlEnum(OpenVRControllerControl.TrackpadX), IndexInControlEnum(OpenVRControllerControl.TrackpadY) }
            };
            controls[IndexInControlEnum(OpenVRControllerControl.Velocity)] = new InputControlData
            {
                name = "Velocity",
                controlType = typeof(Vector3InputControl),
                componentControlIndices = new[] { IndexInControlEnum(OpenVRControllerControl.VelocityX), IndexInControlEnum(OpenVRControllerControl.VelocityY), IndexInControlEnum(OpenVRControllerControl.VelocityZ) }
            };
            controls[IndexInControlEnum(OpenVRControllerControl.AngularVelocity)] = new InputControlData
            {
                name = "Angular Velocity",
                controlType = typeof(Vector3InputControl),
                componentControlIndices = new[] { IndexInControlEnum(OpenVRControllerControl.AngularVelocityX), IndexInControlEnum(OpenVRControllerControl.AngularVelocityY), IndexInControlEnum(OpenVRControllerControl.AngularVelocityZ) }
            };

            if (additionalControls != null)
                controls.AddRange(additionalControls);

            return controls;
        }

        static int IndexInControlEnum(OpenVRControllerControl control)
        {
            return (int)control - (int)OpenVRControllerControl.Min;
        }
    }
}
