using System;

namespace UnityEngine.InputNew
{
	[Serializable]
	public struct InputControlDescriptor
	{
		public int controlIndex;

		public Type deviceType
		{
			get
			{
				if (_cachedDeviceType == null)
					_cachedDeviceType = Type.GetType(_deviceTypeName);

				return _cachedDeviceType;
				;
			}
			set
			{
				_cachedDeviceType = value;
				_deviceTypeName = _cachedDeviceType.AssemblyQualifiedName;
			}
		}

		[SerializeField]
		string _deviceTypeName;

		Type _cachedDeviceType;

		public override string ToString()
		{
			return string.Format("(device:{0}, control:{1})", deviceType.Name, controlIndex);
		}
	}
}
