using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class VirtualAxes : Joystick
	{
		public enum AxisControl
		{
			TranslateX,
			TranslateY,
			TranslateZ,
			Pitch,
			Roll,
			Yaw,
			Scale
		}

		public VirtualAxes(): this("Virtual Axis List") {}

		public VirtualAxes(string deviceName)
		{
			this.deviceName = deviceName;
			
			var controlCount = EnumHelpers.GetValueCount<AxisControl>();
			var controls = new List<InputControlData>(controlCount);
			for (int i = 0; i < controlCount; i++)
				controls.Add(new InputControlData());
			
			// Axes.
			controls[(int)AxisControl.TranslateX] = new InputControlData { name = "Translate X", controlType = typeof(AxisInputControl) };
			controls[(int)AxisControl.TranslateY] = new InputControlData { name = "Translate Y", controlType = typeof(AxisInputControl) };
			controls[(int)AxisControl.TranslateZ] = new InputControlData { name = "Translate Z", controlType = typeof(AxisInputControl) };
			controls[(int)AxisControl.Pitch] = new InputControlData { name = "Pitch", controlType = typeof(AxisInputControl) };
			controls[(int)AxisControl.Roll] = new InputControlData { name = "Roll", controlType = typeof(AxisInputControl) };
			controls[(int)AxisControl.Yaw] = new InputControlData { name = "Yaw", controlType = typeof(AxisInputControl) };
			controls[(int)AxisControl.Scale] = new InputControlData { name = "Scale", controlType = typeof(AxisInputControl) };
			
			SetControls(controls);
		}

		public static VirtualAxes current { get { return InputSystem.GetMostRecentlyUsedDevice<VirtualAxes>(); } }
		
		public void SetAxisValue(int controlIndex, float value)
		{
			var control = this[controlIndex] as InputControl;
			if (control == null)
				return;
			float currentValue = control.rawValue;
			if (value == currentValue && value != 0f)
				return;
			
			var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
			inputEvent.deviceType = typeof(VirtualAxes);
			inputEvent.deviceIndex = 0; 
			inputEvent.controlIndex = controlIndex;
			inputEvent.value = value;
			InputSystem.QueueEvent(inputEvent);
		}
	}
}
