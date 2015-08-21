using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputNew;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class VirtualStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		public int m_MovementRange = 100;
		public AxisOption m_AxesToUse = AxisOption.Both; // The options for the axes that the still will use
		public VirtualJoystickControl m_HorizontalControl = VirtualJoystickControl.LeftStickX;
		public VirtualJoystickControl m_VerticalControl = VirtualJoystickControl.LeftStickY;

		Vector3 m_StartPos;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis

		void OnEnable()
		{
			CreateVirtualAxes();
		}

		void Start()
		{
			m_StartPos = transform.position;
		}

		void UpdateVirtualAxes(Vector3 delta)
		{
			if (m_UseX)
				InputSystem.virtualJoystick.SetAxisValue((int)m_HorizontalControl, delta.x);

			if (m_UseY)
				InputSystem.virtualJoystick.SetAxisValue((int)m_VerticalControl, delta.y);
		}

		void CreateVirtualAxes()
		{
			// Set axes to use
			m_UseX = (m_AxesToUse == AxisOption.Both || m_AxesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (m_AxesToUse == AxisOption.Both || m_AxesToUse == AxisOption.OnlyVertical);
		}

		public void OnDrag(PointerEventData data)
		{
			Vector2 delta = data.position - (Vector2)m_StartPos;

			if (!m_UseX)
				delta.x = 0;

			if (!m_UseY)
				delta.y = 0;

			delta = Vector2.ClampMagnitude(delta, m_MovementRange);

			transform.position = m_StartPos + (Vector3)delta;
			UpdateVirtualAxes(delta / m_MovementRange);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			UpdateVirtualAxes(Vector2.zero);
		}


		public void OnPointerDown(PointerEventData data) { }
	}
}
