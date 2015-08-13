using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class ExecuteAllEvents
	: MonoBehaviour
{
	public void Update()
	{
		InputSystem.BeginNewFrame ();
		InputSystem.ExecuteEvents ();
	}

	public void FixedUpdate()
	{
		InputSystem.BeginNewFrame ();
		InputSystem.ExecuteEvents ();
	}
}
