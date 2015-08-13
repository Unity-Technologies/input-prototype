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
		InputSystem.Initialize(profiles);
	}

	public void Update()
	{
		InputSystem.BeginNewFrame ();
	}
}
