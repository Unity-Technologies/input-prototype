using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.InputNew
{
	[CreateAssetMenu()]
	public class ActionMap : ScriptableObject
	{
		[FormerlySerializedAs("entries")]
		[SerializeField]
		private List<InputAction> m_Actions = new List<InputAction>();
		public List<InputAction> actions { get { return m_Actions; } set { m_Actions = value; } }
		
		[SerializeField]
		private List<ControlScheme> m_ControlSchemes = new List<ControlScheme>();
		public List<ControlScheme> controlSchemes { get { return m_ControlSchemes; } set { m_ControlSchemes = value; } }

		public Type custumActionMapType {
			get
			{
				string typeString = string.Format(
					"{0}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
					name);
				Type t = null;
				try
				{
					t = Type.GetType(typeString);
				}
				catch (Exception e)
				{
					throw new Exception("Failed to create type from string \"" + typeString + "\".", e);
				}

				if (t == null)
					throw new Exception("Failed to create type from string \"" + typeString + "\".");

				return t;
			}
		}
	}
}
