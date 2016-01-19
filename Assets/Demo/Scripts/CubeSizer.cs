using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputNew;
using UnityEngine.Serialization;

// This sizer does not use the UI event system,
// since it's not integrated with the Input System prototype at this point.
// UI is only used for graphics.

public class CubeSizer : MonoBehaviour
{
	FirstPersonControls m_PlayerInput;
	
	[FormerlySerializedAs("controlMap")]
	public ActionMap actionMap;
	public int playerIndex;
	
	[Space(10)]
	public GameObject menu;
	public Slider slider;
	
	public float size { get { return slider.value; } }
	
	public void OpenMenu()
	{
		enabled = true;
		menu.SetActive(true);
		m_PlayerInput = InputSystem.GetPlayerHandle(playerIndex).AssignActions<FirstPersonControls>(actionMap);
		m_PlayerInput.active = true;
	}
	
	public void CloseMenu()
	{
		m_PlayerInput.active = false;
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
		if (m_PlayerInput.moveX.negative.wasJustPressed)
			slider.value -= 0.1f;
		if (m_PlayerInput.moveX.positive.wasJustPressed)
			slider.value += 0.1f;
		if (m_PlayerInput.menu.wasJustPressed)
			ToggleMenu();
	}
}
