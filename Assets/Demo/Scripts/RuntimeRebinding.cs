using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.InputNew;

public class RuntimeRebinding : MonoBehaviour
{
	public ActionMap actionMap;
	public ControlScheme controlScheme;
	
	const int k_NameWidth = 200;
	const int k_BindingWidth = 120;
	
	string[] controlSchemeNames;
	InputControlDescriptor descriptorToBeAssigned = null;
	
	void Start()
	{
		controlSchemeNames = actionMap.controlSchemes.Select(e => e.name).ToArray();
	}
	
	void OnGUI()
	{
		if (controlScheme == null)
		{
			GUILayout.Label("No control scheme assigned.");
			return;
		}
		
		if (actionMap == null)
		{
			GUILayout.Label("No action map assigned.");
			return;
		}
		
		int index = actionMap.controlSchemes.FindIndex(e => e == controlScheme);
		int newIndex = GUILayout.Toolbar(index, controlSchemeNames);
		if (newIndex != index)
		{
			controlScheme = actionMap.controlSchemes[newIndex];
		}
		
		if (controlScheme.bindings.Count != actionMap.actions.Count)
		{
			GUILayout.Label("Control scheme bindings don't match action map actions.");
			return;
		}
		
		GUILayout.Space(10);
		
		for (int control = 0; control < actionMap.actions.Count; control++)
		{
			DisplayControlBinding(actionMap.actions[control], controlScheme.bindings[control]);
		}
	}
	
	void DisplayControlBinding(InputAction action, ControlBinding controlBinding)
	{
		if (controlBinding.sources.Count > 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(action.name, GUILayout.Width(k_NameWidth));
			for (int i = 0; i < controlBinding.sources.Count; i++)
				DisplaySource(controlBinding.sources[i]);
			GUILayout.EndHorizontal();
		}
		
		if (controlBinding.buttonAxisSources.Count > 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(action.name + " (\u2212)", GUILayout.Width(k_NameWidth));
			for (int i = 0; i < controlBinding.buttonAxisSources.Count; i++)
				DisplayButtonAxisSource(controlBinding.buttonAxisSources[i], false);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label(action.name + " (+)", GUILayout.Width(k_NameWidth));
			for (int i = 0; i < controlBinding.buttonAxisSources.Count; i++)
				DisplayButtonAxisSource(controlBinding.buttonAxisSources[i], true);
			GUILayout.EndHorizontal();
		}
	}
	
	void DisplayButtonAxisSource(ButtonAxisSource source, bool positive)
	{
		DisplaySource(positive ? source.positive : source.negative);
	}
	
	void DisplaySource(InputControlDescriptor descriptor)
	{
		if (descriptor == descriptorToBeAssigned)
		{
			GUILayout.Button("...", GUILayout.Width(k_BindingWidth));
		}
		else if (GUILayout.Button(InputDeviceUtility.GetDeviceControlName(descriptor), GUILayout.Width(k_BindingWidth)))
		{
			descriptorToBeAssigned = descriptor;
			InputSystem.ListenForBinding(BindInputControl);
		}
	}
	
	void BindInputControl(InputControl control)
	{
		descriptorToBeAssigned.deviceType = control.provider.GetType();
		descriptorToBeAssigned.controlIndex = control.index;
		descriptorToBeAssigned = null;
	}
}
