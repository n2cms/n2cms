using System;
using System.IO;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Persistence.Serialization;
using N2.Xml;
using N2.Web.UI;
using System.Text;
using N2.Edit.FileSystem;

namespace N2.Edit.Export
{
    [ToolbarPlugin("EXPORT", "exportimport", "Content/Export/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "{ManagementUrl}/Resources/icons/package_come_and_go.png", 150, ToolTip = "export/import page data", AuthorizedRoles = new string[] { "Administrators", "Admin" }, GlobalResourceClassName = "Toolbar")]
	public partial class Default : EditPage
	{
		protected Control tpExport;

		#region Control Fields & Property

		protected IContentTemplate exportedItems;
		protected IContentTemplate importedItems;
		protected Repeater rptAttachments;

		public string UploadedFilePath
		{
			get { return (string) (ViewState["UploadedFilePath"] ?? ""); }
			set { ViewState["UploadedFilePath"] = value; }
		}

		#endregion

		#region Page Event Handlers

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

            exportedItems.CurrentItem = Selection.SelectedItem;
			tpExport.DataBind();
		}

		#endregion

		#region Click Event Handlers

		protected void btnExport_Command(object sender, CommandEventArgs e)
		{
            ExportOptions options = ExportOptions.Default;
            if (chkDefinedDetails.Checked)
                options |= ExportOptions.OnlyDefinedDetails;
            if (chkAttachments.Checked)
                options |= ExportOptions.ExcludeAttachments;

            Engine.Resolve<Exporter>().Export(Selection.SelectedItem, options, Response);
		}

		protected void btnVerify_Click(object sender, EventArgs e)
		{
			UploadedFilePath = System.IO.Path.GetTempFileName() + System.IO.Path.GetExtension(fuImport.PostedFile.FileName);
			fuImport.PostedFile.SaveAs(UploadedFilePath);

			try
			{
				IImportRecord record = Engine.Resolve<Importer>().Read(UploadedFilePath);
				importedItems.CurrentItem = record.RootItem;
				rptAttachments.DataSource = record.Attachments;
				ShowErrors(record);
			}
			catch(WrongVersionException)
			{
				using (Stream s = File.OpenRead(UploadedFilePath))
				{
					N2XmlReader xr = new N2XmlReader(N2.Context.Current);
					importedItems.CurrentItem = xr.Read(s);
				}
			}
			uploadFlow.ActiveViewIndex = 1;
			uploadFlow.Views[1].DataBind();
		}

		protected void btnUploadImport_Click(object sender, EventArgs e)
		{
			Importer importer = Engine.Resolve<Importer>();

			IImportRecord record;
			try
			{
				record = importer.Read(fuImport.FileContent, fuImport.FileName);
			}
			catch (WrongVersionException)
			{
				N2XmlReader xr = new N2XmlReader(N2.Context.Current);
				ContentItem item = xr.Read(fuImport.FileContent);
				record = CreateRecord(item);
				ShowErrors(record);
			}

			Import(importer, record);
		}

		protected void btnImportUploaded_Click(object sender, EventArgs e)
		{
			Importer importer = Engine.Resolve<Importer>();

			IImportRecord record;
			try
			{
				record = importer.Read(UploadedFilePath);
				ShowErrors(record);
			}
			catch (WrongVersionException)
			{
				N2XmlReader xr = new N2XmlReader(N2.Context.Current);
				ContentItem item = xr.Read(File.OpenRead(UploadedFilePath));
				record = CreateRecord(item);
			}

			Import(importer, record);
		}

		private static IImportRecord CreateRecord(ContentItem item)
		{
			ReadingJournal rj = new ReadingJournal();
			rj.Report(item);
			return rj;
		}

		private void Import(Importer importer, IImportRecord record)
		{
			try
			{
				if (chkSkipRoot.Checked)
				{
                    importer.Import(record, Selection.SelectedItem, ImportOption.Children);
                    Refresh(Selection.SelectedItem, ToolbarArea.Both);
				}
				else
				{
                    importer.Import(record, Selection.SelectedItem, ImportOption.All);
					Refresh(record.RootItem, ToolbarArea.Both);
				}

				ShowErrors(record);
			}
			catch(N2Exception ex)
			{
				cvImport.ErrorMessage = ex.Message;
				cvImport.IsValid = false;
				btnImportUploaded.Enabled = false;
			}
			finally
			{
				if (File.Exists(UploadedFilePath))
					File.Delete(UploadedFilePath);
			}
		}

		void ShowErrors(IImportRecord record)
		{
			if(record.Errors.Count > 0)
			{
				StringBuilder errorText = new StringBuilder("<ul>");
				foreach(Exception ex in record.Errors)
				{
					errorText.Append("<li>").Append(ex.Message).Append("</li>");
				}
				errorText.Append("</ul>");
					
				cvImport.IsValid = false;
				cvImport.ErrorMessage = errorText.ToString();
			}
		}

		protected void btnImport_Click(object sender, EventArgs e)
		{
			uploadFlow.ActiveViewIndex = 0;
		}

		#endregion

		protected string CheckExists(string url)
		{
			if (Engine.Resolve<IFileSystem>().FileExists(url))
				return "(existing file will be overwritten)";
			return string.Empty;
		}
	}
}