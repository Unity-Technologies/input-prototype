using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public static class PlayerHandleManager
	{
		static Dictionary<int, PlayerHandle> s_Players = new Dictionary<int, PlayerHandle>();
		static int s_NextPlayerIndex = 0;

		public static IEnumerable<PlayerHandle> players { get { return s_Players.Values; } }

		public static PlayerHandle GetNewPlayerHandle()
		{
			PlayerHandle handle = new PlayerHandle(s_NextPlayerIndex);
			s_Players[handle.index] = handle;
			s_NextPlayerIndex++;
			return handle;
		}

		// Gets existing handle for index if available.
		public static PlayerHandle GetPlayerHandle(int index)
		{
			PlayerHandle player = null;
			s_Players.TryGetValue(index, out player);
			return player;
		}

		internal static void RemovePlayerHandle(PlayerHandle handle)
		{
			s_Players.Remove(handle.index);
		}
	}
}
