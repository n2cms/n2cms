using System.Web.UI;
using N2.Engine;
using System.Web;

namespace N2.Web.UI
{
	public static class PageExtensions
	{
		internal static IEngine GetEngine(this HttpContext context)
		{
			var engine = context.Items["N2.Engine"] as IEngine;
			if (engine != null)
				return engine;

			return N2.Context.Current;
		}

		/// <summary>
		/// Gets the engine from the page or falls back to static context.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public static IEngine GetEngine(this Page page)
		{
			var engine = page.Items["N2.Engine"] as IEngine;
			if (engine != null)
				return engine;

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
