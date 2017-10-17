using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.InputNew
{
	public class PlayerInput : MonoBehaviour
	{
        static PlayerInput s_Singleton;
        public static PlayerInput singleton
        {
            get
            {
                if (s_Singleton == null)
                {
                    var go = new GameObject("Player Input Singleton");
                    go.hideFlags = HideFlags.HideAndDontSave;
                    s_Singleton = go.AddComponent<PlayerInput>();
                    s_Singleton.handle.processAll = true;
                }
                return s_Singleton;
            }
        }

        // Should this player handle request assignment of an input device as soon as the component awakes?
        [FormerlySerializedAs("autoSinglePlayerAssign")]
		public bool autoAssignGlobal = true;

		public List<ActionMapSlot> actionMaps = new List<ActionMapSlot>();

		public PlayerHandle handle { get; set; }

		void Awake()
		{
			if (autoAssignGlobal)
			{
				handle = PlayerHandleManager.GetNewPlayerHandle();
				handle.global = true;
				foreach (ActionMapSlot actionMapSlot in actionMaps)
				{
					ActionMapInput actionMapInput = ActionMapInput.Create(actionMapSlot.actionMap);
					actionMapInput.TryInitializeWithDevices(handle.GetApplicableDevices());
					actionMapInput.active = actionMapSlot.active;
					actionMapInput.blockSubsequent = actionMapSlot.blockSubsequent;
					handle.maps.Add(actionMapInput);
				}
			}
		}

		public T GetActions<T>() where T : ActionMapInput
		{
			if (handle == null)
				return null;
			return handle.GetActions<T>();
		}
	}
}
