using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using N2.Edit.FileManagement;

namespace N2.Addons.UITests.UI
{
	public partial class FileEvents : System.Web.UI.Page
	{
		static bool throwMode = false;
		static bool logMode = false;
		static StringBuilder log = new StringBuilder();

		static FileEvents()
		{
			FileManager.FileDeleted += FileManager_FileDeleted;
			FileManager.FileDeleting += FileManager_FileDeleting;
			FileManager.FileUploaded += FileManager_FileUploaded;
			FileManager.FileUploading += FileManager_FileUploading;
		}

		static void FileManager_FileUploading(object sender, FileEventArgs e)
		{
			if(logMode)
				log.Append("Uploading ").AppendLine(e.FileName);
			if(throwMode)
				throw new ApplicationException("Cannot upload " + e.FileName);
		}

		static void FileManager_FileUploaded(object sender, FileEventArgs e)
		{
			if (logMode)
				log.Append("Uploaded ").AppendLine(e.FileName);
			if (throwMode)
				throw new ApplicationException("Error after uploading " + e.FileName);
		}

		static void FileManager_FileDeleting(object sender, FileEventArgs e)
		{
			if (logMode)
				log.Append("Deleting ").AppendLine(e.FileName);
			if (throwMode)
				throw new ApplicationException("Cannot delete " + e.FileName);
		}

		static void FileManager_FileDeleted(object sender, FileEventArgs e)
		{
			if (logMode)
				log.Append("Deleted ").AppendLine(e.FileName);
			if (throwMode)
				throw new ApplicationException("Error after deleting " + e.FileName);
		}
		
		protected override void OnInit(EventArgs e)
		{
			CheckBox2.Checked = throwMode;
			CheckBox1.Checked = logMode;
			TextBox1.Text = log.ToString();
			base.OnInit(e);
		}

		protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
		{
			throwMode = CheckBox2.Checked;
		}

		protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
		{
			logMode = CheckBox1.Checked;
			if (!logMode)
				log.Length = 0;
		}
	}
}
