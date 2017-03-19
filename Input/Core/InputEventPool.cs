using System;
using System.Collections.Generic;
using UnityEditor.Experimental.EditorVR.Utilities;

namespace UnityEngine.InputNew
{
	class InputEventPool
	{
		readonly Dictionary<Type, List<InputEvent>> m_Pools = new Dictionary<Type, List<InputEvent>>();
		readonly Func<KeyValuePair<Type, List<InputEvent>>, Type, bool> m_GetPool;

		public InputEventPool()
		{
			m_GetPool = GetPool;
		}

		static bool GetPool(KeyValuePair<Type, List<InputEvent>> kvp, Type eventType)
		{
			return kvp.Key == eventType;
		}

		#region Public Methods
		public TEvent ReuseOrCreate<TEvent>()
			where TEvent : InputEvent, new()
		{
			var pool = ObjectUtils.ForEachInDictionary(m_Pools, m_GetPool, typeof(TEvent));
			if (pool.HasValue)
			{
				var list = pool.Value.Value;
				if (list.Count > 0)
				{
					var last = list.Count - 1;
					var inputEvent = list[last];
					list.RemoveAt(last);
					inputEvent.Reset();
					return (TEvent)inputEvent;
				}
			}

			return new TEvent();
		}

		public void Return(InputEvent inputEvent)
		{
			var type = inputEvent.GetType();
			List<InputEvent> list = null;

			var pool = ObjectUtils.ForEachInDictionary(m_Pools, m_GetPool, type);
			if (pool.HasValue)
				list = pool.Value.Value;

			if (list == null)
			{
				list = new List<InputEvent>();
				m_Pools.Add(type, list);
			}

			list.Add(inputEvent);
		}
		#endregion
	}
}
