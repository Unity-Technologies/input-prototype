using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.InputNew
{
	public class ActionMap : ScriptableObject
	{
		[FormerlySerializedAs("entries")]
		[SerializeField]
		private List<InputAction> m_Actions;
		public List<InputAction> actions { get { return m_Actions; } set { m_Actions = value; } }
		
		[FormerlySerializedAs("schemes")]
		[SerializeField]
		private List<string> m_Schemes;
		public List<string> schemes { get { return m_Schemes; } set { m_Schemes = value; } }

		public void OnEnable()
		{
			if (actions != null)
			{
				for (var i = 0; i < actions.Count; ++ i)
				{
					actions[i].controlIndex = i;
				}
			}
		}

		public IEnumerable<Type> GetUsedDeviceTypes(int controlSchemeIndex)
		{
			if (actions == null)
				return Enumerable.Empty<Type>();

			var deviceTypes = new HashSet<Type>();
			foreach (var entry in actions)
			{
				if (controlSchemeIndex >= entry.bindings.Count)
					continue;

				var binding = entry.bindings[controlSchemeIndex];

				foreach (var source in binding.sources)
				{
					deviceTypes.Add(source.deviceType);
				}

				////TODO: button axes
			}

			return deviceTypes;
		}
	}
}
