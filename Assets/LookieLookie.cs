using UnityEngine;
using UnityEngine.InputNew;
using System.Collections;

public class LookieLookie : MonoBehaviour
{
	public InputMap inputMap;

	private InputControlProvider _inputMapInstance;

	public void Awake()
	{
		_inputMapInstance = InputSystem.BindInputs( inputMap );
	}

	public void Update()
	{
	}
}
