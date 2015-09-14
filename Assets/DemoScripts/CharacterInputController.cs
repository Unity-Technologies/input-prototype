using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputNew;
using Random = UnityEngine.Random;

public class CharacterInputController
	: MonoBehaviour
{
	ControlMapInstance m_ControlMapInstance;
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
	public ControlMapEntry menuControl;
	public ControlMapEntry lockCursorControl;
	public ControlMapEntry unlockCursorControl;

	[Space(10)]
	public Transform head;
	public float moveSpeed = 5;
	public GameObject projectile;
	public float timeBetweenShots = 0.5f;
	public CubeSizer sizer;
	public Text controlsText;

	public void Awake()
	{
		m_ControlMapInstance = new ControlMapCombinedInstance(controlMap);
		m_ControlMapInstance.Activate();
		m_Rigid = GetComponent<Rigidbody>();
		
		LockCursor();
	}
	
	public void SetupPlayer(ControlMapInstance controlMapInstance)
	{
		if (m_ControlMapInstance != null)
			m_ControlMapInstance.Deactivate();
		m_ControlMapInstance = controlMapInstance;
	}

	public void Update()
	{
		// Move
		var moveX = m_ControlMapInstance[moveControlX].value;
		var moveY = m_ControlMapInstance[moveControlY].value;

		Vector3 velocity = transform.TransformDirection(new Vector3(moveX, 0, moveY)) * moveSpeed;
		m_Rigid.velocity = new Vector3 (velocity.x, m_Rigid.velocity.y, velocity.z);

		// Look
		if (isCursorLocked)
		{
			var lookX = m_ControlMapInstance[lookControlX].value;
			var lookY = m_ControlMapInstance[lookControlY].value;

			m_Rotation.y += lookX;
			m_Rotation.x = Mathf.Clamp(m_Rotation.x - lookY, -89, 89);
		}

		transform.localEulerAngles = new Vector3(0, m_Rotation.y, 0);
		head.localEulerAngles = new Vector3(m_Rotation.x, 0, 0);

		// Fire
		var fire = m_ControlMapInstance[fireControl].button;
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

		if (m_ControlMapInstance[lockCursorControl].buttonDown)
			LockCursor();

		if (m_ControlMapInstance[unlockCursorControl].buttonDown)
			UnlockCursor();
		
		if (m_ControlMapInstance[menuControl].buttonDown)
			sizer.ToggleMenu();
		
		HandleControlsText();
	}
	
	void HandleControlsText()
	{
		string help = string.Empty;
		
		help += GetControlHelp(m_ControlMapInstance[moveControlX]) + "\n";
		help += GetControlHelp(m_ControlMapInstance[moveControlY]) + "\n";
		help += GetControlHelp(m_ControlMapInstance[lookControlX]) + "\n";
		help += GetControlHelp(m_ControlMapInstance[lookControlY]) + "\n";
		help += GetControlHelp(m_ControlMapInstance[fireControl]) + "\n";
		help += GetControlHelp(m_ControlMapInstance[menuControl]);
		controlsText.text = help;
	}
	
	private string GetControlHelp (InputControl control)
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

	void LockCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void UnlockCursor()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	bool isCursorLocked
	{
		get { return Cursor.lockState == CursorLockMode.Locked; }
	}
}
