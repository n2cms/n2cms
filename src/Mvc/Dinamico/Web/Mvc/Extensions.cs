using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	public static class Extensions
	{
		public static ThemeViewEngine<T> RegisterThemeViewEngine<T>(this ViewEngineCollection viewEngines) where T : VirtualPathProviderViewEngine, new()
		{
			var tve = new ThemeViewEngine<T>();
			viewEngines.Insert(0, tve);
			return tve;
		}

		public static void WrapForTemplateRegistration(this ViewEngineCollection viewEngines)
		{
			for (int i = 0; i < viewEngines.Count; i++)
			{
				viewEngines[i] = new RegistrationViewEngineDecorator(viewEngines[i]);
			}
		}
	}
}