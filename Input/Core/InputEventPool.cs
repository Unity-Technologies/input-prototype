using System;
using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	class InputEventPool
	{
		readonly Dictionary<Type, List<InputEvent>> m_Pools = new Dictionary<Type, List<InputEvent>>();

		#region Public Methods
		public TEvent ReuseOrCreate<TEvent>()
			where TEvent : InputEvent, new()
		{
			List<InputEvent> pool;
			if (m_Pools.TryGetValue(typeof(TEvent), out pool))
			{
				if (pool.Count > 0)
				{
					var last = pool.Count - 1;
					var inputEvent = pool[last];
					pool.RemoveAt(last);
					inputEvent.Reset();
					return (TEvent)inputEvent;
				}
			}

			return new TEvent();
		}

		public void Return(InputEvent inputEvent)
		{
			var type = inputEvent.GetType();

			List<InputEvent> pool;
			if (!m_Pools.TryGetValue(type, out pool))
			{
				pool = new List<InputEvent>();
				m_Pools.Add(type, pool);
			}

			pool.Add(inputEvent);
		}
		#endregion
	}
}
