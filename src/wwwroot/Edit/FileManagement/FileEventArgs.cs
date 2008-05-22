using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit.FileManagement
{
	[Obsolete("Sorry, this is a temporary solution.")]
	public class FileEventArgs : EventArgs
	{
		public FileEventArgs(string fileName)
		{
			this.fileName = fileName;
		}

		private string fileName;
		private bool cancel;

		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		public bool Cancel
		{
			get { return cancel; }
			set { cancel = value; }
		}

	}
}
