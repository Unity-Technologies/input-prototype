using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(InputAction))]
public class InputActionEditor : Editor
{
	static class Styles
	{
		public static GUIContent iconToolbarPlus =	EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
		public static GUIContent iconToolbarMinus =	EditorGUIUtility.IconContent("Toolbar Minus", "Remove from list");
		public static GUIContent iconToolbarPlusMore =	EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
	}
	
	int m_ControlScheme = 0;
	bool m_ShowCommon = true;
	
	InputControlDescriptor m_SelectedSource = null;
	ButtonAxisSource m_SelectedButtonAxisSource = null;
	
	public int controlScheme { get { return m_ControlScheme; } set { m_ControlScheme = value; } }
	public bool showCommon { get { return m_ShowCommon; } set { m_ShowCommon = value; } }
	
	InputAction m_Entry;
	
	void Awake()
	{
		m_Entry = (InputAction)serializedObject.targetObject;
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
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(m_Entry));
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
		
		// Remove and add buttons
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(15 * EditorGUI.indentLevel);
		if (GUILayout.Button(Styles.iconToolbarMinus, GUIStyle.none))
		{
			if (m_SelectedSource != null)
				binding.sources.Remove(m_SelectedSource);
			if (m_SelectedButtonAxisSource != null)
				binding.buttonAxisSources.Remove(m_SelectedButtonAxisSource);
		}
		Rect r = GUILayoutUtility.GetRect(Styles.iconToolbarPlusMore, GUIStyle.none);
		if (GUI.Button(r, Styles.iconToolbarPlusMore, GUIStyle.none))
		{
			ShowAddOptions(r, binding);
		}
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
		var source = new InputControlDescriptor();
		binding.sources.Add(source);
		
		m_SelectedButtonAxisSource = null;
		m_SelectedSource = source;
	}
	
	void AddButtonAxisSource(object data)
	{
		ControlBinding binding = (ControlBinding)data;
		var source = new ButtonAxisSource(new InputControlDescriptor(), new InputControlDescriptor());
		binding.buttonAxisSources.Add(source);
		
		m_SelectedSource = null;
		m_SelectedButtonAxisSource = source;
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
		Rect rect = EditorGUILayout.GetControlRect();
		
		if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
		{
			m_SelectedButtonAxisSource = null;
			m_SelectedSource = source;
			Repaint();
		}
		if (m_SelectedSource == source)
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		
		DrawSourceSummary(rect, "Source", source);
		
		EditorGUILayout.Space();
	}
	
	void DrawButtonAxisSourceSummary(ButtonAxisSource source)
	{
		Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing);
		
		if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
		{
			m_SelectedSource = null;
			m_SelectedButtonAxisSource = source;
			Repaint();
		}
		if (m_SelectedButtonAxisSource == source)
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		
		rect.height = EditorGUIUtility.singleLineHeight;
		DrawSourceSummary(rect, "Source (negative)", source.negative);
		rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		DrawSourceSummary(rect, "Source (positive)", source.positive);
		
		EditorGUILayout.Space();
	}
	
	void DrawSourceSummary(Rect rect, string label, InputControlDescriptor source)
	{
		if (m_SelectedSource == source)
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		
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
	
	string GetSourceString(InputControlDescriptor source)
	{
		return string.Format("{0} {1}", source.deviceType.Name, InputDeviceGUIUtility.GetDeviceControlName(source));
	}
}
