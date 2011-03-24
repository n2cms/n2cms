using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinamico
{
	public static class Detaults
	{
		public static class Containers
		{
			public const string Content = "Content";
			public const string Site = "Site";
			public const string Advanced = "Advanced";
		}

		public static string ImageSize(string preferredSize, string fallbackToZoneNamed)
		{
			if (string.IsNullOrEmpty(preferredSize))
				return ImageSize(fallbackToZoneNamed);
			return preferredSize;
		}

		public static string ImageSize(string zoneName)
		{
			switch (zoneName)
			{
				case "SliderArea":
				case "PreContent":
				case "PostContent":
					return "wide";
				default:
					return "half";
			}
		}
	}
}