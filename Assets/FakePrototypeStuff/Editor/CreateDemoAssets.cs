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

		CreateControl(actionMap, "MoveX", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyCode.A, (int)KeyCode.D, (int)KeyCode.LeftArrow, (int)KeyCode.RightArrow),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.LeftStickX),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.LeftStickX)
		);
		
		CreateControl(actionMap, "MoveY", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyCode.S, (int)KeyCode.W, (int)KeyCode.DownArrow, (int)KeyCode.UpArrow),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.LeftStickY),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.LeftStickY)
		);
		
		CreateControlComposite(actionMap, "Move", InputControlType.Vector2, new[] { 0, 1 });
		
		CreateControl(actionMap, "LookX", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.LockedDeltaX),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.RightStickX),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.RightStickX)
		);
		
		CreateControl(actionMap, "LookY", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.LockedDeltaY),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.RightStickY),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.RightStickY)
		);
		
		CreateControlComposite(actionMap, "Look", InputControlType.Vector2, new[] { 3, 4 });
		
		CreateControl(actionMap, "Fire", InputControlType.Button,
			CreateBinding(typeof(Pointer), (int)PointerControl.LeftButton),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.RightTrigger),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.Action1)
		);
		
		CreateControl(actionMap, "Menu", InputControlType.Button,
			CreateBinding(typeof(Keyboard), (int)KeyCode.Space),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.Start),
			CreateBinding(typeof(VirtualJoystick), (int)VirtualJoystickControl.Menu)
		);
	
		CreateControl(actionMap, "LockCursor", InputControlType.Button,
			CreateBinding(typeof(Pointer), (int)PointerControl.LeftButton)
		);
		CreateControl(actionMap, "UnlockCursor", InputControlType.Button,
			CreateBinding(typeof(Keyboard), (int)KeyCode.Escape)
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
		var profile = ScriptableObject.CreateInstance<GamepadProfile>();
		
		profile.AddDeviceName("Gamepad");
		profile.AddSupportedPlatform("OS X");
		profile.SetMappingsCount(EnumHelpers.GetValueCount<GamepadControl>(), EnumHelpers.GetValueCount<GamepadControl>());
		
		profile.SetMapping(00, GamepadControl.LeftStickX, "Left Stick X");
		profile.SetMapping(01, GamepadControl.LeftStickY, "Left Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(21, GamepadControl.LeftStickButton, "Left Stick Button");
		
		profile.SetMapping(02, GamepadControl.RightStickX, "Right Stick X");
		profile.SetMapping(03, GamepadControl.RightStickY, "Right Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(22, GamepadControl.RightStickButton, "Right Stick Button");
		
		profile.SetMapping(15, GamepadControl.DPadUp, "DPad Up");
		profile.SetMapping(16, GamepadControl.DPadDown, "DPad Down");
		profile.SetMapping(17, GamepadControl.DPadLeft, "DPad Left");
		profile.SetMapping(18, GamepadControl.DPadRight, "DPad Right");
		
		profile.SetMapping(26, GamepadControl.Action1, "A");
		profile.SetMapping(27, GamepadControl.Action2, "B");
		profile.SetMapping(28, GamepadControl.Action3, "X");
		profile.SetMapping(29, GamepadControl.Action4, "Y");
		
		profile.SetMapping(04, GamepadControl.LeftTrigger, "Left Trigger", Range.full, Range.positive);
		profile.SetMapping(05, GamepadControl.RightTrigger, "Right Trigger", Range.full, Range.positive);
		profile.SetMapping(23, GamepadControl.LeftBumper, "Left Bumper");
		profile.SetMapping(24, GamepadControl.RightBumper, "Right Bumper");
		
		profile.SetMapping(19, GamepadControl.Start, "Start");
		profile.SetMapping(20, GamepadControl.Back, "Back");
		profile.SetMapping(25, GamepadControl.System, "System");
		
		const string path = "Assets/FakePrototypeStuff/Xbox360MacProfile.asset";
		AssetDatabase.CreateAsset(profile, path);
	}
	
	static void CreateXbox360WinProfileAsset()
	{
		var profile = ScriptableObject.CreateInstance<GamepadProfile>();
		
		profile.AddDeviceName("Gamepad");
		profile.AddSupportedPlatform("Windows");
		profile.SetMappingsCount(EnumHelpers.GetValueCount<GamepadControl>(), EnumHelpers.GetValueCount<GamepadControl>());
		
		profile.SetMapping(00, GamepadControl.LeftStickX, "Left Stick X");
		profile.SetMapping(01, GamepadControl.LeftStickY, "Left Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(18, GamepadControl.LeftStickButton, "Left Stick Button");
		
		profile.SetMapping(03, GamepadControl.RightStickX, "Right Stick X");
		profile.SetMapping(04, GamepadControl.RightStickY, "Right Stick Y", Range.fullInverse, Range.full);
		profile.SetMapping(19, GamepadControl.RightStickButton, "Right Stick Button");
		
		profile.SetMapping(06, GamepadControl.DPadUp, "DPad Up");
		profile.SetMapping(06, GamepadControl.DPadDown, "DPad Down");
		profile.SetMapping(05, GamepadControl.DPadLeft, "DPad Left");
		profile.SetMapping(05, GamepadControl.DPadRight, "DPad Right"); //  TODO map one input to two outputs
		
		profile.SetMapping(10, GamepadControl.Action1, "A");
		profile.SetMapping(11, GamepadControl.Action2, "B");
		profile.SetMapping(12, GamepadControl.Action3, "X");
		profile.SetMapping(13, GamepadControl.Action4, "Y");
		
		profile.SetMapping(08, GamepadControl.LeftTrigger, "Left Trigger", Range.full, Range.positive);
		profile.SetMapping(09, GamepadControl.RightTrigger, "Right Trigger", Range.full, Range.positive);
		profile.SetMapping(14, GamepadControl.LeftBumper, "Left Bumper");
		profile.SetMapping(15, GamepadControl.RightBumper, "Right Bumper");
		
		profile.SetMapping(17, GamepadControl.Start, "Start");
		profile.SetMapping(16, GamepadControl.Back, "Back");
		
		const string path = "Assets/FakePrototypeStuff/Xbox360WinProfile.asset";
		AssetDatabase.CreateAsset(profile, path);
	}
}
