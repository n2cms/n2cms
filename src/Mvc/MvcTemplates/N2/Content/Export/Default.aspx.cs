using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.FileSystem;
using N2.Edit.Web;
using N2.Persistence.Serialization;
using N2.Security;
using N2.Web.UI;
using N2.Xml;
using N2.Persistence;

namespace N2.Edit.Export
{
	[ToolbarPlugin("IMPORT", "exportimport", "{ManagementUrl}/Content/Export/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Preview, Targets.Preview, "{ManagementUrl}/Resources/icons/package_come_and_go.png", 150, ToolTip = "export/import page data", 
		GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Administer)]
	public partial class Default : EditPage
	{
		protected N2.Web.UI.WebControls.TabPanel tpExport;

		#region Control Fields & Property

		protected IContentTemplate exportedItems;
		protected IContentTemplate importedItems;
		protected Repeater rptAttachments;

		#endregion

		#region Page Event Handlers

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			tpExport.NavigateUrl = "Export.aspx?selected=" + Selection.SelectedItem.Path;
		}

		#endregion

		#region Click Event Handlers

		protected void btnVerify_Click(object sender, EventArgs e)
		{
			var tempFile = Engine.Resolve<TemporaryFileHelper>()
				.GetTemporaryFileName(System.IO.Path.GetExtension(fuImport.PostedFile.FileName));
			fuImport.PostedFile.SaveAs(tempFile);

			if (fuImport.PostedFile.FileName.EndsWith(".csv"))
			{
				csv.ContinueWithImport(tempFile);
				uploadFlow.ActiveViewIndex = 2;
			}
			else
			{
				xml.ContinueWithImport(tempFile);
				uploadFlow.ActiveViewIndex = 1;
			}
		}

		protected void btnUploadImport_Click(object sender, EventArgs e)
		{
			xml.ImportNow(fuImport.PostedFile);
		}

		protected void btnImport_Click(object sender, EventArgs e)
		{
			uploadFlow.ActiveViewIndex = 0;
		}

		#endregion

	}
}