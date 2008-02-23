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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using N2.Definitions;

namespace N2.Web.UI
{
	/// <summary>A data source view that provides the child items of a parent item.</summary>
	public class ChildrenDataSourceView : ItemDataSourceView
	{
		#region Private Fields
		private ContentItem parentItem = null;
		private Collections.ItemList allItems = null;
		private IEnumerable<Collections.ItemFilter> filters;
		#endregion

		#region Constructors
		public ChildrenDataSourceView(Engine.IEngine engine, IDataSource owner, string viewName, ContentItem parentItem)
			: base(engine, owner, viewName)
		{
			this.parentItem = parentItem;
		}		
		#endregion

		#region Properties
		public ContentItem ParentItem
		{
			get { return parentItem; }
			set
			{
				parentItem = value;
				this.allItems = null;
				this.OnDataSourceViewChanged(EventArgs.Empty);
			}
		}

		public IEnumerable<Collections.ItemFilter> Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		public override bool CanInsert
		{
			get { return parentItem != null; }
		}
		public override bool CanRetrieveTotalRowCount
		{
			get { return true; }
		}
		public override bool CanSort
		{
			get { return true; }
		}
		#endregion

		#region Methods
		#region On...
		protected virtual void OnItemCreated(Persistence.ItemEventArgs e)
		{
			EventHandler<Persistence.ItemEventArgs> handler = base.Events[EventItemCreated] as EventHandler<Persistence.ItemEventArgs>;
			if (handler != null)
				handler.Invoke(this, e);
		}
		protected virtual void OnItemCreating(Persistence.ItemEventArgs e)
		{
			EventHandler<Persistence.ItemEventArgs> handler = base.Events[EventItemCreating] as EventHandler<Persistence.ItemEventArgs>;
			if (handler != null)
				handler.Invoke(this, e);

			if (e.AffectedItem != null)
			{
				IDefinitionManager definitions = Engine.Definitions;
				ItemDefinition parentDefinition = definitions.GetDefinition(parentItem.GetType());

				if (parentDefinition.IsChildAllowed(parentDefinition))
				{
					e.AffectedItem = Engine.Definitions.CreateInstance(parentItem.GetType(), parentItem);
					return;
				}
				foreach (ItemDefinition definition in definitions.GetAllowedChildren(parentDefinition, null, HttpContext.Current.User))
				{
					e.AffectedItem = definitions.CreateInstance(definition.ItemType, parentItem);
					return;
				}
				throw new N2.Definitions.NoItemAllowedException(parentItem);
			}
		}
		#endregion

		private Collections.ItemList GetAllItems()
		{
			if (allItems == null && parentItem != null)
				allItems = parentItem.GetChildren();
			return allItems;
		}

		private string currentSortExpression = "";
		protected override System.Collections.IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
		{
			ItemDataSourceSelectingEventArgs selectingArgs = new ItemDataSourceSelectingEventArgs(arguments);
			OnSelecting(selectingArgs);
			Collections.ItemListEventArgs args = new N2.Collections.ItemListEventArgs(GetAllItems());
			if (args.Items == null)
				return null;
			
			if(selectingArgs.Arguments.RetrieveTotalRowCount)
				selectingArgs.Arguments.TotalRowCount = args.Items.Count;

			if (!string.IsNullOrEmpty(selectingArgs.Arguments.SortExpression) && selectingArgs.Arguments.SortExpression != currentSortExpression)
			{
				args.Items.Sort(new Collections.ItemComparer(selectingArgs.Arguments.SortExpression));
				currentSortExpression = selectingArgs.Arguments.SortExpression;
			}

			OnFiltering(args);
			if (Filters != null || (selectingArgs.Arguments.StartRowIndex >= 0 && selectingArgs.Arguments.MaximumRows > 0))
			{
				Collections.ItemList filteredItems = args.Items;
				
				if (Filters != null)
					filteredItems = new Collections.ItemList(filteredItems, Filters);

				if (selectingArgs.Arguments.StartRowIndex >= 0 && selectingArgs.Arguments.MaximumRows > 0)
					filteredItems = new Collections.ItemList(filteredItems, new Collections.CountFilter(selectingArgs.Arguments.StartRowIndex, selectingArgs.Arguments.MaximumRows));
				
				args = new N2.Collections.ItemListEventArgs(filteredItems);
			}

			OnSelected(args);
			return args.Items;
		}

		protected override int ExecuteInsert(System.Collections.IDictionary values)
		{
			if (parentItem == null)
				throw new ArgumentNullException("parentItem", "Can't insert item since we have no parent item to insert below");

			Persistence.ItemEventArgs args = new N2.Persistence.ItemEventArgs(null);
			OnItemCreating(args);
			OnItemCreated(args);

			if (args.AffectedItem != null)
			{
				// Insert new values
				args.AffectedItem.Parent = parentItem;
				foreach (string propertyName in values.Keys)
					args.AffectedItem[propertyName] = values[propertyName];

				OnInserting(args);
				Engine.Persister.Save(args.AffectedItem);
				GetAllItems().Add(args.AffectedItem);
				OnInserted(args);
				OnDataSourceViewChanged(EventArgs.Empty);

				return 1;
			}
			return 0;
		}
 
		#endregion	

		#region Events
		public event EventHandler<Persistence.ItemEventArgs> ItemCreated
		{
			add { base.Events.AddHandler(EventItemCreated, value); }
			remove { base.Events.RemoveHandler(EventItemCreated, value); }
		}
		public event EventHandler<Persistence.ItemEventArgs> ItemCreating
		{
			add { base.Events.AddHandler(EventItemCreating, value); }
			remove { base.Events.RemoveHandler(EventItemCreating, value); }
		}
		#endregion

		#region Static Event Keys
		private static readonly object EventItemCreated = new object();
		private static readonly object EventItemCreating = new object();
		#endregion	
	}
}
