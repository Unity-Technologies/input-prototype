using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections.Generic;

public static class CreateInputMapAsset
{
	[ MenuItem( "Tools/Create Input Map Asset" ) ]
	public static void CreateAsset()
	{
		var inputMap = ScriptableObject.CreateInstance< InputMap >();

		var bindings = new List< InputBinding >();
		var controls = new List< InputControlDescriptor >();
		var lookBinding = new InputBinding
		{
			  name = "Look"
			, controls = controls
		};
		bindings.Add( lookBinding );
		controls.Add( new InputControlDescriptor
			{
				  deviceType = typeof( Pointer )
				, controlIndex = ( int ) PointerControl.Position
			}
		);

		inputMap.bindings = bindings;

		AssetDatabase.CreateAsset( inputMap, "Assets/LookieLookieMap.asset" );
	}
}
