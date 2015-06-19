using UnityEngine;
using UnityEngine.InputNew;
using System.Collections;

public class LogMousePosition : MonoBehaviour
{
	public void Update()
	{
		Debug.Log( "Mouse position: " + InputSystem.mouse.position );
	}
}
