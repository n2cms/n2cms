using System;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Web;
using N2.Security;

namespace N2.Management.Content.Templates
{
    [ToolbarPlugin("TEMPL", "templates", "{ManagementUrl}/Content/Templates/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Options, Targets.Preview, "{ManagementUrl}/Resources/icons/page_white_swoosh.png", 56,
        ToolTip = "create items with default content",
        GlobalResourceClassName = "Toolbar",
        RequiredPermission = Permission.Write,
        Legacy = true)]
    public partial class Default : EditPage
    {
        protected ContentTemplateRepository Templates { get; set; }
        protected IDefinitionManager Definitions { get; set; }
        protected IEditUrlManager Edits { get; set; }
        
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            Templates = Engine.Resolve<ContentTemplateRepository>();
            Definitions = Engine.Resolve<IDefinitionManager>();
            Edits = Engine.Resolve<IEditUrlManager>();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DataBind();
        }

        protected void gvTemplates_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Templates.RemoveTemplate((string)gvTemplates.DataKeys[e.RowIndex][0]);
            DataBind();
        }

        protected void btnAdd_Command(object sender, CommandEventArgs args)
        {
            var template = Selection.SelectedItem.Clone(false);
			var children = Selection.SelectedItem.Children.WhereAccessible();
            for (int i = 0; i < children.Count; i++)
            {
                CheckBox chkChildren = (CheckBox)rptChildren.Items[i].FindControl("chkChildren");
                if (chkChildren.Checked)
                {
                    children[i].Clone(true).AddTo(template);
                }
                
            }
            template.Title = txtTitle.Text;
            template[ContentTemplateRepository.TemplateDescription] = txtDescription.Text;
            Templates.AddTemplate(template);

			Response.Redirect("Default.aspx?template" + template.Path + "#" + tpTemplates.ClientID);
        }
    }
}
