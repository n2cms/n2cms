#region License
/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Web.UI
{
	/// <summary>A DataSourceView that can perform queries against the N2 persistence manager.</summary>
	public class QueryDataSourceView : ItemDataSourceView
	{
		#region Constructors
		public QueryDataSourceView(Engine.IEngine engine, IDataSource owner, string viewName, N2.Persistence.Finder.IQueryEnding query)
		    : base(engine, owner, viewName)
		{
			this.query = query;
		}
		#endregion

		#region Private Members
		private N2.Persistence.Finder.IQueryEnding query;
		#endregion

		#region Properties
		public N2.Persistence.Finder.IQueryEnding Query
		{
			get { return query; }
			set { query = value; }
		}

		#endregion

		#region Methods
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
		{
			if (this.Query != null)
			{
			    ItemDataSourceSelectingEventArgs selectingArgs = new ItemDataSourceSelectingEventArgs(arguments);
			    OnSelecting(selectingArgs);

				IList<ContentItem> items = Query
					.FirstResult(selectingArgs.Arguments.StartRowIndex)
					.MaxResults(selectingArgs.Arguments.MaximumRows)
					.Select();

			    Collections.ItemListEventArgs args = new N2.Collections.ItemListEventArgs(new Collections.ItemList(items));
			    OnSelected(args);
			    return args.Items;
			}
			else
			    return null;
		}
		#endregion
	}
}
