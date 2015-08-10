using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.InputNew
{
	public class ControlMap
		: ScriptableObject
	{
		public List< ControlMapEntry > entries;
		public List< string > schemes;

		public IEnumerable< Type > GetUsedDeviceType( int controlSchemeIndex )
		{
			if ( entries == null )
				return Enumerable.Empty< Type >();

			var deviceTypes = new HashSet< Type >();
			foreach ( var entry in entries )
			{
				var binding = entry.bindings[ controlSchemeIndex ];

				foreach ( var source in binding.sources )
					deviceTypes.Add( source.deviceType );

				////TODO: button axes
			}

			return deviceTypes;
		}
	}
}