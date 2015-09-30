using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ActionMap))]
public class ActionMapEditor : Editor
{
	static class Styles
	{
		public static GUIContent iconToolbarPlus =	EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
		public static GUIContent iconToolbarMinus =	EditorGUIUtility.IconContent("Toolbar Minus", "Remove from list");
		public static GUIContent iconToolbarPlusMore =	EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
	}
	
	ActionMap m_ActionMap;
	
	int m_SelectedScheme = 0;
	InputAction m_SelectedEntry = null;
	
	int selectedScheme
	{
		get { return m_SelectedScheme; }
		set
		{
			if (m_SelectedScheme == value)
				return;
			m_SelectedScheme = value;
			if (m_EntryEditor != null)
				m_EntryEditor.controlScheme = value;
		}
	}
	
	InputAction selectedEntry
	{
		get { return m_SelectedEntry; }
		set
		{
			if (m_SelectedEntry == value)
				return;
			if (m_EntryEditor != null)
				DestroyImmediate(m_EntryEditor);
			m_SelectedEntry = value;
			if (m_SelectedEntry != null)
			{
				m_EntryEditor = (InputActionEditor)Editor.CreateEditor(m_SelectedEntry, typeof(InputActionEditor));
				m_EntryEditor.controlScheme = selectedScheme;
				m_EntryEditor.showCommon = false;
			}
		}
	}
	
	InputActionEditor m_EntryEditor = null;
	
	public void OnEnable()
	{
		m_ActionMap = (ActionMap)serializedObject.targetObject;
	}
	
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		
		if (selectedScheme >= m_ActionMap.schemes.Count)
			selectedScheme = m_ActionMap.schemes.Count - 1;
		
		// Show schemes
		EditorGUIUtility.GetControlID(FocusType.Passive);
		for (int i = 0; i < m_ActionMap.schemes.Count; i++)
		{
			Rect rect = EditorGUILayout.GetControlRect();
			
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
				selectedScheme = i;
			
			if (selectedScheme == i)
				GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			
			EditorGUI.BeginChangeCheck();
			string schemeName = EditorGUI.TextField(rect, "Control Scheme " + i, m_ActionMap.schemes[i]);
			if (EditorGUI.EndChangeCheck())
				m_ActionMap.schemes[i] = schemeName;
			
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
				Event.current.Use();
		}
		
		// Remove an add buttons
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(15 * EditorGUI.indentLevel);
		if (GUILayout.Button(Styles.iconToolbarMinus, GUIStyle.none))
		{
			m_ActionMap.schemes.RemoveAt(selectedScheme);
			
			for (int i = 0; i < m_ActionMap.entries.Count; i++)
			{
				InputAction entry = m_ActionMap.entries[i];
				if (entry.bindings.Count > selectedScheme)
					entry.bindings.RemoveAt(selectedScheme);
				while (entry.bindings.Count > m_ActionMap.schemes.Count)
					entry.bindings.RemoveAt(entry.bindings.Count - 1);
			}
			if (selectedScheme >= m_ActionMap.schemes.Count)
				selectedScheme = m_ActionMap.schemes.Count - 1;
		}
		if (GUILayout.Button(Styles.iconToolbarPlus, GUIStyle.none))
		{
			m_ActionMap.schemes.Add("New Control Scheme");
			for (int i = 0; i < m_ActionMap.entries.Count; i++)
			{
				InputAction entry = m_ActionMap.entries[i];
				while (entry.bindings.Count < m_ActionMap.schemes.Count)
					entry.bindings.Add(new ControlBinding());
			}
			selectedScheme = m_ActionMap.schemes.Count - 1;
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		// Show high level controls
		EditorGUILayout.LabelField("Controls", m_ActionMap.schemes[selectedScheme] + " Bindings");
		EditorGUILayout.BeginVertical("Box");
		foreach (var entry in m_ActionMap.entries)
		{
			DrawEntry(entry, selectedScheme);
		}
		EditorGUILayout.EndVertical();
		
		// Remove an add buttons
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(15 * EditorGUI.indentLevel);
		if (GUILayout.Button(Styles.iconToolbarMinus, GUIStyle.none))
		{
			m_ActionMap.entries.Remove(selectedEntry);
			DestroyImmediate(selectedEntry, true);
			if (!m_ActionMap.entries.Contains(selectedEntry))
				selectedEntry = m_ActionMap.entries[m_ActionMap.entries.Count - 1];
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(m_ActionMap));
		}
		if (GUILayout.Button(Styles.iconToolbarPlus, GUIStyle.none))
		{
			var entry = ScriptableObject.CreateInstance<InputAction>();
			entry.controlData = new InputControlData() { name = "New Control" };
			entry.name = entry.controlData.name;
			entry.bindings = new List<ControlBinding>();
			while (entry.bindings.Count < m_ActionMap.schemes.Count)
				entry.bindings.Add(new ControlBinding());
			m_ActionMap.entries.Add(entry);
			AssetDatabase.AddObjectToAsset(entry, m_ActionMap);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(m_ActionMap));
			selectedEntry = m_ActionMap.entries[m_ActionMap.entries.Count - 1];
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		if (m_EntryEditor != null)
		{
			m_EntryEditor.OnInspectorGUI();
		}
		
		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(m_ActionMap);
	}
	
	void DrawEntry(InputAction entry, int controlScheme)
	{
		ControlBinding binding = (entry.bindings.Count > controlScheme ? entry.bindings[controlScheme] : null);
		
		int sourceCount = 0;
		int buttonAxisSourceCount = 0;
		if (binding != null)
		{
			sourceCount += binding.sources.Count;
			buttonAxisSourceCount += binding.buttonAxisSources.Count;
		}
		int totalSourceCount = sourceCount + buttonAxisSourceCount;
		int lines = Mathf.Max(1, totalSourceCount);
		
		float height = EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1) + 8;
		Rect totalRect = GUILayoutUtility.GetRect(1, height);
		
		Rect baseRect = totalRect;
		baseRect.yMin += 4;
		baseRect.yMax -= 4;
		
		if (selectedEntry == entry)
			GUI.DrawTexture(totalRect, EditorGUIUtility.whiteTexture);
		
		// Show control fields
		
		Rect rect = baseRect;
		rect.height = EditorGUIUtility.singleLineHeight;
		rect.width = EditorGUIUtility.labelWidth - 4;
		
		EditorGUI.LabelField(rect, entry.controlData.name);
		
		// Show binding fields
		
		if (binding != null)
		{
			rect = baseRect;
			rect.height = EditorGUIUtility.singleLineHeight;
			rect.xMin += EditorGUIUtility.labelWidth;
			
			if (binding.primaryIsButtonAxis)
			{
				DrawButtonAxisSources(ref rect, binding);
				DrawSources(ref rect, binding);
			}
			else
			{
				DrawSources(ref rect, binding);
				DrawButtonAxisSources(ref rect, binding);
			}
		}
		
		if (Event.current.type == EventType.MouseDown && totalRect.Contains(Event.current.mousePosition))
		{
			selectedEntry = entry;
			Event.current.Use();
		}
	}
	
	void DrawSources(ref Rect rect, ControlBinding binding)
	{
		for (int i = 0; i < binding.sources.Count; i++)
		{
			DrawSourceSummary(rect, binding.sources[i]);
			rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
	}
	
	void DrawButtonAxisSources(ref Rect rect, ControlBinding binding)
	{
		for (int i = 0; i < binding.buttonAxisSources.Count; i++)
		{
			DrawButtonAxisSourceSummary(rect, binding.buttonAxisSources[i]);
			rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
	}
	
	void DrawSourceSummary(Rect rect, InputControlDescriptor source)
	{
		EditorGUI.LabelField(rect, GetSourceString(source));
	}
	
	void DrawButtonAxisSourceSummary(Rect rect, ButtonAxisSource source)
	{
		if (source.negative.deviceType == source.positive.deviceType)
			EditorGUI.LabelField(rect,
				string.Format("{0} {1} & {2}",
					InputDeviceGUIUtility.GetDeviceName(source.negative),
					InputDeviceGUIUtility.GetDeviceControlName(source.negative),
					InputDeviceGUIUtility.GetDeviceControlName(source.positive)
				)
			);
		else
			EditorGUI.LabelField(rect, string.Format("{0} & {1}", GetSourceString(source.negative), GetSourceString(source.positive)));
	}
	
	string GetSourceString(InputControlDescriptor source)
	{
		return string.Format("{0} {1}", InputDeviceGUIUtility.GetDeviceName(source), InputDeviceGUIUtility.GetDeviceControlName(source));
	}
}
