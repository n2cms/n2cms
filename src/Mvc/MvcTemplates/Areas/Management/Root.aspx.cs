using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Templates.Mvc.Areas.Management
{
	public partial class Root : N2.Edit.Web.EditPage, IContentTemplate, IItemContainer
	{
		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			sc.Visible = Engine.SecurityManager.IsAdmin(User);

			Title = CurrentItem["AlternativeTitle"] as string;
		}

		protected override void OnPreRender(System.EventArgs e)
		{
			base.OnPreRender(e);

			if (ControlPanel.GetState(this) != ControlPanelState.DragDrop)
			{
				HideIfEmpty(c1, Zone2.DataSource);
				HideIfEmpty(c2, Zone3.DataSource);
				HideIfEmpty(c3, Zone4.DataSource);
			}
		}

		private void HideIfEmpty(Control container, IList<ContentItem> items)
		{
			container.Visible = items != null && items.Count > 0;
		}

		#region IContentTemplate Members

		public ContentItem CurrentItem { get; set; }

		#endregion
	}
}
