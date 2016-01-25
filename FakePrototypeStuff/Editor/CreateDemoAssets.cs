using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections.Generic;
using Assets.Utilities;

public static class CreateDemoAssets
{
	private static void CreateControl(ActionMap actionMap, string name, InputControlType controlType, params ControlBinding[] bindingsPerControlScheme)
	{
		var action = new InputAction();
		action.controlData = new InputControlData
		{
			name = name,
			controlType = controlType
		};
		actionMap.actions.Add(action);
		for (int i = 0; i < actionMap.controlSchemes.Count; i++)
		{
			var binding = (i >= bindingsPerControlScheme.Length ? null : bindingsPerControlScheme[i]);
			actionMap.controlSchemes[i].bindings.Add(binding);
		}
	}
	
	private static ControlBinding CreateBinding(System.Type deviceType, int controlIndex)
	{
		return new ControlBinding 
		{
			sources = new List<InputControlDescriptor>
			{
				new InputControlDescriptor
				{
					deviceType = deviceType,
					controlIndex = controlIndex
				}
			}
		};
	}
	
	private static ControlBinding CreateButtonAxisBinding(System.Type deviceType, params int[] controlIndices)
	{
		if (controlIndices.Length % 2 != 0)
			throw new System.Exception("Number of indices must be even.");
		var binding = new ControlBinding();
		binding.primaryIsButtonAxis = true;
		binding.buttonAxisSources = new List<ButtonAxisSource>();
		for (int i = 0; i < controlIndices.Length; i += 2)
		{
			binding.buttonAxisSources.Add(
				new ButtonAxisSource(
					new InputControlDescriptor
					{
						deviceType = deviceType,
						controlIndex = controlIndices[i]
					},
					new InputControlDescriptor
					{
						deviceType = deviceType,
						controlIndex = controlIndices[i + 1]
					}
				)
			);
		}
		return binding;
	}
	
	private static void CreateControlComposite(ActionMap actionMap, string name, InputControlType controlType, int[] indices)
	{
		var action = new InputAction();
		action.controlData = new InputControlData
		{
			name = name,
			controlType = controlType,
			componentControlIndices = indices
		};
		actionMap.actions.Add(action);
		// We have to create dummy bindings for now to make the indices match up.
		for (int i = 0; i < actionMap.controlSchemes.Count; i++)
			actionMap.controlSchemes[i].bindings.Add(new ControlBinding());
	}
	
	[ MenuItem("Tools/Create Input Map Asset") ]
	public static void CreateInputMapAsset()
	{
		var actionMap = ScriptableObject.CreateInstance<ActionMap>();
		actionMap.controlSchemes = new List<ControlScheme>
		{
			new ControlScheme("KeyboardMouse"),
			new ControlScheme("Gamepad"),
			new ControlScheme("VirtualJoystick")
		};

		var gamepad = new Gamepad();
		var virtualJoystick = new VirtualJoystick();

		CreateControl(actionMap, "MoveX", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyCode.A, (int)KeyCode.D, (int)KeyCode.LeftArrow, (int)KeyCode.RightArrow),
			CreateBinding(typeof(Gamepad), gamepad.leftStickX.index),
			CreateBinding(typeof(VirtualJoystick), virtualJoystick.leftStickX.index)
		);
		
		CreateControl(actionMap, "MoveY", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyCode.S, (int)KeyCode.W, (int)KeyCode.DownArrow, (int)KeyCode.UpArrow),
			CreateBinding(typeof(Gamepad), gamepad.leftStickY.index),
			CreateBinding(typeof(VirtualJoystick), virtualJoystick.leftStickY.index)
		);
		
		CreateControlComposite(actionMap, "Move", InputControlType.Vector2, new[] { 0, 1 });
		
		CreateControl(actionMap, "LookX", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.LockedDeltaX),
			CreateBinding(typeof(Gamepad), gamepad.rightStickX.index),
			CreateBinding(typeof(VirtualJoystick), virtualJoystick.rightStickX.index)
		);
		
		CreateControl(actionMap, "LookY", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.LockedDeltaY),
			CreateBinding(typeof(Gamepad), gamepad.rightStickY.index),
			CreateBinding(typeof(VirtualJoystick), virtualJoystick.rightStickY.index)
		);
		
		CreateControlComposite(actionMap, "Look", InputControlType.Vector2, new[] { 3, 4 });
		
		CreateControl(actionMap, "Fire", InputControlType.Button,
			CreateBinding(typeof(Pointer), (int)PointerControl.LeftButton),
			CreateBinding(typeof(Gamepad), gamepad.rightTrigger.index),
			CreateBinding(typeof(VirtualJoystick), virtualJoystick.action1.index)
		);
		
		CreateControl(actionMap, "Menu", InputControlType.Button,
			CreateBinding(typeof(Keyboard), (int)KeyCode.Space),
			CreateBinding(typeof(Gamepad), gamepad.start.index),
			CreateBinding(typeof(VirtualJoystick), virtualJoystick.menu.index)
		);
	
		CreateControl(actionMap, "LockCursor", InputControlType.Button,
			CreateBinding(typeof(Pointer), (int)PointerControl.LeftButton)
		);
		CreateControl(actionMap, "UnlockCursor", InputControlType.Button,
			CreateBinding(typeof(Keyboard), (int)KeyCode.Escape)
		);
		CreateControl(actionMap, "Reconfigure", InputControlType.Button,
			CreateBinding(typeof(Keyboard), (int)KeyCode.R)
		);

		const string path = "Assets/DemoAssets/FirstPersonControls.asset";
		AssetDatabase.CreateAsset(actionMap, path);
	}
}
