using System.Collections.Generic;
using System.Linq;
using Assets.Utilities;

namespace UnityEngine.InputNew
{
    public class TrackedController : TrackedInputDevice
    {
        public enum TrackedControllerControl
        {
            Min = Action1,

            Action1 = TrackedInputDeviceControl.Max + 1,

            Max = Action1
        }

        public enum Handedness
        {
            Left,
            Right
        }

        static readonly string[] k_Tags = { "Left", "Right" };

        public static string[] tags
        {
            get
            {
                return k_Tags;
            }
        }

        public Handedness? hand { get; set; }

        public override int tagIndex
        {
            get
            {
                if (hand.HasValue)
                    return (int)hand.Value;
                return -1;
            }
        }

        public ButtonInputControl action1 { get { return (ButtonInputControl)this[(int)TrackedControllerControl.Action1]; } }

        public TrackedController()
            : this("TrackedController", null) { }

        public TrackedController(string deviceName, List<InputControlData> additionalControls)
            : base(deviceName, GetControls(additionalControls)) { }

        public override int GenericControlIndexFromNative(int nativeControlIndex)
        {
            switch (nativeControlIndex)
            {
                case 28:
                case 30:
                    return (int)TrackedControllerControl.Action1;
                default:
                    return -1;
            }
        }

        static List<InputControlData> GetControls(List<InputControlData> additionalControls)
        {
            var numControls = EnumHelpers.GetValueCount<TrackedControllerControl>();
            var controls = Enumerable.Repeat(new InputControlData(), numControls).ToList();

            controls[IndexInControlEnum(TrackedControllerControl.Action1)] = new InputControlData { name = "Action 1", controlType = typeof(ButtonInputControl) };

            if (additionalControls != null)
                controls.AddRange(additionalControls);

            return controls;
        }

        static int IndexInControlEnum(TrackedControllerControl control)
        {
            return (int)control - (int)TrackedControllerControl.Min;
        }
    }
}
