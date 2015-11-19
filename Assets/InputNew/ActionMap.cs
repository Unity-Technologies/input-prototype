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
		private List<InputAction> m_Actions = new List<InputAction>();
		public List<InputAction> actions { get { return m_Actions; } set { m_Actions = value; } }
		
		[SerializeField]
		private List<ControlScheme> m_ControlSchemes = new List<ControlScheme>();
		public List<ControlScheme> controlSchemes { get { return m_ControlSchemes; } set { m_ControlSchemes = value; } }
	}
}
