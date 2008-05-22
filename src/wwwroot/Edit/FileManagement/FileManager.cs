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
	public class FileManager
	{
		public static event EventHandler<FileEventArgs> FileUploading;
		public static event EventHandler<FileEventArgs> FileUploaded;
		public static event EventHandler<FileEventArgs> FileDeleting;
		public static event EventHandler<FileEventArgs> FileDeleted;

		internal static bool CancelUploading(string url)
		{
			if (FileManager.FileUploading != null)
			{
				FileEventArgs fea = new FileEventArgs(url);
				FileManager.FileUploading.Invoke(null, fea);
				return fea.Cancel;
			}
			return false;
		}
		internal static bool InvokeUploaded(string url)
		{
			if (FileManager.FileUploaded != null)
			{
				FileEventArgs fea = new FileEventArgs(url);
				FileManager.FileUploaded.Invoke(null, fea);
				return fea.Cancel;
			}
			return false;
		}
		internal static bool CancelDeleting(string url)
		{
			if (FileManager.FileDeleting != null)
			{
				FileEventArgs fea = new FileEventArgs(url);
				FileManager.FileDeleting.Invoke(null, fea);
				return fea.Cancel;
			}
			return false;
		}
		internal static bool InvokeDeleted(string url)
		{
			if (FileManager.FileDeleted != null)
			{
				FileEventArgs fea = new FileEventArgs(url);
				FileManager.FileDeleted.Invoke(null, fea);
				return fea.Cancel;
			}
			return false;
		}
	}
}
