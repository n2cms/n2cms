using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Edit.FileSystem.Items;
using N2.Edit.Web;
using N2.Resources;

namespace N2.Edit.FileSystem
{
    public partial class File1 : EditPage
    {
		protected IEnumerable<ContentItem> ancestors;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Engine.Resolve<EditSection>().FileSystem.IsTextFile(SelectedFile.Url))
			{
				btnEdit.Visible = true;
			}

			ancestors = Find.EnumerateParents(Selection.SelectedItem, null, true).Where(a => a is AbstractNode).Reverse();

			DataBind();

			Refresh(Selection.SelectedItem, ToolbarArea.Navigation);
		}

		protected override void RegisterToolbarSelection()
		{
			string script = GetToolbarSelectScript("preview");
			Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
		}

        protected File SelectedFile
        {
            get { return Selection.SelectedItem as File; }
        }

    	protected void OnDownloadCommand(object sender, CommandEventArgs e)
    	{
			Response.ContentType = "application/octet-stream";
			Response.AppendHeader("Content-disposition", "attachment; filename=" + SelectedFile.Name);
    		SelectedFile.TransmitTo(Response.OutputStream);
			Response.End();
		}

		protected void OnEditCommand(object sender, CommandEventArgs e)
		{
			txtContent.Text = SelectedFile.ReadFile();
			btnEdit.Visible = false;
			btnDownload.Visible = false;
			txtContent.Visible = true;
			btnSave.Visible = true;
			btnCancel.Visible = true;
		}

		protected void OnSaveCommand(object sender, CommandEventArgs e)
		{
			SelectedFile.WriteFile(txtContent.Text);
			OnCancelCommand(sender, e);
		}

		protected void OnCancelCommand(object sender, CommandEventArgs e)
		{
			btnEdit.Visible = true;
			btnDownload.Visible = true;
			txtContent.Visible = false;
			btnSave.Visible = false;
			btnCancel.Visible = false;
		}
    }
}
