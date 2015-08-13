using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputNew
{
	public interface IInputConsumer
	{
		string name { get; }
		IList<IInputConsumer> children { get; }
		ProcessInputDelegate processInput { get; set; }
		BeginNewFrameDelegate beginNewFrame { get; set; }
	}
}
