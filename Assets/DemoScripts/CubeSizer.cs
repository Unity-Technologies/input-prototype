using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputNew;

// This sizer does not use the UI event system,
// since it's not integrated with the Input System prototype at this point.
// UI is only used for graphics.

public class CubeSizer : MonoBehaviour
{
	PlayerInput m_ControlMapInstance;
	
	public ActionMap controlMap;
	public PlayerInput referenceControlMapInstance;
	
	[Space(10)]
	public InputAction moveControlX;
	public InputAction menuControl;
	
	[Space(10)]
	public GameObject menu;
	public Slider slider;
	
	public float size { get { return slider.value; } }
	
	public void OpenMenu()
	{
		enabled = true;
		menu.SetActive(true);
		m_ControlMapInstance = InputSystem.CreateMapInstance(controlMap, referenceControlMapInstance);
		m_ControlMapInstance.Activate();
	}
	
	public void CloseMenu()
	{
		m_ControlMapInstance.Deactivate();
		menu.SetActive(false);
		enabled = false;
	}
	
	public void ToggleMenu()
	{
		if (enabled)
			CloseMenu();
		else
			OpenMenu();
	}
	
	void Update()
	{
		slider.value += m_ControlMapInstance[moveControlX].value * 0.05f;
		if (m_ControlMapInstance[menuControl].buttonDown)
			ToggleMenu();
	}
}
