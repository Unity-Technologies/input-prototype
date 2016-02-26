using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.InputNew
{
	[Serializable]
	public class ControlScheme
	{
		[SerializeField]
		private string m_Name;
		public string name { get { return m_Name; } set { m_Name = value; } }

		[SerializeField]
		private ActionMap m_ActionMap;
		public ActionMap actionMap { get { return m_ActionMap; } }
		
		[SerializeField]
		private List<ControlBinding> m_Bindings = new List<ControlBinding> ();
		public List<ControlBinding> bindings { get { return m_Bindings; } set { m_Bindings = value; } }

		public bool customized { get; internal set; }

		public ControlScheme()
		{
		}
		
		public ControlScheme(string name, ActionMap actionMap)
		{
			m_Name = name;
			m_ActionMap = actionMap;
		}

		public virtual ControlScheme Clone()
		{
			var clone = (ControlScheme) Activator.CreateInstance(GetType());
			clone.m_Name = m_Name;
			clone.m_ActionMap = m_ActionMap;
			clone.m_Bindings = m_Bindings.Select(x => x.Clone()).ToList();
			// Don't clone customized flag.
			return clone;
		}
		
		public IEnumerable<Type> GetUsedDeviceTypes()
		{
			if (bindings == null)
				return Enumerable.Empty<Type>();
			
			var deviceTypes = new HashSet<Type>();
			foreach (var binding in bindings)
			{
				foreach (var source in binding.sources)
				{
					deviceTypes.Add(source.deviceType);
				}
				
				foreach (var source in binding.buttonAxisSources)
				{
					deviceTypes.Add(source.negative.deviceType);
					deviceTypes.Add(source.positive.deviceType);
				}
			}
			
			return deviceTypes;
		}

		public int GetDevicesHash()
		{
			// Note: Xor is associative, so order doesn't matter (when no shifting is involved).
			return GetUsedDeviceTypes().Aggregate(0, (result, element) => result ^ element.GetHashCode());
		}
		
		public void ExtractDeviceTypesAndControlIndices (Dictionary<Type, List<int>> controlIndicesPerDeviceType)
		{
			foreach (var binding in bindings)
				binding.ExtractDeviceTypesAndControlIndices(controlIndicesPerDeviceType);
		}
	}
}
