using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputNew;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public VirtualJoystickControl m_ButtonControl = VirtualJoystickControl.Action1;

		bool m_Down = false;

		void Start()
		{
			m_Down = false;
		}

		public void OnPointerUp(PointerEventData data)
		{
			m_Down = false;
			VirtualJoystick.current.SetButtonValue((int)m_ButtonControl, m_Down);
		}


		public void OnPointerDown(PointerEventData data)
		{
			m_Down = true;
			VirtualJoystick.current.SetButtonValue((int)m_ButtonControl, m_Down);
		}
	}
}
