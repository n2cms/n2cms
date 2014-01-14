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

        public static SessionContext NH
        {
            get { return Context.Current.Resolve<ISessionProvider>().OpenSession; }
        }
    }
}
