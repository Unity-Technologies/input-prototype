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
		private List<InputAction> m_Entries;
		public List<InputAction> entries { get { return m_Entries; } set { m_Entries = value; } }
		
		[FormerlySerializedAs("schemes")]
		[SerializeField]
		private List<string> m_Schemes;
		public List<string> schemes { get { return m_Schemes; } set { m_Schemes = value; } }

		public void OnEnable()
		{
			if (entries != null)
			{
				for (var i = 0; i < entries.Count; ++ i)
				{
					entries[i].controlIndex = i;
				}
			}
		}

		public IEnumerable<Type> GetUsedDeviceTypes(int controlSchemeIndex)
		{
			if (entries == null)
				return Enumerable.Empty<Type>();

			var deviceTypes = new HashSet<Type>();
			foreach (var entry in entries)
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
