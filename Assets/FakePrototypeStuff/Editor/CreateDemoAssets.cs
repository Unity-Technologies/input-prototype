using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections.Generic;
using Assets.Utilities;

public static class CreateDemoAssets
{
	private static ControlMapEntry CreateControl (string name, InputControlType controlType, System.Type deviceType, int controlIndex)
	{
		var entry = ScriptableObject.CreateInstance< ControlMapEntry >();
		entry.controlData = new InputControlData
		{
			name = name,
			controlType = controlType
		};
		entry.bindings = new List< ControlBinding >
		{
			new ControlBinding 
			{
				sources = new List< InputControlDescriptor >
				{
					new InputControlDescriptor
					{
						deviceType = deviceType,
						controlIndex = controlIndex
					}
				}
			}
		};
		return entry;
	}
	
	private static ControlMapEntry CreateControlButtonAxis (string name, InputControlType controlType, System.Type deviceType, int controlIndexNegative, int controlIndexPositive)
	{
		var entry = ScriptableObject.CreateInstance< ControlMapEntry >();
		entry.controlData = new InputControlData
		{
			name = name,
			controlType = controlType
		};
		entry.bindings = new List< ControlBinding >
		{
			new ControlBinding 
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
			}
		};
		return entry;
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
	
	[ MenuItem( "Tools/Create Input Map Asset" ) ]
	public static void CreateInputMapAsset()
	{
		var controlMap = ScriptableObject.CreateInstance< ControlMap >();

		var entries = new List< ControlMapEntry >();
		entries.Add( CreateControlButtonAxis( "MoveX", InputControlType.RelativeAxis, typeof( Keyboard ), (int)KeyControl.A, (int)KeyControl.D ));
		entries.Add( CreateControlButtonAxis( "MoveY", InputControlType.RelativeAxis, typeof( Keyboard ), (int)KeyControl.S, (int)KeyControl.W ));
		entries.Add( CreateControlComposite( "Move", InputControlType.Vector2, new int[] { 0, 1 } ));
		entries.Add( CreateControl( "LookX", InputControlType.RelativeAxis, typeof( Pointer ), (int)PointerControl.PositionX ));
		entries.Add( CreateControl( "LookY", InputControlType.RelativeAxis, typeof( Pointer ), (int)PointerControl.PositionY ));
		entries.Add( CreateControlComposite( "Look", InputControlType.Vector2, new int[] { 3, 4 } ));
		entries.Add( CreateControl( "Fire", InputControlType.Button, typeof( Pointer ), (int)PointerControl.LeftButton ));
	
		controlMap.entries = entries;
		controlMap.schemes = new List< string > { "default" };

		const string path = "Assets/DemoAssets/LookieLookieMap.asset";
		AssetDatabase.CreateAsset( controlMap, path );
		for ( int i = 0; i < entries.Count; i++ )
			AssetDatabase.AddObjectToAsset( entries[i], path );
	}

	[ MenuItem( "Tools/Create Device Profile Asset" ) ]
	public static void CreateDeviceProfileAsset()
	{
		var profile = ScriptableObject.CreateInstance< GamepadProfile >();

		profile.AddDeviceName( "Generic Gamepad" );
		profile.SetMappingsCount( EnumHelpers.GetValueCount< GamepadControl >() );
		profile.SetMapping( ( int ) GamepadControl.ButtonA, GamepadControl.ButtonB );
		profile.SetMapping( ( int ) GamepadControl.ButtonB, GamepadControl.ButtonA );

		const string path = "Assets/FakePrototypeStuff/FakepadProfile.asset";
		AssetDatabase.CreateAsset( profile, path );
	}
}
