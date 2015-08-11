using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections.Generic;
using Assets.Utilities;

public static class CreateDemoAssets
{
	[ MenuItem( "Tools/Create Input Map Asset" ) ]
	public static void CreateInputMapAsset()
	{
		var controlMap = ScriptableObject.CreateInstance< ControlMap >();

		var entries = new List< ControlMapEntry >();
		var lookX = ScriptableObject.CreateInstance< ControlMapEntry >();
		lookX.controlData = new InputControlData
			  {
					  name = "LookX"
					, controlType = InputControlType.RelativeAxis
			  };
		lookX.bindings = new List< ControlBinding >
			{
				new ControlBinding 
				{
					  sources = new List< InputControlDescriptor >
					  {
							new InputControlDescriptor
							{
								  deviceType = typeof( Pointer )
								, controlIndex = ( int ) PointerControl.PositionX
							}
					  }
				}
			};
		entries.Add( lookX );

		var lookY = ScriptableObject.CreateInstance< ControlMapEntry >();
		lookY.controlData = new InputControlData
			  {
					  name = "LookY"
					, controlType = InputControlType.RelativeAxis
			  };
		lookY.bindings = new List< ControlBinding >
			{
				new ControlBinding 
				{
					  sources = new List< InputControlDescriptor >
					  {
							new InputControlDescriptor
							{
								  deviceType = typeof( Pointer )
								, controlIndex = ( int ) PointerControl.PositionY
							}
					  }
				}
			};
		entries.Add( lookY );

		var look = ScriptableObject.CreateInstance< ControlMapEntry >();
		look.controlData = new InputControlData
			  {
					  name = "Look"
					, controlType = InputControlType.Vector2
					, componentControlIndices = new int[] { 0, 1 }
			  };
		entries.Add( look );
	
		controlMap.entries = entries;
		controlMap.schemes = new List< string > { "default" };

		const string path = "Assets/LookieLookieMap.asset";
		AssetDatabase.CreateAsset( controlMap, path );
		AssetDatabase.AddObjectToAsset( lookX, path );
		AssetDatabase.AddObjectToAsset( lookY, path );
		AssetDatabase.AddObjectToAsset( look, path );
	}

	[ MenuItem( "Tools/Create Device Profile Asset" ) ]
	public static void CreateDeviceProfileAsset()
	{
		var profile = ScriptableObject.CreateInstance< GamepadProfile >();

		profile.AddDeviceName( "Generic Gamepad" );
		profile.SetMappingsCount( EnumHelpers.GetValueCount< GamepadControl >() );
		profile.SetMapping( ( int ) GamepadControl.ButtonA, GamepadControl.ButtonB );
		profile.SetMapping( ( int ) GamepadControl.ButtonB, GamepadControl.ButtonA );

		const string path = "Assets/FakepadProfile.asset";
		AssetDatabase.CreateAsset( profile, path );
	}
}
