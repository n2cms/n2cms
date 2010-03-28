using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Management.Myself;
using N2.Web.UI;
using N2.Edit.Wizard;
using N2.Edit.Wizard.Items;
using N2.Edit;
using N2;
using N2.Details;

namespace Management.N2.Myself
{
	[PartDefinition("Wizards", 
		TemplateUrl = "~/N2/Myself/Wizards.ascx",
		IconUrl = "~/N2/Resources/icons/wand.png")]
	[WithEditableTitle("Title", 10)]
	public partial class WizardsPart : RootPartBase
	{
		public override string Title
		{
			get { return base.Title ?? "Wizards"; }
			set { base.Title = value; }
		}
	}

	public partial class Wizards : ContentUserControl<ContentItem, WizardsPart>
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			var wizard = Engine.Resolve<LocationWizard>();
			rptLocations.DataSource = wizard.GetLocations();
			rptLocations.DataBind();

		}

		protected virtual string GetEditUrl(MagicLocation location)
		{
			return Engine.EditManager.GetEditNewPageUrl(location.Location, location.GetDefinition(Engine.Definitions), location.ZoneName, CreationPosition.Below);
		}
	}
}