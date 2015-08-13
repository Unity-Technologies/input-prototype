using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class MouseInputToEvents
	: MonoBehaviour
{
	Vector3 m_LastMousePosition;

	public void Update()
	{
		SendButtonEvents();
		SendMoveEvent();
	}

	void SendButtonEvents()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
			SendClickEvent(PointerControl.LeftButton, true);
		if (Input.GetKeyUp(KeyCode.Mouse0))
			SendClickEvent(PointerControl.LeftButton, false);
	}

	void SendMoveEvent()
	{
		var newMousePosition = Input.mousePosition;
		if (newMousePosition == m_LastMousePosition)
			return;

		var inputEvent = InputSystem.CreateEvent<PointerMoveEvent>();
		inputEvent.deviceType = typeof(Mouse);
		inputEvent.deviceIndex = 0;
		inputEvent.delta = newMousePosition - m_LastMousePosition;
		inputEvent.position = newMousePosition;

		InputSystem.QueueEvent(inputEvent);

		m_LastMousePosition = newMousePosition;
	}

	void SendClickEvent(PointerControl controlIndex, bool clicked)
	{
		////REVIEW: should this be a pointer-specific event type?
		var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
		inputEvent.deviceType = typeof(Mouse);
		inputEvent.deviceIndex = 0;
		inputEvent.controlIndex = (int)controlIndex;
		inputEvent.value = clicked ? 1.0f : 0.0f;
		InputSystem.QueueEvent(inputEvent);
	}
}
