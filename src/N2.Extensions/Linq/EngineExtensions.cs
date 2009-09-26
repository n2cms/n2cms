using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.NH;
using N2.Details;

namespace N2.Linq
{
	public static class EngineExtensions
	{
		public static IQueryable<T> ContentItems<T>(this IEngine engine) where T : ContentItem
		{
			return new ContentContext(engine.Resolve<ISessionProvider>()).Elements<T>();
		}
		public static IQueryable<ContentItem> ContentItems(this IEngine engine)
		{
			return engine.ContentItems<ContentItem>();
		}
		public static IQueryable<T> ContentDetails<T>(this IEngine engine) where T : ContentDetail
		{
			return new ContentContext(engine.Resolve<ISessionProvider>()).Elements<T>();
		}
	}
}
