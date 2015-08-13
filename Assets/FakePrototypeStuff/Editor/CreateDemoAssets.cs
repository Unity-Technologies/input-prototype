using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections.Generic;
using Assets.Utilities;

public static class CreateDemoAssets
{
	private static ControlMapEntry CreateControl (string name, InputControlType controlType, params ControlBinding[] bindingsPerControlScheme)
	{
		var entry = ScriptableObject.CreateInstance< ControlMapEntry >();
		entry.controlData = new InputControlData
		{
			name = name,
			controlType = controlType
		};
		entry.bindings = new List<ControlBinding>(bindingsPerControlScheme);
		return entry;
	}
	
	private static ControlBinding CreateBinding (System.Type deviceType, int controlIndex)
	{
		return new ControlBinding 
		{
			sources = new List< InputControlDescriptor >
			{
				new InputControlDescriptor
				{
					deviceType = deviceType,
					controlIndex = controlIndex
				}
			}
		};
	}
	
	private static ControlBinding CreateButtonAxisBinding (System.Type deviceType, int controlIndexNegative, int controlIndexPositive)
	{
		return new ControlBinding 
		{
			buttonAxisSources = new List< ButtonAxisSource >
			{
				new ButtonAxisSource(
					new InputControlDescriptor
					{
						deviceType = deviceType,
						controlIndex = controlIndexNegative
					},
					new InputControlDescriptor
					{
						deviceType = deviceType,
						controlIndex = controlIndexPositive
					}
				)
			}
		};
	}
	
	private static ControlMapEntry CreateControlComposite (string name, InputControlType controlType, int[] indices)
	{
		var entry = ScriptableObject.CreateInstance< ControlMapEntry >();
		entry.controlData = new InputControlData
		{
			name = name,
			controlType = controlType,
			componentControlIndices = indices
		};
		return entry;
	}
	
	[ MenuItem("Tools/Create Input Map Asset") ]
	public static void CreateInputMapAsset()
	{
		var controlMap = ScriptableObject.CreateInstance< ControlMap >();

		var entries = new List< ControlMapEntry >();
		entries.Add(CreateControl("MoveX", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyControl.A, (int)KeyControl.D),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.LeftStickX)
		));
		
		entries.Add(CreateControl("MoveY", InputControlType.RelativeAxis,
			CreateButtonAxisBinding(typeof(Keyboard), (int)KeyControl.S, (int)KeyControl.W),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.LeftStickY)
		));
		
		entries.Add(CreateControlComposite("Move", InputControlType.Vector2, new[] { 0, 1 }));
		
		entries.Add(CreateControl("LookX", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.PositionX),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.RightStickX)
		));
		
		entries.Add(CreateControl("LookY", InputControlType.RelativeAxis,
			CreateBinding(typeof(Pointer), (int)PointerControl.PositionY),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.RightStickY)
		));
		
		entries.Add(CreateControlComposite("Look", InputControlType.Vector2, new[] { 3, 4 }));
		
		entries.Add(CreateControl("Fire", InputControlType.Button,
			CreateBinding(typeof(Pointer), (int)PointerControl.LeftButton),
			CreateBinding(typeof(Gamepad), (int)GamepadControl.RightTrigger)
		));
	
		controlMap.entries = entries;
		controlMap.schemes = new List< string > { "KeyboardMouse", "Gamepad" };

		const string path = "Assets/DemoAssets/FirstPersonControls.asset";
		AssetDatabase.CreateAsset(controlMap, path);
		for (int i = 0; i < entries.Count; i++)
			AssetDatabase.AddObjectToAsset(entries[i], path);
	}

	[ MenuItem("Tools/Create Device Profile Asset") ]
	public static void CreateDeviceProfileAsset()
	{
		CreateXbox360MacProfileAsset();
	}

	static void CreateXbox360MacProfileAsset()
	{
		var profile = ScriptableObject.CreateInstance< GamepadProfile >();
		
		profile.AddDeviceName("Generic Gamepad");
		profile.AddSupportedPlatform("OS X");
		profile.SetMappingsCount(EnumHelpers.GetValueCount< GamepadControl >());
		
		profile.SetMapping(00, GamepadControl.LeftStickX, "Left Stick X");
		profile.SetMapping(01, GamepadControl.LeftStickY, "Left Stick Y");
		profile.SetMapping(21, GamepadControl.LeftStickButton, "Left Stick Button");
		
		profile.SetMapping(02, GamepadControl.RightStickX, "Right Stick X");
		profile.SetMapping(03, GamepadControl.RightStickY, "Right Stick Y");
		profile.SetMapping(22, GamepadControl.RightStickButton, "Right Stick Button");
		
		profile.SetMapping(15, GamepadControl.DPadUp, "DPad Up");
		profile.SetMapping(16, GamepadControl.DPadDown, "DPad Down");
		profile.SetMapping(17, GamepadControl.DPadLeft, "DPad Left");
		profile.SetMapping(18, GamepadControl.DPadRight, "DPad Right");
		
		profile.SetMapping(26, GamepadControl.ButtonA, "A");
		profile.SetMapping(27, GamepadControl.ButtonB, "B");
		profile.SetMapping(28, GamepadControl.ButtonX, "X");
		profile.SetMapping(29, GamepadControl.ButtonY, "Y");
		
		profile.SetMapping(04, GamepadControl.LeftTrigger, "Left Trigger");
		profile.SetMapping(05, GamepadControl.RightTrigger, "Right Trigger");
		profile.SetMapping(23, GamepadControl.LeftBumper, "Left Bumper");
		profile.SetMapping(24, GamepadControl.RightBumper, "Right Bumper");
		
		profile.SetMapping(19, GamepadControl.Start, "Start");
		profile.SetMapping(20, GamepadControl.Back, "Back");
		profile.SetMapping(25, GamepadControl.Back, "System");
		
		const string path = "Assets/FakePrototypeStuff/Xbox360MacProfile.asset";
		AssetDatabase.CreateAsset(profile, path);
	}
}
