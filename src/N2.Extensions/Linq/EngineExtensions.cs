using System.Linq;
using N2.Engine;
using N2.Persistence.NH;
using N2.Details;
using NHibernate.Linq;

namespace N2.Linq
{
	public static class EngineExtensions
	{
		public static IQueryable<T> Query<T>(this IEngine engine)
		{
			if(typeof(ContentItem).IsAssignableFrom(typeof(T)))
				return new ContentQueryable<T>(engine.Resolve<ISessionProvider>().OpenSession.Session.Linq<T>());
			return engine.Resolve<ISessionProvider>().OpenSession.Session.Linq<T>();
		}

		public static IQueryable<T> QueryItems<T>(this IEngine engine) where T : ContentItem
		{
			return engine.Query<T>();
		}
		public static IQueryable<ContentItem> QueryItems(this IEngine engine)
		{
			return engine.Query<ContentItem>();
		}
		public static IQueryable<T> QueryDetails<T>(this IEngine engine) where T : ContentDetail
		{
			return engine.Query<T>();
		}
		public static IQueryable<DetailCollection> QueryDetailCollections(this IEngine engine)
		{
			return engine.Query<DetailCollection>();
		}
	}
}
