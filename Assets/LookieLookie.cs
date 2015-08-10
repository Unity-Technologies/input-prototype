using UnityEngine;
using UnityEngine.InputNew;
using System.Linq;

public class LookieLookie : MonoBehaviour
{
	public ControlMap controlMap;
	public ControlMapEntry lookControlX;
	public ControlMapEntry lookControlY;

	private ControlMapInstance _controlMapInstance;

	public void Awake()
	{
		_controlMapInstance = InputSystem.BindInputs( controlMap ).First();
		_controlMapInstance.Activate();
	}

	public void Update()
	{
		var lookX = _controlMapInstance[ lookControlX ].floatValue;
		var lookY = _controlMapInstance[ lookControlY ].floatValue;

		Debug.Log( string.Format( "lookX = {0}, lookY = {1}", lookX, lookY ) );
	}
}
