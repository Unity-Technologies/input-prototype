using System;

namespace UnityEngine.InputNew
{
	// ------------------------------------------------------------------------
	//	Events.
	// ------------------------------------------------------------------------

	public abstract class InputEvent
	{
		#region Public Methods

		public override string ToString()
		{
			if ( deviceType == null )
				return base.ToString();

			return string.Format
				(
					  "{0} on {1}:{2} at {3}"
					, GetType().Name
					, deviceType.Name
					, deviceIndex
					, time
				);
		}

		#endregion

		#region Public Properties

		public float time { get; set; }
		public Type deviceType { get; set; }
		public int deviceIndex { get; set; }

		public InputDevice device
		{
			get
			{
				if (_cachedDevice == null && deviceType != null)
					_cachedDevice = InputSystem.LookupDevice (deviceType, deviceIndex);

				return _cachedDevice;
			}
		}

		#endregion

		internal void Reset ()
		{
			time = 0.0f;
			deviceType = null;
			deviceIndex = 0;
			_cachedDevice = null;
		}

		private InputDevice _cachedDevice;
	}
}


// -------- from old single file thing


	////REVIEW: we may want to store actual state for compounds such that we can do postprocessing on them (like normalize vectors, for example)

	// ------------------------------------------------------------------------
	//	Devices.
	// ------------------------------------------------------------------------
	
	////TODO: how deal with compound devices (e.g. gamepads that also have a touchscreen)?
	////	create a true CompoundDevice class that is a collection of InputDevices?

	////FIXME: currently compounds go in the same array as primitives and thus lead to allocation of state which is useless for them

	////REVIEW: have a single Pointer class representing the union of all types of pointer devices or have multiple specific subclasses?
	////	also: where to keep the state for "the one" pointer

	// ------------------------------------------------------------------------
	//	Bindings.
	// ------------------------------------------------------------------------
	
	// Three different naming approaches:
	// 1. ControlMap, ControlMapEntry
	// 2. InputActionMap, InputAction
	// 3. InputActivityMap, InputActivity

	////NOTE: this needs to be proper asset stuff; can't be done in script code only

	// ------------------------------------------------------------------------
	//	System.
	// ------------------------------------------------------------------------
