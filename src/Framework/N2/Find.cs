#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */
#endregion

using System.Linq;
using N2.Linq;
using NHibernate;
using N2.Persistence.NH;

namespace N2
{
	/// <summary>
	/// Provides easy access to finder and commonly used items.
	/// </summary>
	public sealed class Find : Persistence.GenericFind<ContentItem,ContentItem>
	{
		public static IQueryable<ContentItem> Query()
		{
			return Context.Current.QueryItems();
		}

		public static IQueryable<T> Query<T>()
		{
			return Context.Current.Query<T>();
		}

		public static class NH
		{
			public static ISession Session()
			{
				return Context.Current.Resolve<ISessionProvider>().OpenSession.Session;
			}

			public static ICriteria Criteria<T>() where T : class
			{
				return Session().CreateCriteria<T>();
			}

			public static IMultiCriteria MultiCriteria<T>()
			{
				return Session().CreateMultiCriteria();
			}

			public static IQuery Query(string queryString)
			{
				return Session().CreateQuery(queryString);
			}

			public static IMultiQuery MultiQuery()
			{
				return Session().CreateMultiQuery();
			}
		}
	}
}
