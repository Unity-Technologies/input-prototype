using System.Collections.Generic;

namespace UnityEngine.InputNew
{
	/// <summary>
	/// An object that exposes a specific configuration of input controls.
	/// </summary>
	public interface IInputControlProvider
	{
		////REVIEW: this should be readonly but not sure ReadOnlyCollection from .NET 2 is good enough
		IList< InputControlData > controls { get; }
	}
}