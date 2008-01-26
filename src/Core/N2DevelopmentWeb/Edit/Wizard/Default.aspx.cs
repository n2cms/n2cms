using System;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit.Wizard.Items;

namespace N2.Edit.Wizard
{
	[ToolbarPlugIn("", "wizard", "~/Edit/Wizard/Default.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/Wizard/Img/wand.png", 5, ToolTip = "create items in default locations")]
	public partial class Default : Web.EditPage
	{
		protected TextBox txtTitle;
		protected DropDownList ddlTypes;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			hlCancel.NavigateUrl = SelectedItem.Url;

			txtTitle.Text = SelectedItem.Title;
			ItemDefinition d = Engine.Definitions.GetDefinition(SelectedItem.GetType());
			ddlTypes.DataSource = d.AllowedChildren;
			ddlTypes.DataBind();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			LoadLocations();
		}

		private void LoadLocations()
		{
			rptLocations.DataSource = Wizard.GetLocations();
			rptLocations.DataBind();
		}

		private LocationWizard Wizard
		{
			get { return N2.Context.Instance.Resolve<LocationWizard>(); }
		}

		protected virtual string GetEditUrl(MagicLocation location)
		{
			return Engine.EditManager.GetEditNewPageUrl(location.Location, location.GetDefinition(Engine.Definitions), location.ZoneName, CreationPosition.Below);
		}

		protected virtual void btnAdd_Command(object  sender, CommandEventArgs args)
		{
			Wizard.AddLocation(SelectedItem, ddlTypes.SelectedValue, txtTitle.Text, string.Empty);
			LoadLocations();
		}
	}
}
