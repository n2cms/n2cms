#if DEMO
using System;
using N2.Edit.FileSystem;
using System.Web;

namespace N2.Templates.Mvc.Areas.Tests.Demo
{
	//<components>
	//    <add service="N2.Edit.FileSystem.IFileSystem, N2" implementation="N2.Templates.Mvc.Areas.Tests.Demo.DemoFileSystem, N2.Templates.Mvc" />
	//</components>
	public class DemoFileSystem : VirtualPathFileSystem
	{
		public override void WriteFile(string virtualPath, System.IO.Stream inputStream)
		{
			if (!System.Text.RegularExpressions.Regex.IsMatch(virtualPath, "\\.gif$|\\.jpg$|\\.jpeg$|\\.png$|\\.txt$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
				throw new Exception("Invalid file extension for demo site");

			base.WriteFile(virtualPath, inputStream);
		}

		public override void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
		{
			if(!VirtualPathUtility.ToAbsolute(destinationVirtualPath).StartsWith("/upload", StringComparison.InvariantCultureIgnoreCase))
				throw new Exception("Cannot move to outside of the upload folder");

			base.MoveDirectory(fromVirtualPath, destinationVirtualPath);
		}

		public override void MoveFile(string fromVirtualPath, string destinationVirtualPath)
		{
			if (!VirtualPathUtility.ToAbsolute(destinationVirtualPath).StartsWith("/upload", StringComparison.InvariantCultureIgnoreCase))
				throw new Exception("Cannot move to outside of the upload folder");

			base.MoveFile(fromVirtualPath, destinationVirtualPath);
		}

		public override System.IO.Stream OpenFile(string virtualPath, bool openRead = false)
		{
			if (!VirtualPathUtility.ToAbsolute(virtualPath).StartsWith("/upload", StringComparison.InvariantCultureIgnoreCase))
				throw new Exception("Cannot open outside of the upload folder");

			return base.OpenFile(virtualPath, openRead);
		}

		public override void CopyFile(string fromVirtualPath, string destinationVirtualPath)
		{
			if (!VirtualPathUtility.ToAbsolute(destinationVirtualPath).StartsWith("/upload", StringComparison.InvariantCultureIgnoreCase))
				throw new Exception("Cannot copy to outside of the upload folder");

			base.CopyFile(fromVirtualPath, destinationVirtualPath);
		}

		public override void CreateDirectory(string virtualPath)
		{
			if (!VirtualPathUtility.ToAbsolute(virtualPath).StartsWith("/upload", StringComparison.InvariantCultureIgnoreCase))
				throw new Exception("Cannot create outside of the upload folder");

			base.CreateDirectory(virtualPath);
		}
	}
}
#endif