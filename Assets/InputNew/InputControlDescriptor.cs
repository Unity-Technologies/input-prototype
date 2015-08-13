using System;

namespace UnityEngine.InputNew
{
	[ Serializable ]
	public struct InputControlDescriptor
	{
		public int controlIndex;

		public Type deviceType
		{
			get
			{
				if ( m_CachedDeviceType == null )
					m_CachedDeviceType = Type.GetType( m_DeviceTypeName );

				return m_CachedDeviceType;;
			}
			set
			{
				m_CachedDeviceType = value;
				m_DeviceTypeName = m_CachedDeviceType.AssemblyQualifiedName;
			}
		}

		[ SerializeField ]
		private string m_DeviceTypeName;

		private Type m_CachedDeviceType;
		
		public override string ToString ()
		{
			return string.Format( "(device:{0}, control:{1})", deviceType.Name, controlIndex );
		}
	}
}