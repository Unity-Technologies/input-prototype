using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;
using Random = UnityEngine.Random;
using UnityEngine.Serialization;

public class MultiplayerManager
	: MonoBehaviour
{
	public GameObject playerPrefab;
	[FormerlySerializedAs("controlMap")]
	public ActionMap actionMap;
	public GameObject hubCamera;

	enum PlayerStatus { Inactive, Joined, Ready }
	
	class PlayerInfo
	{
		public PlayerHandle playerHandle;
		public FirstPersonControls actions;
		public bool ready = false;
		public int colorIndex;

		public PlayerInfo(PlayerHandle playerHandle)
		{
			this.playerHandle = playerHandle;
			actions = playerHandle.GetActions<FirstPersonControls>();
			actions.active = true;
		}
	}
	
	List<PlayerInfo> players = new List<PlayerInfo>();
	
	public void Destroy()
	{
		for (int i = players.Count - 1; i >= 0; i--)
		{
			if (players[i].actions != null)
				players[i].actions.active = false;
		}
	}
	
	public void Update()
	{
		if (players.Count < 4)
		{
			PlayerHandle newPlayer = InputSystem.CreatePlayerHandle(actionMap, true);
			if (newPlayer != null)
				players.Add(new PlayerInfo(newPlayer));
		}

		int readyCount = 0;
		for (int i = players.Count - 1; i >= 0; i--)
		{
			var player = players[i];
			if (!player.ready)
			{
				if (player.actions.fire.wasJustPressed)
					player.ready = true;
				if (player.actions.menu.wasJustPressed)
				{
					player.playerHandle.Destroy();
					players.Remove(player);
					continue;
				}
				if (player.actions.moveX.positive.wasJustPressed)
					player.colorIndex = ((player.colorIndex + 1) % colors.Length);
				if (player.actions.moveX.negative.wasJustPressed)
					player.colorIndex = ((player.colorIndex + colors.Length - 1) % colors.Length);
			}
			else
			{
				if (player.actions.fire.wasJustPressed || player.actions.menu.wasJustPressed)
					player.ready = false;
			}
			if (player.ready)
				readyCount++;
		}

		if (readyCount >= 1 && (players.Count - readyCount) == 0)
			StartGame();
	}
	
	Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };
	
	public void OnGUI()
	{
		float width = 200;
		float height = 300;
		int playerNum = 0;
		for (int i = 0; i < players.Count; i++)
		{
			PlayerInfo player = players[i];
			
			Rect rect = new Rect(20 + (width + 20) * playerNum, (Screen.height - height) * 0.5f, width, height);
			GUILayout.BeginArea(rect, "Player", "box");
			GUILayout.Space(20);
			
			GUI.color = colors[player.colorIndex];
			GUILayout.Button(GUIContent.none, GUILayout.Height(50));
			GUI.color = Color.white;
			
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (!player.ready)
				GUILayout.Label(string.Format("Press {0} when ready", player.actions.fire.GetPrimarySourceName()));
			else
				GUILayout.Label("READY");
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			playerNum++;
		}
	}
	
	void StartGame()
	{
		hubCamera.SetActive(false);
		
		int playerCount = players.Count;
		float fraction = 1f / playerCount;
		for (int i = 0; i < players.Count; i++)
		{
			PlayerInfo playerInfo = players[i];
			if (!playerInfo.ready)
			{
				playerInfo.playerHandle.Destroy();
				continue;
			}
			
			var player = (GameObject)Instantiate(playerPrefab, Vector3.right * 2 * i, Quaternion.identity);
			player.GetComponent<CharacterInputController>().SetupPlayer(playerInfo.playerHandle.index);
			player.GetComponentInChildren<Camera>().rect = new Rect(0, fraction * i, 1, fraction);
			player.transform.Find("Canvas/Virtual Joystick").gameObject.SetActive(false);
		}
		
		gameObject.SetActive(false);
	}
}
