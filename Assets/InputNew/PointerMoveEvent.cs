using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public class PointerMoveEvent
		: PointerEvent
	{
		#region Public Properties

		public Vector3 delta { get; set; }

		#endregion
	}
}
