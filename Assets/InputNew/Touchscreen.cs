using System;
using System.Collections.Generic;
using UnityEngine;

////REVIEW: should there be PointerMoveEvents for touches?

namespace UnityEngine.InputNew
{
	public class Touchscreen
		: Pointer
	{
		#region Constructors

		public Touchscreen(string deviceName, List<InputControlData> controls)
			: base(deviceName, controls) { }

		#endregion

		#region Public Methods

		public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
		{
			var consumed = false;

			var touchEvent = inputEvent as TouchEvent;
			if (touchEvent != null)
			{
				consumed |= intoState.SetCurrentValue((int)GetControlForFinger(touchEvent.touch.fingerId, TouchControl.Touch0PositionX), touchEvent.touch.position.x);
				consumed |= intoState.SetCurrentValue((int)GetControlForFinger(touchEvent.touch.fingerId, TouchControl.Touch0PositionY), touchEvent.touch.position.y);
				consumed |= intoState.SetCurrentValue((int)GetControlForFinger(touchEvent.touch.fingerId, TouchControl.Touch0DeltaX), touchEvent.touch.delta.x);
				consumed |= intoState.SetCurrentValue((int)GetControlForFinger(touchEvent.touch.fingerId, TouchControl.Touch0DeltaY), touchEvent.touch.delta.y);

				// If we currently don't have a pointer finger and this is a finger-down event,
				// make the finger the current pointer.
				if (m_CurrentPointerFingerId == 0 && touchEvent.touch.phase == TouchPhase.Began)
					m_CurrentPointerFingerId = touchEvent.touch.fingerId;

				////TODO: set pointer button state from touches

				// If the finger is the current pointer, update the pointer state.
				if (m_CurrentPointerFingerId == touchEvent.touch.fingerId)
				{
					consumed |= intoState.SetCurrentValue((int)PointerControl.PositionX, touchEvent.touch.position.x);
					consumed |= intoState.SetCurrentValue((int)PointerControl.PositionY, touchEvent.touch.position.y);
					consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaX, touchEvent.touch.delta.x);
					consumed |= intoState.SetCurrentValue((int)PointerControl.DeltaY, touchEvent.touch.delta.y);

					if (touchEvent.touch.phase == TouchPhase.Ended)
						m_CurrentPointerFingerId = 0;
				}

				// Store complete touch state for finger.
				var touchIndex = GetTouchIndexForFinger(touchEvent.touch.fingerId);
				m_Touches[touchIndex] = touchEvent.touch;
			}

			if (consumed)
				return true;

			return base.ProcessEventIntoState(inputEvent, intoState);
		}

		public static Touchscreen CreateDefault()
		{
			var controls = CreateDefaultControls();

			for (var i = 0; i < MaxConcurrentTouches; ++i)
				AddControlsForTouch(i, controls);
			
			return new Touchscreen("Generic Touchscreen", controls);
		}

		#endregion

		#region Non-Public Methods

		private static void AddControlsForTouch(int index, List<InputControlData> controls)
		{
			var prefix = "Touch" + index;

			var positionIndex = controls.Count;
			controls.Add(new InputControlData
			{
				  name = prefix + "Position"
				, componentControlIndices = new[] { positionIndex + 1, positionIndex + 2 }
			});
			controls.Add(new InputControlData
			{
				  name = prefix + "PositionX"
				, controlType = InputControlType.AbsoluteAxis
			});
			controls.Add(new InputControlData
			{
				  name = prefix + "PositionY"
				, controlType = InputControlType.AbsoluteAxis
			});

			var deltaIndex = controls.Count;
			controls.Add(new InputControlData
			{
				  name = prefix + "Delta"
				, componentControlIndices = new[] { deltaIndex + 1, deltaIndex + 2 }
			});
			controls.Add(new InputControlData
			{
				  name = prefix + "DeltaX"
				, controlType = InputControlType.AbsoluteAxis
			});
			controls.Add(new InputControlData
			{
				  name = prefix + "DeltaY"
				, controlType = InputControlType.AbsoluteAxis
			});
		}

		private static TouchControl GetControlForFinger(int fingerId, TouchControl control)
		{
			return (TouchControl)((int)control - (int)TouchControl.Touch0Position
				+ (fingerId * TouchControlConstants.ControlsPerTouch)
				+ TouchControl.Touch0Position);
		}

		private int GetTouchIndexForFinger(int fingerId)
		{
			for (var i = 0; i < m_Touches.Count; ++i)
				if (m_Touches[i].fingerId == fingerId)
					return i;

			m_Touches.Add(new Touch());
			return m_Touches.Count - 1;
		}

		#endregion

		#region Public Properties

		////REVIEW: this needs to be readonly, really
		public List<Touch> touches
		{
			get { return m_Touches; }
		}

		////REVIEW: this needs to be dynamic in the real thing
		public const int MaxConcurrentTouches = 5;

		#endregion

		#region Fields

		private List<Touch> m_Touches = new List<Touch>(MaxConcurrentTouches);
		private int m_CurrentPointerFingerId;

		#endregion
	}
}
