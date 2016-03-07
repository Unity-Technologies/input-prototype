using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class DeviceAssignmentsWindow : EditorWindow
	{
		static int s_MaxAssignedDevices;
		static int s_MaxMapDevices;
		static int s_MaxMaps;
		const int kElementWidth = 160;

		Vector2 scrollPos;

		[MenuItem ("Window/Device Assignments")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			DeviceAssignmentsWindow window = (DeviceAssignmentsWindow)EditorWindow.GetWindow (typeof (DeviceAssignmentsWindow));
			window.Show();
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
			var players = InputSystem.players;

			s_MaxAssignedDevices = 0;
			foreach (var player in players)
				s_MaxAssignedDevices = Mathf.Max(s_MaxAssignedDevices, player.assignments.Count);

			s_MaxMaps = 0;
			foreach (var player in players)
			{
				s_MaxMaps = Mathf.Max(s_MaxMaps, player.maps.Count);
				foreach (var map in player.maps)
				{
					s_MaxMapDevices = Mathf.Max(s_MaxMapDevices, map.GetCurrentlyUsedDevices().Count);
				}
			}

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
			GUILayout.Label("Unassigned Devices");

			EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
			foreach (var device in devices)
			{
				if (device.assignment != null)
					continue;
				GUILayout.Label(device.ToString(), "box", GUILayout.Width(kElementWidth));
			}
			EditorGUILayout.EndHorizontal();
		}

		void ShowGlobalPlayerHandles(IEnumerable<InputDevice> devices, IEnumerable<PlayerHandle> players)
		{
			GUILayout.Label("Global Player Handles");

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
			GUILayout.Label("Player Handles");

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
			int lines = 2 + s_MaxAssignedDevices + s_MaxMaps * (2 + s_MaxMapDevices);
			Rect rect = GUILayoutUtility.GetRect(kElementWidth, EditorGUIUtility.singleLineHeight * lines + 3, "box", GUILayout.ExpandWidth(false));
			GUI.Box(rect, "Player " + player.index);
			rect.height = EditorGUIUtility.singleLineHeight;
			Rect origRect = rect;

			rect.y += EditorGUIUtility.singleLineHeight;

			for (int i = 0; i < player.assignments.Count; i++)
			{
				GUI.Label(rect, player.assignments[i].device.ToString());
				rect.y += EditorGUIUtility.singleLineHeight;
			}

			rect = origRect;
			rect.y += EditorGUIUtility.singleLineHeight * (2 + s_MaxAssignedDevices);

			for (int i = 0; i < player.maps.Count; i++)
			{
				ActionMapInput map = player.maps[i];
				GUI.color = new Color(1, 1, 1, map.active ? 1 : 0.5f);

				GUI.Label(rect, map.GetType().Name);
				rect.y += EditorGUIUtility.singleLineHeight;
				GUI.Label(rect, "(" + (map.controlScheme == null ? "None" : map.controlScheme.name) + ")");
				rect.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.indentLevel++;
				var devices = map.GetCurrentlyUsedDevices();
				for (int j = 0; j < devices.Count; j++)
				{
					EditorGUI.LabelField(rect, devices[j].ToString());
					rect.y += EditorGUIUtility.singleLineHeight;
				}
				EditorGUI.indentLevel--;
			}
		}
	}
}