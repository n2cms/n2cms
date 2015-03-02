using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using Dinamico.Models;
using N2.Definitions;

namespace Dinamico
{
    public static class Defaults
    {
        public static class Containers
        {
            public const string Metadata = "Metadata";
            public const string Content = "Content";
            public const string Site = "Site";
            public const string Advanced = "Advanced";
        }

		public static string ContentClass(string zoneName)
		{
			switch (zoneName)
			{
				case "SliderArea":
					return "carousel-caption";
				default:
					return "content";
			}
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


		/// <summary>
		/// Picks the translation best matching the browser-language or the first translation in the list
		/// </summary>
		/// <param name="request"></param>
		/// <param name="currentPage"></param>
		/// <returns></returns>
		public static ContentItem SelectLanguage(this HttpRequestBase request, ContentItem currentPage)
		{
			var start = Find.ClosestOf<IStartPage>(currentPage) ?? N2.Find.StartPage;
			if (start == null) return null;

			if (start is LanguageIntersection)
			{
				var translations = GetTranslations(currentPage).ToList();

				if (request.UserLanguages == null)
					return translations.FirstOrDefault();

				var selectedlanguage = request.UserLanguages.Select(ul => translations.FirstOrDefault(t => t.LanguageCode == ul))
					.Where(t => t != null)
					.FirstOrDefault();
				return selectedlanguage ?? translations.FirstOrDefault();
			}

			return start;
		}

		private static IEnumerable<StartPage> GetTranslations(ContentItem currentPage)
		{
			return currentPage.Children.FindPages().OfType<StartPage>();
		}
    }
}
