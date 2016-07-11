using System;

namespace UnityEngine.InputNew
{
	[Serializable]
	public class SerializableDeviceType
	{
		public SerializableType type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
			}
		}

		public int tagIndex
		{
			get
			{
				return m_TagIndex;
			}
			set
			{
				m_TagIndex = value;
			}
		}

		[SerializeField]
		private SerializableType m_Type;

		[SerializeField]
		private int m_TagIndex;

		public SerializableDeviceType Clone()
		{
			var clone = new SerializableDeviceType();
			clone.m_TagIndex = m_TagIndex;
			clone.m_Type = m_Type;

			return clone;
		}
	}
}