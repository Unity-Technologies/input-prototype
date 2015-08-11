using UnityEngine;
using UnityEngine.InputNew;

public class InputManager
	: MonoBehaviour
{
	public InputDeviceProfile[] profiles;

	public void Awake()
	{
		InputSystem.Initialize( profiles );
	}
}
