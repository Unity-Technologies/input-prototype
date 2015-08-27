using UnityEngine;
using UnityEngine.InputNew;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
//using System.Reflection;
using System;
using System.Linq;

internal static class InputDeviceGUIUtility
{
	static Dictionary<System.Type, InputDevice> s_DeviceInstances = new Dictionary<System.Type, InputDevice>();
	static Dictionary<System.Type, string[]> s_DeviceControlNames = new Dictionary<System.Type, string[]>();
	
	static string[] s_DeviceNames = null;
	static Type[] s_DeviceTypes = null;
	static Dictionary<Type, int> s_IndicesOfDevices = null;
	
	public static InputDevice GetDevice(System.Type type)
	{
		InputDevice device = null;
		if (!s_DeviceInstances.TryGetValue(type, out device))
		{
			device = (InputDevice)System.Activator.CreateInstance(type);
			s_DeviceInstances[type] = device;
		}
		return device;
	}
	
	public static string GetDeviceControlName(System.Type type, int controlIndex)
	{
		return GetDeviceControlNames(type)[controlIndex];
	}
	
	public static string[] GetDeviceControlNames(System.Type type)
	{
		string[] names = null;
		if (!s_DeviceControlNames.TryGetValue(type, out names))
		{
			InputDevice device = GetDevice(type);
			names = new string[device.GetControlCount()];
			for (int i = 0; i < names.Length; i++)
				names[i] = device.GetControlData(i).name;
			s_DeviceControlNames[type] = names;
		}
		return names;
	}
	
	static void InitDevices()
	{
		if (s_DeviceTypes != null)
			return;
		
		s_DeviceTypes = (
			from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
			from assemblyType in domainAssembly.GetExportedTypes()
			where assemblyType.IsSubclassOf(typeof(InputDevice))
			select assemblyType
		).OrderBy(e => GetInheritancePath(e)).ToArray();
		
		s_DeviceNames = s_DeviceTypes.Select(e => e.Name).ToArray();
		
		s_IndicesOfDevices = new Dictionary<Type, int>();
		for (int i = 0; i < s_DeviceTypes.Length; i++)
			s_IndicesOfDevices[s_DeviceTypes[i]] = i;
	}
	
	public static string[] GetDeviceNames()
	{
		InitDevices();
		return s_DeviceNames;
	}
	
	public static int GetDeviceIndex(Type type)
	{
		InitDevices();
		return s_IndicesOfDevices[type];
	}
	
	public static Type GetDeviceType(int index)
	{
		InitDevices();
		return s_DeviceTypes[index];
	}
	
	static string GetInheritancePath(Type type)
	{
		if (type.BaseType == typeof(InputDevice))
			return type.Name;
		return GetInheritancePath(type.BaseType) + "/" + type.Name;
	}
}

[CustomEditor(typeof(ControlMapEntry))]
public class ControlMapEntryEditor : Editor
{
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
	}
	
	void DrawBinding (ControlBinding binding)
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
		DrawSourceSummary("Source (-)", source.negative);
		DrawSourceSummary("Source (+)", source.positive);
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
		return string.Format("{0} {1}", source.deviceType.Name, InputDeviceGUIUtility.GetDeviceControlName(source.deviceType, source.controlIndex));
	}
}
