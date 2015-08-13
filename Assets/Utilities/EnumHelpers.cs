using System;
using System.Linq;

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
