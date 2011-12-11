using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Definitions;
using N2.Persistence.NH;
using N2.Persistence;

namespace N2.Management.Content.Export
{
	public partial class BulkEditing : EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			var discriminators = new HashSet<string>(Engine.Resolve<IContentItemRepository>()
				.FindDiscriminatorsBelow(Selection.SelectedItem)
				.Select(d => d.Discriminator));

			ddlTypes.DataSource = Engine.Definitions.GetDefinitions()
				.Where(d => discriminators.Contains(d.Discriminator))
				.OrderByDescending(d => typeof(ISystemNode).IsAssignableFrom(d.ItemType) ? 0 : 1)
				.ThenBy(d => d.IsPage ? 0 : 1)
				.ThenByDescending(d => d.NumberOfItems / 10)
				.ThenBy(d => d.SortOrder)
				.ThenBy(d => d.Title);
			ddlTypes.DataBind();
		}

		protected void ddlTypes_OnSelectedIndexChanged(object sender, EventArgs args)
		{
		}
	}
}