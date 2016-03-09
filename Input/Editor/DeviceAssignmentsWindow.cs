using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.InputNew
{
	public class DeviceAssignmentsWindow : EditorWindow
	{
		static int s_MaxAssignedDevices;
		static int s_MaxMapDevices;
		static int s_MaxMaps;
		const int kDeviceElementWidth = 160;
		const int kPlayerElementWidth = kDeviceElementWidth * 2 + 4;

		Vector2 scrollPos;

		static class Styles {
			public static GUIStyle boxStyle;
			static Styles()
			{
				boxStyle = new GUIStyle("box");
				boxStyle.normal.textColor = EditorStyles.label.normal.textColor;
			}
		}

		[MenuItem ("Window/Players")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			DeviceAssignmentsWindow window = (DeviceAssignmentsWindow)EditorWindow.GetWindow (typeof (DeviceAssignmentsWindow));
			window.Show();
			window.titleContent = new GUIContent("Players");
		}

		void OnEnable()
		{
			PlayerHandle.onChange += Repaint;
			ActionMapInput.onStatusChange += Repaint;
			EditorApplication.playmodeStateChanged += Repaint;
		}

		void OnDisable()
		{
			PlayerHandle.onChange -= Repaint;
			ActionMapInput.onStatusChange -= Repaint;
			EditorApplication.playmodeStateChanged -= Repaint;
		}
		
		void OnGUI()
		{
			var devices = InputSystem.leastToMostRecentlyUsedDevices;
			var players = PlayerHandleManager.players;

			s_MaxAssignedDevices = 1;
			foreach (var player in players)
				s_MaxAssignedDevices = Mathf.Max(s_MaxAssignedDevices, player.assignments.Count);

			s_MaxMaps = 1;
			foreach (var player in players)
				s_MaxMaps = Mathf.Max(s_MaxMaps, player.maps.Count);

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			{
				ShowUnassignedDevices(devices);

				EditorGUILayout.Space();

				ShowGlobalPlayerHandles(devices, players);

				EditorGUILayout.Space();

				ShowPlayerHandles(devices, players);
			}
			EditorGUILayout.EndScrollView();
		}

		void ShowUnassignedDevices(IEnumerable<InputDevice> devices)
		{
			GUILayout.Label("Unassigned Devices", EditorStyles.boldLabel);
			GUILayout.Label("(In the prototype the available devices are hard-coded so for now presence in this list doesn't mean the devices physically exist and are connected.)");

			EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
			foreach (var device in devices)
			{
				if (device.assignment != null)
					continue;
				GUILayout.Label(device.ToString(), Styles.boxStyle, GUILayout.Width(kDeviceElementWidth));
			}
			EditorGUILayout.EndHorizontal();
		}

		void ShowGlobalPlayerHandles(IEnumerable<InputDevice> devices, IEnumerable<PlayerHandle> players)
		{
			GUILayout.Label("Global Player Handles", EditorStyles.boldLabel);
			GUILayout.Label("Listen to all unassigned devices.");

			EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
			foreach (var player in players)
			{
				if (!player.global)
					continue;
				DrawPlayerHandle(player);
			}
			EditorGUILayout.EndHorizontal();
		}

		void ShowPlayerHandles(IEnumerable<InputDevice> devices, IEnumerable<PlayerHandle> players)
		{
			GUILayout.Label("Player Handles", EditorStyles.boldLabel);
			GUILayout.Label("Listen to devices they have assigned.");

			EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
			foreach (var player in players)
			{
				if (player.global)
					continue;
				DrawPlayerHandle(player);
			}
			EditorGUILayout.EndHorizontal();
		}

		void DrawPlayerHandle(PlayerHandle player)
		{
			EditorGUIUtility.labelWidth = 180;

			int fixedLines = 3;
			int lines = fixedLines + s_MaxAssignedDevices + s_MaxMaps * 3;
			Rect rect = GUILayoutUtility.GetRect(kPlayerElementWidth, EditorGUIUtility.singleLineHeight * lines + 3, "box", GUILayout.ExpandWidth(false));
			GUI.Box(rect, "Player " + player.index + (player.global ? " (Global)" : ""), Styles.boxStyle);
			rect.height = EditorGUIUtility.singleLineHeight;
			Rect origRect = rect;

			rect.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(rect, "Assigned Devices");
			EditorGUI.indentLevel++;
			rect.y += EditorGUIUtility.singleLineHeight;
			if (player.assignments.Count == 0)
				EditorGUI.LabelField(rect, "None");
			for (int i = 0; i < player.assignments.Count; i++)
			{
				EditorGUI.LabelField(rect, player.assignments[i].device.ToString());
				rect.y += EditorGUIUtility.singleLineHeight;
			}
			EditorGUI.indentLevel--;

			rect = origRect;
			rect.y += EditorGUIUtility.singleLineHeight * (fixedLines - 1 + s_MaxAssignedDevices);

			EditorGUI.LabelField(rect, "Action Map Inputs");
			rect.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.indentLevel++;

			for (int i = 0; i < player.maps.Count; i++)
			{
				ActionMapInput map = player.maps[i];

				Color oldColor = GUI.color;
				if (!map.active)
				{
					Color color = GUI.color;
					color.a *= 0.5f;
					GUI.color = color;
				}

				EditorGUI.LabelField(rect, map.GetType().Name);
				rect.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.indentLevel++;

				string schemeString = "-";
				if (map.active)
				{
					if (map.controlScheme != null)
						schemeString = map.controlScheme.name;
					else
						schemeString = "None";
				}
				EditorGUI.LabelField(rect, "Current Control Scheme", schemeString);
				rect.y += EditorGUIUtility.singleLineHeight;

				string devicesString = "-";
				if (map.active)
				{
					devicesString = string.Join(", ", map.GetCurrentlyUsedDevices().Select(e => e.ToString()).ToArray());
					if (devicesString == "")
						devicesString = "None";
				}
				EditorGUI.LabelField(rect, "Currently Used Devices", devicesString);
				rect.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.indentLevel--;

				GUI.color = oldColor;
			}

			EditorGUI.indentLevel--;
		}
	}
}