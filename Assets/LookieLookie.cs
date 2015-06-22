using UnityEngine;
using UnityEngine.InputNew;
using System.Collections;
using System.Linq;

public class LookieLookie : MonoBehaviour
{
	public InputMap inputMap;

	private InputMapInstance _inputMapInstance;
	private InputControlData _lookBinding;

	public void Awake()
	{
		_inputMapInstance = InputSystem.BindInputs( inputMap ).First();
		_inputMapInstance.Activate();
	}

	public void Update()
	{
		//var position = _inputMapInstance[ lookBinding ].vector3Value;
	}
}
