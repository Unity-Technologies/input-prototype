using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputNew;
using Random = UnityEngine.Random;
using UnityEngine.Serialization;

public class CharacterInputController
	: MonoBehaviour
{
	FirstPersonControls m_PlayerInput;
	Rigidbody m_Rigid;
	Vector2 m_Rotation = Vector2.zero;
	float m_TimeOfLastShot;

	[FormerlySerializedAs("controlMap")]
	public ActionMap actionMap;

	[Space(10)]
	public Transform head;
	public float moveSpeed = 5;
	public GameObject projectile;
	public float timeBetweenShots = 0.5f;
	public CubeSizer sizer;
	public Text controlsText;

	public void Awake()
	{
		m_PlayerInput = InputSystem.CreatePlayer<FirstPersonControls>(actionMap);
		sizer.referencePlayerInput = m_PlayerInput;
		m_PlayerInput.Activate();
		m_Rigid = GetComponent<Rigidbody>();
		
		LockCursor(true);
	}
	
	public void SetupPlayer(FirstPersonControls playerInput)
	{
		if (m_PlayerInput != null)
			m_PlayerInput.Deactivate();
		m_PlayerInput = playerInput;
		sizer.referencePlayerInput = m_PlayerInput;
	}

	public void Update()
	{
		// Move
		var moveX = m_PlayerInput.moveX.value;
		var moveY = m_PlayerInput.moveY.value;

		Vector3 velocity = transform.TransformDirection(new Vector3(moveX, 0, moveY)) * moveSpeed;
		m_Rigid.velocity = new Vector3(velocity.x, m_Rigid.velocity.y, velocity.z);

		// Look
		var lookX = m_PlayerInput.lookX.value;
		var lookY = m_PlayerInput.lookY.value;

		m_Rotation.y += lookX;
		m_Rotation.x = Mathf.Clamp(m_Rotation.x - lookY, -89, 89);

		transform.localEulerAngles = new Vector3(0, m_Rotation.y, 0);
		head.localEulerAngles = new Vector3(m_Rotation.x, 0, 0);

		// Fire
		var fire = m_PlayerInput.fire.button;
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

		if (m_PlayerInput.lockCursor.buttonDown)
			LockCursor(true);

		if (m_PlayerInput.unlockCursor.buttonDown)
			LockCursor(false);
		
		if (m_PlayerInput.menu.buttonDown)
			sizer.ToggleMenu();
		
		HandleControlsText();
	}
	
	void HandleControlsText()
	{
		string help = string.Empty;
		
		help += GetControlHelp(m_PlayerInput.moveX) + "\n";
		help += GetControlHelp(m_PlayerInput.moveY) + "\n";
		help += GetControlHelp(m_PlayerInput.lookX) + "\n";
		help += GetControlHelp(m_PlayerInput.lookY) + "\n";
		help += GetControlHelp(m_PlayerInput.fire) + "\n";
		help += GetControlHelp(m_PlayerInput.menu);
		controlsText.text = help;
	}
	
	private string GetControlHelp(InputControl control)
	{
		return string.Format("Use {0} to {1}!", control.GetPrimarySourceName(), control.name);
	}

	void Fire()
	{
		var newProjectile = Instantiate(projectile);
		newProjectile.transform.position = head.position + head.forward * 0.6f;
		newProjectile.transform.rotation = head.rotation;
		float size = (sizer == null ? 1 : sizer.size);
		newProjectile.transform.localScale *= size;
		newProjectile.GetComponent<Rigidbody>().mass = Mathf.Pow(size, 3);
		newProjectile.GetComponent<Rigidbody>().AddForce(head.forward * 20f, ForceMode.Impulse);
		newProjectile.GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
	}

	void LockCursor(bool value)
	{
		var mouse = Mouse.current;
		if (mouse != null)
			mouse.cursor.isLocked = value;
	}
}
