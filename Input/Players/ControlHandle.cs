using System;
using UnityEngine;

namespace UnityEngine.InputNew
{
	// We need a non-generic base class so that we can create a PropertyDrawer for it.
	public abstract class ControlHandle {}

	public class ControlHandle<T> : ControlHandle where T : InputControl
	{
	    T m_Control;

		public InputAction action;
        public T control
        {
            get
            {
                if (m_Control == null)
                {
                    m_Control = ActionMapInput.GetSingletonInstance(action.actionMap)[action.actionIndex] as T;
                }
                return m_Control;
            }
        }

        public void Bind(PlayerHandle player)
		{
			var map = player.GetActions(action.actionMap);
            m_Control = map[action.actionIndex] as T;
		}
	}
}
