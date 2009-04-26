using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
	public class Repeater : System.Web.UI.WebControls.Repeater
	{
		private ITemplate emptyTemplate = null;
		int itemsCreated;
		
		[DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(System.Web.UI.WebControls.RepeaterItem)), Browsable(false)]
		public virtual ITemplate EmptyTemplate
		{
			get { return this.emptyTemplate; }
			set { this.emptyTemplate = value; }
		}
		protected override void CreateControlHierarchy(bool useDataSource)
		{
			itemsCreated = 0;
			base.CreateControlHierarchy(useDataSource);

			if (itemsCreated == 0 && EmptyTemplate != null)
			{
				System.Web.UI.WebControls.RepeaterItem ri = new System.Web.UI.WebControls.RepeaterItem(0, System.Web.UI.WebControls.ListItemType.Header);
				EmptyTemplate.InstantiateIn(ri);
				Controls.Add(ri);
				OnItemCreated(new System.Web.UI.WebControls.RepeaterItemEventArgs(ri));
				if (useDataSource)
				{
					ri.DataBind();
					OnItemDataBound(new System.Web.UI.WebControls.RepeaterItemEventArgs(ri));
				}
			}
		}

		protected override void OnItemCreated(System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			base.OnItemCreated(e);

			switch (e.Item.ItemType)
			{	
				case ListItemType.Footer:
				case ListItemType.Header:
				case ListItemType.Pager:
				case ListItemType.Separator:
					break;
				default:
					itemsCreated++;
					break;
			}
		}
	}
}
