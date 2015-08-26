using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ControlMap))]
public class ControlMapEditor : Editor
{
	static Dictionary<System.Type, InputDevice> s_DeviceInstances = new Dictionary<System.Type, InputDevice>();
	
	ControlMap m_ControlMap;
	
	int m_SelectedScheme = 0;
	
	public void OnEnable ()
	{
		m_ControlMap = (ControlMap)serializedObject.targetObject;
	}
	
	public override void OnInspectorGUI ()
	{
		// Show schemes
		EditorGUILayout.LabelField("Control Schemes");
		EditorGUIUtility.GetControlID(FocusType.Passive);
		EditorGUI.indentLevel++;
		for (int i = 0; i < m_ControlMap.schemes.Count; i++)
		{
			Rect rect = EditorGUILayout.GetControlRect();
			
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
				m_SelectedScheme = i;
			
			if (m_SelectedScheme == i)
				GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			
			EditorGUI.BeginChangeCheck();
			string schemeName = EditorGUI.TextField(rect, m_ControlMap.schemes[i]);
			if (EditorGUI.EndChangeCheck())
				m_ControlMap.schemes[i] = schemeName;
			
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
				Event.current.Use();
		}
		EditorGUI.indentLevel--;
		
		EditorGUILayout.Space();
		
		// Show high level controls
		EditorGUILayout.LabelField("Controls", "Control Scheme Bindings");
		EditorGUILayout.Space();
		foreach (var entry in m_ControlMap.entries)
		{
			DrawEntry(entry, m_SelectedScheme);
		}
	}
	
	void DrawEntry(ControlMapEntry entry, int controlScheme)
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
		int lines = Mathf.Max(2, totalSourceCount);
		
		float height = EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1) + 10;
		Rect totalRect = EditorGUILayout.GetControlRect(true, height);
		
		// Show control fields
		
		Rect rect = totalRect;
		rect.height = EditorGUIUtility.singleLineHeight;
		rect.width = EditorGUIUtility.labelWidth;
		
		EditorGUI.BeginChangeCheck();
		string name = EditorGUI.TextField(rect, entry.controlData.name);
		if (EditorGUI.EndChangeCheck())
		{
			InputControlData data = entry.controlData;
			data.name = name;
			entry.controlData = data;
			entry.name = name;
		}
		
		rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		
		EditorGUI.BeginChangeCheck();
		var type = (InputControlType)EditorGUI.EnumPopup(rect, entry.controlData.controlType);
		if (EditorGUI.EndChangeCheck())
		{
			InputControlData data = entry.controlData;
			data.controlType = type;
			entry.controlData = data;
		}
		
		// Show binding fields
		
		if (binding != null)
		{
			rect = totalRect;
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
		EditorGUI.LabelField(rect, string.Format("{0} - {1}", GetSourceString(source.negative), GetSourceString(source.positive)));
	}
	
	string GetSourceString (InputControlDescriptor source)
	{
		return string.Format("{0} {1}", source.deviceType.Name, GetDeviceControlName(source.deviceType, source.controlIndex));
	}
	
	static InputDevice GetDevice(System.Type type)
	{
		InputDevice device = null;
		if (!s_DeviceInstances.TryGetValue(type, out device))
		{
			device = (InputDevice)System.Activator.CreateInstance(type);
			s_DeviceInstances[type] = device;
		}
		return device;
	}
	
	static string GetDeviceControlName(System.Type type, int controlIndex)
	{
		InputDevice device = GetDevice(type);
		return device.GetControlData(controlIndex).name;
	}
	
	static List<string> GetDeviceControlNames(System.Type type)
	{
		InputDevice device = GetDevice(type);

		var list = new List<string>(device.GetControlCount());
		for (int i = 0; i < device.GetControlCount(); i++)
			list.Add(device.GetControlData(i).name);

		return list;
	}
}
