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
		private List<SerializableDeviceType> m_SerializableDeviceTypes = new List<SerializableDeviceType>();
		public List<SerializableDeviceType> serializableDeviceTypes { get { return m_SerializableDeviceTypes; } set { m_SerializableDeviceTypes = value; } }
		
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
			clone.m_SerializableDeviceTypes = m_SerializableDeviceTypes.Select(x => x.Clone()).ToList();            
			clone.m_ActionMap = m_ActionMap;
			clone.m_Bindings = m_Bindings.Select(x => x.Clone()).ToList();
			// Don't clone customized flag.
			return clone;
		}

		public void ExtractDeviceTypesAndControlIndices (Dictionary<Type, List<int>> controlIndicesPerDeviceType)
		{
			foreach (var binding in bindings)
				binding.ExtractDeviceTypesAndControlIndices(controlIndicesPerDeviceType);
		}
	}
}
