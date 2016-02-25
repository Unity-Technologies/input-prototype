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
		private List<SerializableType> m_SerializableDeviceTypes;
		public List<SerializableType> serializableDeviceTypes { get { return m_SerializableDeviceTypes; } set { m_SerializableDeviceTypes = value; } }
		public IEnumerable<Type> deviceTypes { get { return m_SerializableDeviceTypes.Select(e => e.value); } }

		[SerializeField]
		private ActionMap m_ActionMap;
		public ActionMap actionMap { get { return m_ActionMap; } }
		
		[SerializeField]
		private List<ControlBinding> m_Bindings = new List<ControlBinding> ();
		public List<ControlBinding> bindings { get { return m_Bindings; } set { m_Bindings = value; } }
		
		public ControlScheme(string name, ActionMap actionMap)
		{
			m_Name = name;
			m_ActionMap = actionMap;
		}

		public void ExtractDeviceTypesAndControlIndices (Dictionary<Type, List<int>> controlIndicesPerDeviceType)
		{
			foreach (var binding in bindings)
				binding.ExtractDeviceTypesAndControlIndices(controlIndicesPerDeviceType);
		}
	}
}
