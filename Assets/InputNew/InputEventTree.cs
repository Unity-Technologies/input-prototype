using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	class InputEventTree
		: IInputConsumer
	{
		#region Fields

		readonly List<IInputConsumer> m_Children = new List<IInputConsumer>();
		public bool isStack { get; set; }

		#endregion

		#region Public Methods

		public bool ProcessEvent(InputEvent inputEvent)
		{
			return ProcessEventRecursive(this, inputEvent);
		}

		#endregion

		#region Non-Public Methods

		protected static bool ProcessEventRecursive(IInputConsumer consumer, InputEvent inputEvent)
		{
			var callback = consumer.processInput;
			if (callback != null)
			{
				if (callback(inputEvent))
					return true;
			}

			// Iterate in reverse order to get stack behavior.
			if (consumer.isStack)
			{
				for (int i = consumer.children.Count - 1; i >= 0; i--)
				{
					if (ProcessEventRecursive(consumer.children[i], inputEvent))
						return true;
				}
			}
			else
			{
				for (int i = 0; i < consumer.children.Count; i++)
				{
					if (ProcessEventRecursive(consumer.children[i], inputEvent))
						return true;
				}
			}

			return false;
		}

		internal void BeginNewFrame()
		{
			BeginNewFrame(this);
		}
		
		private void BeginNewFrame(IInputConsumer consumer)
		{
			var callback = consumer.beginNewFrame;
			if (callback != null)
				callback();

			foreach (var child in consumer.children)
				BeginNewFrame(child);
		}

		#endregion

		#region Public Properties

		public string name { get; set; }

		public IList<IInputConsumer> children
		{
			get { return m_Children; }
		}

		public ProcessInputDelegate processInput { get; set; }

		public BeginNewFrameDelegate beginNewFrame { get; set; }

		#endregion
	}
}
