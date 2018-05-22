using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.InputNew
{
	[CreateAssetMenu()]
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class ActionMap : ScriptableObject
	{
		public static readonly string kDefaultNamespace = "UnityEngine.InputNew";

		[FormerlySerializedAs("entries")]
		[SerializeField]
		private List<InputAction> m_Actions = new List<InputAction>();
		public List<InputAction> actions { get { return m_Actions; } set { m_Actions = value; } }
		
		[SerializeField]
		private List<ControlScheme> m_ControlSchemes = new List<ControlScheme>();
		private List<ControlScheme> m_ControlSchemeCopies; // In players or playmode we always hand out copies and retain the originals.
		public List<ControlScheme> controlSchemes
		{
			get
			{
#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
				if (!s_IsInPlayMode)
				{
					return m_ControlSchemes; // ActionMapEditor modifies this directly.
				}
#endif
				if (m_ControlSchemeCopies == null || m_ControlSchemeCopies.Count == 0)
				{
					m_ControlSchemeCopies = m_ControlSchemes.Select(x => x.Clone()).ToList();
#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
					s_ActionMapsToCleanUpAfterPlayMode.Add(this);
#endif
				}
				return m_ControlSchemeCopies;
			}

			set
			{
				m_ControlSchemes = value;
				m_ControlSchemeCopies = null;
			}
		}

		// In the editor, throw away all customizations when exiting playmode.
#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
		static List<ActionMap> s_ActionMapsToCleanUpAfterPlayMode = new List<ActionMap>();
		static bool s_IsInPlayMode;

		static ActionMap()
		{
			EditorApplication.delayCall += () => HandlePlayModeCustomizations(PlayModeStateChange.EnteredEditMode);
			EditorApplication.playModeStateChanged += HandlePlayModeCustomizations;
		}

		static void HandlePlayModeCustomizations(PlayModeStateChange stateChange)
		{
			if (stateChange == PlayModeStateChange.EnteredPlayMode)
			{
				s_IsInPlayMode = true;
			}
			else if (stateChange == PlayModeStateChange.ExitingPlayMode || stateChange == PlayModeStateChange.EnteredEditMode)
			{
				s_IsInPlayMode = false;

				// Throw away all the copies of ControlSchemes we made in play mode.
				foreach (var actionMap in s_ActionMapsToCleanUpAfterPlayMode)
				{
					actionMap.m_ControlSchemeCopies = null;
				}
				s_ActionMapsToCleanUpAfterPlayMode.Clear();
			}
		}
#endif

		public Type mapType
		{
			get
			{
				if ( m_CachedMapType == null )
				{
					if (m_MapTypeName == null)
						return null;
					m_CachedMapType = Type.GetType( m_MapTypeName );
				}
				return m_CachedMapType;
			}
			set
			{
				m_CachedMapType = value;
				m_MapTypeName = m_CachedMapType.AssemblyQualifiedName;
			}
		}
		[SerializeField]
		private string m_MapTypeName;
		private Type m_CachedMapType;
		public void SetMapTypeName(string name)
		{
			m_MapTypeName = name;
		}

		public Type customActionMapType {
			get
			{
				string typeString = string.Format(
					"{0}.{1}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
					string.IsNullOrEmpty(m_CustomNamespace) ? kDefaultNamespace : m_CustomNamespace,
                    name);
				Type t = null;
				try
				{
					t = Type.GetType(typeString);
					if (t == null)
					{
						typeString = string.Format("{0}.{1}",
							string.IsNullOrEmpty(m_CustomNamespace) ? kDefaultNamespace : m_CustomNamespace,
							name);

						var assemblies = AppDomain.CurrentDomain.GetAssemblies();
						foreach (var assembly in assemblies)
						{
							try
							{
								t = assembly.GetType(typeString);
								if (t != null)
									break;
							}
							catch (ReflectionTypeLoadException)
							{
								// Skip any assemblies that don't load properly -- suppress errors
							}
						}
					}
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

		[SerializeField]
		private string m_CustomNamespace;
		public string customNamespace
		{
			get
			{
				return m_CustomNamespace;
			}
			set
			{
				m_CustomNamespace = value;
			}
		}

		public string GetCustomizations()
		{
			var customizedControlSchemes = m_ControlSchemeCopies.Where(x => x.customized).ToList();
			return JsonUtility.ToJson(customizedControlSchemes);
		}

		public void RevertCustomizations()
		{
			m_ControlSchemeCopies = null;
		}

		public void RevertCustomizations(ControlScheme controlScheme)
		{
			if (m_ControlSchemeCopies != null)
			{
				for (var i = 0; i < m_ControlSchemeCopies.Count; ++i)
				{
					if (m_ControlSchemeCopies[i] == controlScheme)
					{
						m_ControlSchemeCopies[i] = m_ControlSchemes[i].Clone();
						break;
					}
				}
			}
		}

		public void RestoreCustomizations(string customizations)
		{
			var customizedControlSchemes = JsonUtility.FromJson<List<ControlScheme>>(customizations);
			foreach (var customizedScheme in customizedControlSchemes)
			{
				// See if it replaces an existing scheme.
				var replacesExisting = false;
				for (var i = 0; i < controlSchemes.Count; ++i)
				{
					if (String.Compare(controlSchemes[i].name, customizedScheme.name, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0)
					{
						// Yes, so get rid of current scheme.
						controlSchemes[i] = customizedScheme;
						replacesExisting = true;
						break;
					}
				}

				if (!replacesExisting)
				{
					// No, so add as new scheme.
					controlSchemes.Add(customizedScheme);
				}
			}
		}
	}
}
