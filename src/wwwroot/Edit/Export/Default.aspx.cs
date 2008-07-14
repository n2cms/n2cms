using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Serialization;
using N2.Xml;
using N2.Web.UI;

namespace N2.Edit.Export
{
	[ToolbarPlugin("", "exportimport", "~/Edit/Export/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/package_come_and_go.gif", 150, ToolTip = "export/import page data", AuthorizedRoles = new string[] { "Administrators", "Admin" }, GlobalResourceClassName = "Toolbar")]
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

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
            hlCancel.NavigateUrl = CancelUrl();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			exportedItems.CurrentItem = SelectedItem;
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

			Engine.Resolve<Exporter>().Export(SelectedItem, options, Response);
		}

		protected void btnVerify_Click(object sender, EventArgs e)
		{
			UploadedFilePath = Path.GetTempFileName() + Path.GetExtension(fuImport.PostedFile.FileName);
			fuImport.PostedFile.SaveAs(UploadedFilePath);

			try
			{
				IImportRecord record = Engine.Resolve<Importer>().Read(UploadedFilePath);
				importedItems.CurrentItem = record.RootItem;
				rptAttachments.DataSource = record.Attachments;
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
					importer.Import(record, SelectedItem, ImportOption.Children);
					Refresh(SelectedItem, ToolbarArea.Both);
				}
				else
				{
					importer.Import(record, SelectedItem, ImportOption.All);
					Refresh(record.RootItem, ToolbarArea.Both);
				}
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

		protected void btnImport_Click(object sender, EventArgs e)
		{
			uploadFlow.ActiveViewIndex = 0;
		}

		#endregion

		protected string CheckExists(string url)
		{
			if (File.Exists(Server.MapPath(url)))
				return "(existing file will be overwritten)";
			return string.Empty;
		}
	}
}