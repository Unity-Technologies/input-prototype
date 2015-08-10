using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	public interface IInputConsumer
	{
		string name { get; }

		IList< IInputConsumer > children { get; }

		ProcessInputDelegate processInput { get; set; }
	}
}