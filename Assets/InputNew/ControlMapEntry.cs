using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public class ControlMapEntry
		: ScriptableObject
	{
		public InputControlData controlData
		{
			get { return _controlData; }
			set
			{
				_controlData = value;
				name = _controlData.name;
			}
		}
		// This is one entry for each control scheme (matching indices).
		public List< ControlBinding > bindings;
		[ SerializeField ]
		private InputControlData _controlData;
	}
}