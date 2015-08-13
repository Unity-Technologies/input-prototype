using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class ExecuteAllEvents
	: MonoBehaviour
{
	public void Update()
	{
		InputSystem.ExecuteEvents ();
	}

	public void FixedUpdate()
	{
		InputSystem.ExecuteEvents ();
	}
}
