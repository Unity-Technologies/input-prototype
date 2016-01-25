using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class InputManager
	: MonoBehaviour
{
	public InputDeviceProfile[] profiles;

	public void Awake()
	{
		if (profiles == null)
			profiles = new InputDeviceProfile[] {};
		InputSystem.Initialize(profiles);
	}

	public void Update()
	{
		InputSystem.BeginFrame();
	}
}
