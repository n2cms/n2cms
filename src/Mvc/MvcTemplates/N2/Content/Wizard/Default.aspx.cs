using System;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit.Wizard.Items;

namespace N2.Edit.Wizard
{
    [ToolbarPlugin("WIZARD", "wizard", "{ManagementUrl}/Content/Wizard/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Options, Targets.Preview, "{ManagementUrl}/Resources/icons/wand.png", 55, 
        ToolTip = "create items in default locations", 
        GlobalResourceClassName = "Toolbar",
        OptionProvider = typeof(WizardOptionProvider),
        Legacy = true)]
    public partial class Default : Web.EditPage
    {
        protected TextBox txtTitle;
        protected DropDownList ddlTypes;

        protected IDefinitionManager Definitions;
        protected IEditUrlManager Edits;
        protected LocationWizard Wizard;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            Definitions = Engine.Definitions;
            Edits = Engine.ManagementPaths;
            Wizard = Engine.Resolve<LocationWizard>();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadAdd();
            LoadLocations();
        }

        private void LoadAdd()
        {
            lblLocationTitle.DataBind();
            txtTitle.Text = Selection.SelectedItem.Title;
            ddlTypes.DataSource = Definitions.GetAllowedChildren(Selection.SelectedItem, null)
                .WhereAuthorized(Engine.SecurityManager, User, Selection.SelectedItem)
                .SelectMany(d =>
                    {
                        return new [] { new 
                        {
                            Value = d.Discriminator,
                            Title = d.Title
                        }}.Union(Engine.Resolve<ITemplateAggregator>().GetTemplates(d.ItemType).Select(t =>
                            new 
                            {
                                Value = d.Discriminator + ":" + t.Name,
                                Title = "*  " + t.Title
                            }));
                    });
            ddlTypes.DataBind();
        }

        private void LoadLocations()
        {
            gvLocations.DataSource = Wizard.GetLocations();
            gvLocations.DataBind();
        }

        protected virtual string GetEditUrl(MagicLocation location)
        {
            if (location.Location == null)
                return null;

            return Edits.GetEditNewPageUrl(location.Location, 
                location.GetDefinition(Definitions), 
                location.ZoneName, 
                CreationPosition.Below) + "&template=" + location.ContentTemplate;
        }

        // list

        protected void gvLocations_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Wizard.RemoveLocation((int)gvLocations.DataKeys[e.RowIndex].Value);
            LoadLocations();
        }

        // add

        protected virtual void btnAdd_Command(object  sender, CommandEventArgs args)
        {
            string[] selection = ddlTypes.SelectedValue.Split(':');
            Wizard.AddLocation(Selection.SelectedItem, selection[0], selection.Length > 1 ? selection[1] : null, txtTitle.Text, string.Empty);

            Response.Redirect("Default.aspx#" + tpType.ClientID);
        }

    }
}
