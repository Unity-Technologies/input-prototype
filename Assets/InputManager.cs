using UnityEngine;
using UnityEngine.InputNew;

public class InputManager
	: MonoBehaviour
{
	public InputDeviceProfile[] profiles;

	public void Awake()
	{
		Debug.Log( "Initialize input system" );
		InputSystem.Initialize( profiles );
	}
}
