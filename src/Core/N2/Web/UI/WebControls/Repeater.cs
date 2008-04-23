using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;

namespace N2.Web.UI.WebControls
{
	public class Repeater : System.Web.UI.WebControls.Repeater
	{
		private ITemplate emptyTemplate = null;

		[DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(System.Web.UI.WebControls.RepeaterItem)), Browsable(false)]
		public virtual ITemplate EmptyTemplate
		{
			get { return this.emptyTemplate; }
			set { this.emptyTemplate = value; }
		}
		protected override void CreateControlHierarchy(bool useDataSource)
		{
			base.CreateControlHierarchy(useDataSource);

			if (this.Controls.Count == 0 && EmptyTemplate != null)
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
	}
}
