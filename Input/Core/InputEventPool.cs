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
			var enumerator = m_Pools.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var kvp = enumerator.Current;
				if (kvp.Key == typeof(TEvent))
				{
					var pool = kvp.Value;
					if (pool.Count > 0)
					{
						var last = pool.Count - 1;
						var inputEvent = pool[last];
						pool.RemoveAt(last);
						inputEvent.Reset();
						return (TEvent)inputEvent;
					}
				}
			}
			enumerator.Dispose();

			return new TEvent();
		}

		public void Return(InputEvent inputEvent)
		{
			var type = inputEvent.GetType();

			List<InputEvent> pool = null;
			var enumerator = m_Pools.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var kvp = enumerator.Current;
				if (kvp.Key == type)
					pool = kvp.Value;
			}
			enumerator.Dispose();

			if (pool == null)
			{
				pool = new List<InputEvent>();
				m_Pools.Add(type, pool);
			}

			pool.Add(inputEvent);
		}
		#endregion
	}
}
