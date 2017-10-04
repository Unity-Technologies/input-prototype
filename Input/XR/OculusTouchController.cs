using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
    public class OculusTouchController : TrackedController
    {
        public enum OculusTouchControllerControl
        {
            Action1Touch = TrackedControllerControl.Action1 + 1,
            Action2,
            Action2Touch,

            Trigger,
            TriggerTouch,
            TriggerNearTouch,
            HandTrigger,

            ThumbRestTouch,
            ThumbNearTouch,

            Start,

            StickPress,
            StickTouch,
            StickX,
            StickY,

            AccelerationX,
            AccelerationY,
            AccelerationZ,
            AngularAccelerationX,
            AngularAccelerationY,
            AngularAccelerationZ,
            VelocityX,
            VelocityY,
            VelocityZ,
            AngularVelocityX,
            AngularVelocityY,
            AngularVelocityZ,

            // Compound controls
            Stick,
            Acceleration,
            AngularAcceleration,
            Velocity,
            AngularVelocity,
        }

        // TODO: Inherit tags
        private static readonly string[] kTags = { "Left", "Right" };
        public new static string[] Tags
        {
            get
            {
                return kTags;
            }
        }

        public ButtonInputControl action1Touch { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.Action1Touch]; } }
        public ButtonInputControl action2 { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.Action2]; } }
        public ButtonInputControl action2Touch { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.Action2Touch]; } }

        public ButtonInputControl trigger { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.Trigger]; } }
        public ButtonInputControl triggerTouch { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.TriggerTouch]; } }
        public ButtonInputControl triggerNearTouch { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.TriggerNearTouch]; } }
        public ButtonInputControl handTrigger { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.HandTrigger]; } }

        public ButtonInputControl thumbRestTouch { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.ThumbRestTouch]; } }
        public ButtonInputControl thumbNearTouch { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.ThumbNearTouch]; } }

        public ButtonInputControl start { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.Start]; } }

        public ButtonInputControl stickPress { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.StickPress]; } }
        public ButtonInputControl stickTouch { get { return (ButtonInputControl)this[(int)OculusTouchControllerControl.StickTouch]; } }
        public AxisInputControl stickX { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.StickX]; } }
        public AxisInputControl stickY { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.StickY]; } }

        public AxisInputControl accelerationX { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AccelerationX]; } }
        public AxisInputControl accelerationY { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AccelerationY]; } }
        public AxisInputControl accelerationZ { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AccelerationZ]; } }
        public AxisInputControl angularAccelerationX { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AngularAccelerationX]; } }
        public AxisInputControl angularAccelerationY { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AngularAccelerationY]; } }
        public AxisInputControl angularAccelerationZ { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AngularAccelerationZ]; } }
        public AxisInputControl velocityX { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.VelocityX]; } }
        public AxisInputControl velocityY { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.VelocityY]; } }
        public AxisInputControl velocityZ { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.VelocityZ]; } }
        public AxisInputControl angularVelocityX { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AngularVelocityX]; } }
        public AxisInputControl angularVelocityY { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AngularVelocityY]; } }
        public AxisInputControl angularVelocityZ { get { return (AxisInputControl)this[(int)OculusTouchControllerControl.AngularVelocityZ]; } }

        // Compound controls
        public Vector2InputControl stick { get { return (Vector2InputControl)this[(int)OculusTouchControllerControl.Stick]; } }
        public Vector3InputControl acceleration { get { return (Vector3InputControl)this[(int)OculusTouchControllerControl.Acceleration]; } }
        public Vector3InputControl angularAcceleration { get { return (Vector3InputControl)this[(int)OculusTouchControllerControl.AngularAcceleration]; } }
        public Vector3InputControl velocity { get { return (Vector3InputControl)this[(int)OculusTouchControllerControl.Velocity]; } }
        public Vector3InputControl angularVelocity { get { return (Vector3InputControl)this[(int)OculusTouchControllerControl.AngularVelocity]; } }

        public OculusTouchController()
            : this("OculusTouchController", null) { }

        public OculusTouchController(string deviceName, List<InputControlData> additionalControls)
            : base(deviceName, GetControls(additionalControls)) { }

        public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
        {
            var consumed = false;

            var trackingEvent = inputEvent as TrackingEvent;
            if (trackingEvent != null)
            {
                if ((trackingEvent.availableFields & TrackingEvent.Flags.AccelerationAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AccelerationX, trackingEvent.acceleration.x);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AccelerationY, trackingEvent.acceleration.y);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AccelerationZ, trackingEvent.acceleration.z);
                }

                if ((trackingEvent.availableFields & TrackingEvent.Flags.AngularAccelerationAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AngularAccelerationX, trackingEvent.angularAcceleration.x);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AngularAccelerationY, trackingEvent.angularAcceleration.y);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AngularAccelerationZ, trackingEvent.angularAcceleration.z);
                }

                if ((trackingEvent.availableFields & TrackingEvent.Flags.VelocityAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.VelocityX, trackingEvent.velocity.x);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.VelocityY, trackingEvent.velocity.y);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.VelocityZ, trackingEvent.velocity.z);
                }

                if ((trackingEvent.availableFields & TrackingEvent.Flags.AngularVelocityAvailable) != 0)
                {
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AngularVelocityX, trackingEvent.angularVelocity.x);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AngularVelocityY, trackingEvent.angularVelocity.y);
                    consumed |= intoState.SetCurrentValue((int)OculusTouchControllerControl.AngularVelocityZ, trackingEvent.angularVelocity.z);
                }
            }

            consumed |= base.ProcessEventIntoState(inputEvent, intoState);
            return consumed;
        }

        public override int GenericControlIndexFromNative(int nativeControlIndex)
        {
            switch (nativeControlIndex)
            {
                case 38:
                case 40:
                    return (int)OculusTouchControllerControl.Action1Touch;
                case 29:
                case 31:
                    return (int)OculusTouchControllerControl.Action2;
                case 39:
                case 41:
                    return (int)OculusTouchControllerControl.Action2Touch;

                case 8:
                case 9:
                    return (int)OculusTouchControllerControl.Trigger;
                case 42:
                case 43:
                    return (int)OculusTouchControllerControl.TriggerTouch;
                case 12:
                case 13:
                    return (int)OculusTouchControllerControl.TriggerNearTouch;
                case 10:
                case 11:
                    return (int)OculusTouchControllerControl.HandTrigger;

                case 46:
                case 47:
                    return (int)OculusTouchControllerControl.ThumbRestTouch;
                case 14:
                case 15:
                    return (int)OculusTouchControllerControl.ThumbNearTouch;

                case 35:
                    return (int)OculusTouchControllerControl.Start;

                case 36:
                case 37:
                    return (int)OculusTouchControllerControl.StickPress;
                case 44:
                case 45:
                    return (int)OculusTouchControllerControl.StickTouch;
                case 0:
                case 3:
                    return (int)OculusTouchControllerControl.StickX;
                case 1:
                case 4:
                    return (int)OculusTouchControllerControl.StickY;

                default:
                    return base.GenericControlIndexFromNative(nativeControlIndex);
            }
        }

        static List<InputControlData> GetControls(List<InputControlData> additionalControls)
        {
            var numControls = EnumHelpers.GetValueCount<OculusTouchControllerControl>();
            var controls = Enumerable.Repeat(new InputControlData(), numControls).ToList();

            // TODO: Better solution than subtracting lowest enum value
            controls[(int)OculusTouchControllerControl.Action1Touch - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Action 1 Touch", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.Action2 - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Action 2", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.Action2Touch - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Action 2 Touch", controlType = typeof(ButtonInputControl) };

            controls[(int)OculusTouchControllerControl.Trigger - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Trigger", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.TriggerTouch - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Trigger Touch", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.TriggerNearTouch - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Trigger Near Touch", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.HandTrigger - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Hand Trigger", controlType = typeof(ButtonInputControl) };

            controls[(int)OculusTouchControllerControl.ThumbRestTouch - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Thumb Rest Touch", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.ThumbNearTouch - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Thumb Near Touch", controlType = typeof(ButtonInputControl) };

            controls[(int)OculusTouchControllerControl.Start - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Start", controlType = typeof(ButtonInputControl) };

            controls[(int)OculusTouchControllerControl.StickPress - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Stick Press", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.StickTouch - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Stick Touch", controlType = typeof(ButtonInputControl) };
            controls[(int)OculusTouchControllerControl.StickX - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Stick X", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.StickY - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Stick Y", controlType = typeof(AxisInputControl) };

            controls[(int)OculusTouchControllerControl.AccelerationX - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Acceleration X", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AccelerationY - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Acceleration Y", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AccelerationZ - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Acceleration Z", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AngularAccelerationX - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Angular Acceleration X", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AngularAccelerationY - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Angular Acceleration Y", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AngularAccelerationZ - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Angular Acceleration Z", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.VelocityX - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Velocity X", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.VelocityY - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Velocity Y", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.VelocityZ - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Velocity Z", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AngularVelocityX - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Angular Velocity X", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AngularVelocityY - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Angular Velocity Y", controlType = typeof(AxisInputControl) };
            controls[(int)OculusTouchControllerControl.AngularVelocityZ - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData { name = "Angular Velocity Z", controlType = typeof(AxisInputControl) };

            controls[(int)OculusTouchControllerControl.Stick - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData
            {
                name = "Stick",
                controlType = typeof(Vector2InputControl),
                componentControlIndices = new[] { (int)OculusTouchControllerControl.StickX, (int)OculusTouchControllerControl.StickY }
            };
            controls[(int)OculusTouchControllerControl.Acceleration - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData
            {
                name = "Acceleration",
                controlType = typeof(Vector3InputControl),
                componentControlIndices = new[] { (int)OculusTouchControllerControl.AccelerationX, (int)OculusTouchControllerControl.AccelerationY, (int)OculusTouchControllerControl.AccelerationZ }
            };
            controls[(int)OculusTouchControllerControl.AngularAcceleration - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData
            {
                name = "Angular Acceleration",
                controlType = typeof(Vector3InputControl),
                componentControlIndices = new[] { (int)OculusTouchControllerControl.AngularAccelerationX, (int)OculusTouchControllerControl.AngularAccelerationY, (int)OculusTouchControllerControl.AngularAccelerationZ }
            };
            controls[(int)OculusTouchControllerControl.Velocity - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData
            {
                name = "Velocity",
                controlType = typeof(Vector3InputControl),
                componentControlIndices = new[] { (int)OculusTouchControllerControl.VelocityX, (int)OculusTouchControllerControl.VelocityY, (int)OculusTouchControllerControl.VelocityZ }
            };
            controls[(int)OculusTouchControllerControl.AngularVelocity - (int)OculusTouchControllerControl.Action1Touch] = new InputControlData
            {
                name = "Angular Velocity",
                controlType = typeof(Vector3InputControl),
                componentControlIndices = new[] { (int)OculusTouchControllerControl.AngularVelocityX, (int)OculusTouchControllerControl.AngularVelocityY, (int)OculusTouchControllerControl.AngularVelocityZ }
            };

            if (additionalControls != null)
                controls.AddRange(additionalControls);

            return controls;
        }
    }
}
