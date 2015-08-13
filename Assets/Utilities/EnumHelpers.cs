using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Utilities
{
	public static class EnumHelpers
	{
		public static int GetNameCount<TEnum>()
		{
			return Enum.GetNames(typeof(TEnum)).Length;
		}

		public static int GetValueCount<TEnum>()
		{
			// Slow...
			return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Distinct().Count();
		}
	}
}
