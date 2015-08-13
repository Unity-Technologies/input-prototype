using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	/// <summary>
	///     A device that can point at and click on things.
	/// </summary>
	public abstract class Pointer
		: InputDevice
	{
		#region Constructors

		protected Pointer(string deviceName, List<InputControlData> controls)
			: base(deviceName, controls) { }

		#endregion

		#region Public Methods

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			if (base.ProcessEventIntoState(inputEvent, intoState))
				return true;

			var consumed = false;

			var moveEvent = inputEvent as PointerMoveEvent;
			if (moveEvent != null)
			{
				consumed |= intoState.SetCurrentValue((int)PointerControl.PositionX, moveEvent.position.x);
				consumed |= intoState.SetCurrentValue((int)PointerControl.PositionY, moveEvent.position.y);
				consumed |= intoState.SetCurrentValue((int)PointerControl.PositionZ, moveEvent.position.z);

				consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaX, moveEvent.delta.x);
				consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaY, moveEvent.delta.y);
				consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaZ, moveEvent.delta.z);

				return consumed;
			}

			var clickEvent = inputEvent as GenericControlEvent;
			if (clickEvent != null)
			{
				consumed |= intoState.SetCurrentValue((int)PointerControl.LeftButton, clickEvent.value);

				return consumed;
			}

			return false;
		}

		#endregion

		#region Non-Public Methods

		protected static List<InputControlData> CreateDefaultControls()
		{
			var controls = new List<InputControlData>();

			controls.Add(item: new InputControlData
			{
				name = "Position"
				, controlType = InputControlType.Vector3
				, componentControlIndices = new[] { (int)PointerControl.PositionX, (int)PointerControl.PositionY, (int)PointerControl.PositionZ }
			});

			controls.Add(new InputControlData { name = "PositionX", controlType = InputControlType.AbsoluteAxis });
			controls.Add(new InputControlData { name = "PositionY", controlType = InputControlType.AbsoluteAxis });
			controls.Add(new InputControlData { name = "PositionZ", controlType = InputControlType.AbsoluteAxis });

			controls.Add(item: new InputControlData
			{
				name = "Delta"
				, controlType = InputControlType.Vector3
				, componentControlIndices = new[] { (int)PointerControl.DeltaX, (int)PointerControl.DeltaY, (int)PointerControl.DeltaZ }
			});

			controls.Add(new InputControlData { name = "DeltaX", controlType = InputControlType.RelativeAxis });
			controls.Add(new InputControlData { name = "DeltaY", controlType = InputControlType.RelativeAxis });
			controls.Add(new InputControlData { name = "DeltaZ", controlType = InputControlType.RelativeAxis });
			controls.Add(new InputControlData { name = "Pressure", controlType = InputControlType.AbsoluteAxis });
			controls.Add(new InputControlData { name = "Tilt", controlType = InputControlType.AbsoluteAxis });
			controls.Add(new InputControlData { name = "Rotation", controlType = InputControlType.AbsoluteAxis });
			controls.Add(new InputControlData { name = "LeftButton", controlType = InputControlType.Button });
			controls.Add(new InputControlData { name = "RightButton", controlType = InputControlType.Button });
			controls.Add(new InputControlData { name = "MiddleButton", controlType = InputControlType.Button });

			return controls;
		}

		#endregion

		#region Public Properties

		public Vector3 position
		{
			get { return state[(int)PointerControl.Position].vector3; }
		}

		public float pressure
		{
			get { return state[(int)PointerControl.Pressure].value; }
		}

		#endregion
	}
}
