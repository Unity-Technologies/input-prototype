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

		CreateControl(actionMap, "MoveX", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyCode.A, (int)KeyCode.D, (int)KeyCode.LeftArrow, (int)KeyCode.RightArrow),
			CreateBinding(typeof(Gamepad), gamepad.leftStickX.index),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.LeftStickX)
		);
		
		CreateControl(actionMap, "MoveY", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyCode.S, (int)KeyCode.W, (int)KeyCode.DownArrow, (int)KeyCode.UpArrow),
			CreateBinding(typeof(Gamepad), gamepad.leftStickY.index),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.LeftStickY)
		);
		
		CreateControlComposite(actionMap, "Move", InputControlType.Vector2, new[] { 0, 1 });
		
		CreateControl(actionMap, "LookX", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.LockedDeltaX),
			CreateBinding(typeof(Gamepad), gamepad.rightStickX.index),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.RightStickX)
		);
		
		CreateControl(actionMap, "LookY", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.LockedDeltaY),
			CreateBinding(typeof(Gamepad), gamepad.rightStickY.index),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.RightStickY)
		);
		
		CreateControlComposite(actionMap, "Look", InputControlType.Vector2, new[] { 3, 4 });
		
		CreateControl(actionMap, "Fire", InputControlType.Button,
			CreateBinding(typeof(Pointer), (int)PointerControl.LeftButton),
			CreateBinding(typeof(Gamepad), gamepad.rightTrigger.index),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.Action1)
		);
		
		CreateControl(actionMap, "Menu", InputControlType.Button,
			CreateBinding(typeof(Keyboard), (int)KeyCode.Space),
			CreateBinding(typeof(Gamepad), gamepad.start.index),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.Menu)
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

	[ MenuItem("Tools/Create Device Profile Asset") ]
	public static void CreateDeviceProfileAsset()
	{
		CreateXbox360MacProfileAsset();
		CreateXbox360WinProfileAsset();
	}

	static void CreateXbox360MacProfileAsset()
	{
		var profile = ScriptableObject.CreateInstance<JoystickProfile>();
		var gamepad = new Gamepad();
		
		profile.AddDeviceName("Gamepad");
		profile.AddSupportedPlatform("OS X");
		profile.SetMappingsCount(gamepad.controls.Count, gamepad.controls.Count);
		
		profile.SetMapping(00, gamepad.leftStickX.index, "Left Stick X");
		profile.SetMapping(01, gamepad.leftStickY.index, "Left Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(21, gamepad.leftStickButton.index, "Left Stick Button");
		
		profile.SetMapping(02, gamepad.rightStickX.index, "Right Stick X");
		profile.SetMapping(03, gamepad.rightStickY.index, "Right Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(22, gamepad.rightStickButton.index, "Right Stick Button");
		
		profile.SetMapping(15, gamepad.dPadUp.index, "DPad Up");
		profile.SetMapping(16, gamepad.dPadDown.index, "DPad Down");
		profile.SetMapping(17, gamepad.dPadLeft.index, "DPad Left");
		profile.SetMapping(18, gamepad.dPadRight.index, "DPad Right");
		
		profile.SetMapping(26, gamepad.action1.index, "A");
		profile.SetMapping(27, gamepad.action2.index, "B");
		profile.SetMapping(28, gamepad.action3.index, "X");
		profile.SetMapping(29, gamepad.action4.index, "Y");
		
		profile.SetMapping(04, gamepad.leftTrigger.index, "Left Trigger", Range.full, Range.positive);
		profile.SetMapping(05, gamepad.rightTrigger.index, "Right Trigger", Range.full, Range.positive);
		profile.SetMapping(23, gamepad.leftBumper.index, "Left Bumper");
		profile.SetMapping(24, gamepad.rightBumper.index, "Right Bumper");
		
		profile.SetMapping(19, gamepad.start.index, "Start");
		profile.SetMapping(20, gamepad.back.index, "Back");
		profile.SetMapping(25, gamepad.system.index, "System");
		
		const string path = "Assets/FakePrototypeStuff/Xbox360MacProfile.asset";
		AssetDatabase.CreateAsset(profile, path);
	}
	
	static void CreateXbox360WinProfileAsset()
	{
		var profile = ScriptableObject.CreateInstance<JoystickProfile>();
		var gamepad = new Gamepad();
		
		profile.AddDeviceName("Gamepad");
		profile.AddSupportedPlatform("Windows");
		profile.SetMappingsCount(gamepad.controls.Count, gamepad.controls.Count);

		profile.SetMapping(00, gamepad.leftStickX.index, "Left Stick X");
		profile.SetMapping(01, gamepad.leftStickY.index, "Left Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(18, gamepad.leftStickButton.index, "Left Stick Button");
		
		profile.SetMapping(03, gamepad.rightStickX.index, "Right Stick X");
		profile.SetMapping(04, gamepad.rightStickY.index, "Right Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(19, gamepad.rightStickButton.index, "Right Stick Button");
		
		profile.SetMapping(06, gamepad.dPadUp.index, "DPad Up");
		profile.SetMapping(06, gamepad.dPadDown.index, "DPad Down");
		profile.SetMapping(05, gamepad.dPadLeft.index, "DPad Left");
		profile.SetMapping(05, gamepad.dPadRight.index, "DPad Right");
		
		profile.SetMapping(10, gamepad.action1.index, "A");
		profile.SetMapping(11, gamepad.action2.index, "B");
		profile.SetMapping(12, gamepad.action3.index, "X");
		profile.SetMapping(13, gamepad.action4.index, "Y");
		
		profile.SetMapping(08, gamepad.leftTrigger.index, "Left Trigger", Range.full, Range.positive);
		profile.SetMapping(09, gamepad.rightTrigger.index, "Right Trigger", Range.full, Range.positive);
		profile.SetMapping(14, gamepad.leftBumper.index, "Left Bumper");
		profile.SetMapping(15, gamepad.rightBumper.index, "Right Bumper");
		
		profile.SetMapping(17, gamepad.start.index, "Start");
		profile.SetMapping(16, gamepad.back.index, "Back");
		
		const string path = "Assets/FakePrototypeStuff/Xbox360WinProfile.asset";
		AssetDatabase.CreateAsset(profile, path);
	}
}
