using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputNew;

public class CharacterInputController
	: MonoBehaviour
{
	ControlMapInstance m_ControlMapInstance;
	float m_LastLookX;
	float m_LastLookY;
	Rigidbody m_Rigid;
	Vector2 m_Rotation = Vector2.zero;
	float m_TimeOfLastShot;

	public ControlMap controlMap;

	[Space(10)]
	public ControlMapEntry moveControlX;
	public ControlMapEntry moveControlY;
	public ControlMapEntry lookControlX;
	public ControlMapEntry lookControlY;
	public ControlMapEntry fireControl;

	[Space(10)]
	public Transform head;
	public float moveSpeed = 5;
	public GameObject projectile;
	public float timeBetweenShots = 0.5f;

	public void Awake()
	{
		m_ControlMapInstance = InputSystem.BindInputs(controlMap).First();
		m_ControlMapInstance.Activate();
		m_Rigid = GetComponent<Rigidbody>();
	}

	public void Update()
	{
		// Move
		if (moveControlX)
		{
			var moveX = m_ControlMapInstance[moveControlX].floatValue;
			var moveY = m_ControlMapInstance[moveControlY].floatValue;

			m_Rigid.velocity = transform.TransformDirection(new Vector3(moveX, 0, moveY)) * moveSpeed;
		}

		// Look
		var lookX = m_ControlMapInstance[lookControlX].floatValue;
		var lookY = m_ControlMapInstance[lookControlY].floatValue;

		// HACK UNTIL MOUSE IS RELATIVE
		lookX = lookX - m_LastLookX;
		lookY = lookY - m_LastLookY;
		m_LastLookX = lookX + m_LastLookX;
		m_LastLookY = lookY + m_LastLookY;

		m_Rotation.y += lookX;
		transform.localEulerAngles = new Vector3(0, m_Rotation.y, 0);

		m_Rotation.x = Mathf.Clamp(m_Rotation.x - lookY, -89, 89);
		head.localEulerAngles = new Vector3(m_Rotation.x, 0, 0);

		// Fire
		var fire = m_ControlMapInstance[fireControl].boolValue;
		if (fire)
		{
			var currentTime = Time.time;
			var timeElapsedSinceLastShot = currentTime - m_TimeOfLastShot;
			if (timeElapsedSinceLastShot > timeBetweenShots)
			{
				m_TimeOfLastShot = currentTime;
				Fire();
			}
		}
	}

	void Fire()
	{
		var newProjectile = Instantiate(projectile);
		newProjectile.transform.position = head.position + head.forward * 0.6f;
		newProjectile.transform.rotation = head.rotation;
		newProjectile.GetComponent<Rigidbody>().AddForce(head.forward * 20f, ForceMode.Impulse);
	}
}
