using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections.Generic;

public static class CreateInputMapAsset
{
	[ MenuItem( "Tools/Create Input Map Asset" ) ]
	public static void CreateAsset()
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

		const string path = "Assets/LookieLookieMap.asset";
		AssetDatabase.CreateAsset( controlMap, path );
		AssetDatabase.AddObjectToAsset( lookX, path );
		AssetDatabase.AddObjectToAsset( lookY, path );
		AssetDatabase.AddObjectToAsset( look, path );
	}
}
