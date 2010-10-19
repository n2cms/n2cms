using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Edit.Web;
using N2.Edit.Templating;
using N2.Definitions;

namespace N2.Management.Content.Templates
{
	[ToolbarPlugin("TEMPL", "templates", "Content/Templates/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/N2/Resources/icons/page_white_swoosh.png", 56,
		ToolTip = "create items with default content",
		GlobalResourceClassName = "Toolbar")]
	public partial class Default : EditPage
	{
		protected ITemplateRepository Templates { get; set; }
		protected IDefinitionManager Definitions { get; set; }
		protected IEditManager Edit { get; set; }
		
		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			Templates = Engine.Resolve<ITemplateRepository>();
			Definitions = Engine.Resolve<IDefinitionManager>();
			Edit = Engine.Resolve<IEditManager>();
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
			var children = Selection.SelectedItem.GetChildren();
			for (int i = 0; i < children.Count; i++)
			{
				CheckBox chkChildren = (CheckBox)rptChildren.Items[i].FindControl("chkChildren");
				if (chkChildren.Checked)
				{
					children[i].Clone(true).AddTo(template);
				}
				
			}
			template.Title = txtTitle.Text;
			template[TemplateRepository.TemplateDescription] = txtDescription.Text;
			Templates.AddTemplate(template);

			Response.Redirect(Selection.SelectedItem.Url);
		}
	}
}
