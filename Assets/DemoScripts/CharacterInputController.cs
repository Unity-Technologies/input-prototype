using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

	[Space(10)]
	public Transform head;
	public float moveSpeed = 5;
	public GameObject projectile;
	public float timeBetweenShots = 0.5f;

	public void Awake()
	{
		m_ControlMapInstance = new ControlMapCombinedInstance(controlMap);
		m_ControlMapInstance.Activate();
		m_Rigid = GetComponent<Rigidbody>();
	}

	public void Update()
	{
		// Move
		var moveX = m_ControlMapInstance[moveControlX].value;
		var moveY = m_ControlMapInstance[moveControlY].value;

		Vector3 velocity = transform.TransformDirection(new Vector3(moveX, 0, moveY)) * moveSpeed;
		m_Rigid.velocity = new Vector3 (velocity.x, m_Rigid.velocity.y, velocity.z);

		// Look
		var lookX = m_ControlMapInstance[lookControlX].value;
		var lookY = m_ControlMapInstance[lookControlY].value;

		m_Rotation.y += lookX;
		transform.localEulerAngles = new Vector3(0, m_Rotation.y, 0);

		m_Rotation.x = Mathf.Clamp(m_Rotation.x - lookY, -89, 89);
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
	}
	
	public void OnGUI()
	{
		GUILayout.Label(GetControlHelp(m_ControlMapInstance[moveControlX]));
		GUILayout.Label(GetControlHelp(m_ControlMapInstance[moveControlY]));
		GUILayout.Label(GetControlHelp(m_ControlMapInstance[lookControlX]));
		GUILayout.Label(GetControlHelp(m_ControlMapInstance[lookControlY]));
		GUILayout.Label(GetControlHelp(m_ControlMapInstance[fireControl]));
	}
	
	static List<string> s_Names = new List<string>();
	private string GetControlHelp (InputControl control)
	{
		control.GetPrimarySourceNames(s_Names);
		if (s_Names.Count == 2)
			return string.Format("Use {0} and {1} to {2}!", s_Names[0], s_Names[1], control.name);
		else
			return string.Format("Use {0} to {1}!", s_Names[0], control.name);
	}

	void Fire()
	{
		var newProjectile = Instantiate(projectile);
		newProjectile.transform.position = head.position + head.forward * 0.6f;
		newProjectile.transform.rotation = head.rotation;
		newProjectile.GetComponent<Rigidbody>().AddForce(head.forward * 20f, ForceMode.Impulse);
		newProjectile.GetComponent<MeshRenderer>().material.color = new Color( Random.value, Random.value, Random.value, 1.0f );
	}
}
