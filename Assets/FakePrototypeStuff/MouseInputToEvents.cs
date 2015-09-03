using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class MouseInputToEvents
	: MonoBehaviour
{
	bool m_Ignore = false;

	public void Update()
	{
		SendButtonEvents();
		SendMoveEvent();
	}

	void SendButtonEvents()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ())
				m_Ignore = true;
			else
				SendClickEvent(PointerControl.LeftButton, true);
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			if (m_Ignore)
				m_Ignore = false;
			else
				SendClickEvent(PointerControl.LeftButton, false);
		}
	}

	void SendMoveEvent()
	{
		if (m_Ignore)
			return;
		
		var deltaX = Input.GetAxis("Mouse X");
		var deltaY = Input.GetAxis("Mouse Y");

		// Don't send events if the mouse hasn't moved.
		if (deltaX == 0.0f && deltaY == 0.0f)
			return;

		var inputEvent = InputSystem.CreateEvent<PointerMoveEvent>();
		inputEvent.deviceType = typeof(Mouse);
		inputEvent.deviceIndex = 0;
		inputEvent.delta = new Vector3(deltaX, deltaY, 0.0f);
		inputEvent.position = Input.mousePosition;

		InputSystem.QueueEvent(inputEvent);
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
