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
					var list = kvp.Value;
					if (list.Count > 0)
					{
						enumerator.Dispose();
						var last = list.Count - 1;
						var inputEvent = list[last];
						list.RemoveAt(last);
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
			var enumerator = m_Pools.GetEnumerator();
			var type = inputEvent.GetType();
			List<InputEvent> queue = null;
			while (enumerator.MoveNext())
			{
				var kvp = enumerator.Current;
				if (kvp.Key == type)
					queue = kvp.Value;
			}
			enumerator.Dispose();

			if (queue == null)
			{
				queue = new List<InputEvent>();
				m_Pools.Add(type, queue);
			}

			queue.Add(inputEvent);
		}

		#endregion
	}
}
