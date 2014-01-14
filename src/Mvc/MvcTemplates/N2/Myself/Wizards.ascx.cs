using System;
using N2.Details;
using N2.Edit;
using N2.Edit.Wizard;
using N2.Edit.Wizard.Items;
using N2.Web.UI;

namespace N2.Management.Myself
{
    [PartDefinition("Wizards", 
        TemplateUrl = "{ManagementUrl}/Myself/Wizards.ascx",
        IconUrl = "{ManagementUrl}/Resources/icons/wand.png")]
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
            return Engine.ManagementPaths.GetEditNewPageUrl(location.Location, location.GetDefinition(Engine.Definitions), location.ZoneName, CreationPosition.Below);
        }
    }
}
