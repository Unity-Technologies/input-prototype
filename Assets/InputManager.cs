using UnityEngine;
using UnityEngine.InputNew;

public class InputManager : MonoBehaviour
{
	public void Update()
	{
		InputSystem.ExecuteEvents();
	}

	public void FixedUpdate()
	{
		InputSystem.ExecuteEvents();
	}
}
