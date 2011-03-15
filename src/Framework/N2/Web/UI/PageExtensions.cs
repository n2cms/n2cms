using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using N2.Engine;

namespace N2.Web.UI
{
	public static class PageExtensions
	{
		public static IEngine GetEngine(this Page page)
		{
			var engineProvider = page as IProvider<IEngine>;
			if (engineProvider != null)
				return engineProvider.Get();

			return N2.Context.Current;
		}

		public static T ResolveService<T>(this Page page) where T: class
		{
			return page.GetEngine().Resolve<T>();
		}
	}
}
