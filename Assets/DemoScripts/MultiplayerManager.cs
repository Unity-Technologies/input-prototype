using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;
using Random = UnityEngine.Random;

public class MultiplayerManager
	: MonoBehaviour
{
	public GameObject playerPrefab;
	public ControlMap controlMap;
	public GameObject hubCamera;
	
	[Space(10)]
	public ControlMapEntry readyControl;
	public ControlMapEntry leaveControl;
	public ControlMapEntry navigateControl;

	enum PlayerStatus { Inactive, Joined, Ready }
	
	class PlayerInfo
	{
		public ControlMapInstance controls;
		public PlayerStatus status;
		public int colorIndex;
	}
	
	List<PlayerInfo> potentialPlayers = new List<PlayerInfo>();
	
	public void Start()
	{
		var potentialPlayerInputs = InputSystem.CreateMapInstances(controlMap);
		foreach (var mapInstance in potentialPlayerInputs)
		{
			mapInstance.Activate();
			potentialPlayers.Add(new PlayerInfo() { controls = mapInstance });
		}
	}
	
	public void Destroy()
	{
		for (int i = potentialPlayers.Count - 1; i >= 0; i--)
		{
			potentialPlayers[i].controls.Deactivate();
		}
	}
	
	public void Update()
	{
		int joinedCount = 0;
		int readyCount = 0;
		for (int i = potentialPlayers.Count - 1; i >= 0; i--)
		{
			var player = potentialPlayers[i];
			switch(player.status)
			{
				case PlayerStatus.Inactive:
				{
					if (player.controls.anyButton.buttonDown)
					{
						player.status = PlayerStatus.Joined;
						// Move to end
						potentialPlayers.Remove(player);
						potentialPlayers.Add(player);
					}
					break;
				}
				case PlayerStatus.Joined:
				{
					if (player.controls[readyControl].buttonDown)
						player.status = PlayerStatus.Ready;
					if (player.controls[leaveControl].buttonDown)
						player.status = PlayerStatus.Inactive;
					if (player.controls[navigateControl].buttonDown)
						player.colorIndex = ((player.colorIndex + 1) % colors.Length);
					break;
				}
				case PlayerStatus.Ready:
				{
					if (player.controls[readyControl].buttonDown || player.controls[leaveControl].buttonDown)
						player.status = PlayerStatus.Joined;
					break;
				}
			}
			
			if (player.status == PlayerStatus.Joined)
				joinedCount++;
			else if (player.status == PlayerStatus.Ready)
				readyCount++;
		}
		
		if (readyCount > 1 && joinedCount == 0)
			StartGame();
	}
	
	Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };
	
	public void OnGUI()
	{
		float width = 200;
		float height = 300;
		int playerNum = 0;
		for (int i = 0; i < potentialPlayers.Count; i++)
		{
			PlayerInfo player = potentialPlayers[i];
			if (player.status == PlayerStatus.Inactive)
				continue;
			
			Rect rect = new Rect(20 + (width + 20) * playerNum, (Screen.height - height) * 0.5f, width, height);
			GUILayout.BeginArea(rect, "Player", "box");
			GUILayout.Space(20);
			
			GUI.color = colors[player.colorIndex];
			GUILayout.Button(GUIContent.none, GUILayout.Height(50));
			GUI.color = Color.white;
			
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (player.status != PlayerStatus.Ready)
				GUILayout.Label(string.Format("Press {0} when ready", player.controls[readyControl].GetPrimarySourceName()));
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
		
		int playerCount = 0;
		for (int i = 0; i < potentialPlayers.Count; i++)
			if (potentialPlayers[i].status == PlayerStatus.Ready)
				playerCount++;
		
		int playerNum = 0;
		float fraction = 1f / playerCount;
		for (int i = 0; i < potentialPlayers.Count; i++)
		{
			PlayerInfo playerInfo = potentialPlayers[i];
			if (playerInfo.status != PlayerStatus.Ready)
				continue;
			
			var player = (GameObject)Instantiate(playerPrefab, Vector3.right * 2 * playerNum, Quaternion.identity);
			player.GetComponent<CharacterInputController>().SetupPlayer(playerInfo.controls);
			player.GetComponentInChildren<Camera>().rect = new Rect(0, fraction * playerNum, 1, fraction);
			
			playerNum++;
		}
		
		gameObject.SetActive(false);
	}
}
