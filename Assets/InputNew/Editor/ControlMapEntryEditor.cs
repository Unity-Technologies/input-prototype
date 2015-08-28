using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(ControlMapEntry))]
public class ControlMapEntryEditor : Editor
{
	static class Styles
	{
		public static GUIContent iconToolbarPlus =	EditorGUIUtility.IconContent ("Toolbar Plus", "Add to list");
		public static GUIContent iconToolbarMinus =	EditorGUIUtility.IconContent ("Toolbar Minus", "Remove from list");
		public static GUIContent iconToolbarPlusMore =	EditorGUIUtility.IconContent ("Toolbar Plus More", "Choose to add to list");
	}
	
	int m_ControlScheme = 0;
	bool m_ShowCommon = true;
	
	public int controlScheme { get { return m_ControlScheme; } set { m_ControlScheme = value; } }
	public bool showCommon { get { return m_ShowCommon; } set { m_ShowCommon = value; } }
	
	ControlMapEntry m_Entry;
	
	void Awake ()
	{
		m_Entry = (ControlMapEntry)serializedObject.targetObject;
	}
	
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		
		EditorGUI.BeginChangeCheck();
		string name = EditorGUILayout.TextField("Name", m_Entry.controlData.name);
		if (EditorGUI.EndChangeCheck())
		{
			InputControlData data = m_Entry.controlData;
			data.name = name;
			m_Entry.controlData = data;
			m_Entry.name = name;
		}
		
		EditorGUI.BeginChangeCheck();
		var type = (InputControlType)EditorGUILayout.EnumPopup("Type", m_Entry.controlData.controlType);
		if (EditorGUI.EndChangeCheck())
		{
			InputControlData data = m_Entry.controlData;
			data.controlType = type;
			m_Entry.controlData = data;
		}
		
		EditorGUILayout.Space();
		
		if (showCommon)
		{
			// Show common stuff
			for (int i = 0; i < m_Entry.bindings.Count; i++)
			{
				EditorGUILayout.LabelField("Control Scheme "+i);
				EditorGUI.indentLevel++;
				DrawBinding(m_Entry.bindings[i]);
				EditorGUI.indentLevel--;
			}
		}
		else
		{
			if (controlScheme >= 0 && controlScheme < m_Entry.bindings.Count)
			DrawBinding(m_Entry.bindings[controlScheme]);
		}
		
		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(m_Entry);
	}
	
	void DrawBinding(ControlBinding binding)
	{
		if (binding.primaryIsButtonAxis)
		{
			DrawButtonAxisSources(binding);
			DrawSources(binding);
		}
		else
		{
			DrawSources(binding);
			DrawButtonAxisSources(binding);
		}
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(15 * EditorGUI.indentLevel);
		GUILayout.Button(Styles.iconToolbarMinus, GUIStyle.none);
		Rect r = GUILayoutUtility.GetRect(Styles.iconToolbarPlusMore, GUIStyle.none);
		if (GUI.Button(r, Styles.iconToolbarPlusMore, GUIStyle.none))
			ShowAddOptions(r, binding);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}
	
	void ShowAddOptions(Rect rect, ControlBinding binding)
	{
		GenericMenu menu = new GenericMenu();
		menu.AddItem(new GUIContent("Regular Source"), false, AddSource, binding);
		menu.AddItem(new GUIContent("Button Axis Source"), false, AddButtonAxisSource, binding);
		menu.DropDown(rect);
	}
	
	void AddSource(object data)
	{
		ControlBinding binding = (ControlBinding)data;
		binding.sources.Add(new InputControlDescriptor());
	}
	
	void AddButtonAxisSource(object data)
	{
		ControlBinding binding = (ControlBinding)data;
		binding.buttonAxisSources.Add(new ButtonAxisSource(new InputControlDescriptor(), new InputControlDescriptor()));
	}
	
	void DrawSources(ControlBinding binding)
	{
		for (int i = 0; i < binding.sources.Count; i++)
		{
			DrawSourceSummary(binding.sources[i]);
		}
	}
	
	void DrawButtonAxisSources(ControlBinding binding)
	{
		for (int i = 0; i < binding.buttonAxisSources.Count; i++)
		{
			DrawButtonAxisSourceSummary(binding.buttonAxisSources[i]);
		}
	}
	
	void DrawSourceSummary(InputControlDescriptor source)
	{
		DrawSourceSummary("Source", source);
		EditorGUILayout.Space();
	}
	
	void DrawButtonAxisSourceSummary(ButtonAxisSource source)
	{
		DrawSourceSummary("Source (negative)", source.negative);
		DrawSourceSummary("Source (positive)", source.positive);
		EditorGUILayout.Space();
	}
	
	void DrawSourceSummary(string label, InputControlDescriptor source)
	{
		Rect rect = EditorGUILayout.GetControlRect();
		rect = EditorGUI.PrefixLabel(rect, new GUIContent(label));
		rect.width = (rect.width - 4) * 0.5f;
		
		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		string[] deviceNames = InputDeviceGUIUtility.GetDeviceNames();
		EditorGUI.BeginChangeCheck();
		int deviceIndex = EditorGUI.Popup(rect, InputDeviceGUIUtility.GetDeviceIndex(source.deviceType), deviceNames);
		if (EditorGUI.EndChangeCheck())
			source.deviceType = InputDeviceGUIUtility.GetDeviceType(deviceIndex);
		
		rect.x += rect.width + 4;
		
		string[] controlNames = InputDeviceGUIUtility.GetDeviceControlNames(source.deviceType);
		EditorGUI.BeginChangeCheck();
		int controlIndex = EditorGUI.Popup(rect, source.controlIndex, controlNames);
		if (EditorGUI.EndChangeCheck())
			source.controlIndex = controlIndex;
		
		EditorGUI.indentLevel = indentLevel;
	}
	
	string GetSourceString (InputControlDescriptor source)
	{
		return string.Format("{0} {1}", source.deviceType.Name, InputDeviceGUIUtility.GetDeviceControlName(source));
	}
}
