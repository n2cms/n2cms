using System;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Web.Hosting;
using N2.Web;
using System.Collections.Generic;

namespace N2.Edit.Web
{
	public class FileSiteMapProvider : System.Web.SiteMapProvider
	{
		private FileSiteMapNode NewNode(string url)
		{
			string path = HostingEnvironment.MapPath(url);
			if (url == "~/")
				return new RootNode(this, url);
			if (File.Exists(path))
				return new FileNode(this, url);
			return new DirectoryNode(this, url);

		}

		internal static string GetPathOnDisk(string rawUrl)
		{
			return HostingEnvironment.MapPath(rawUrl.Split('?')[0]);
		}

		private static string UnMapPath(string physicalPath, string virtualParentUrl)
		{
			string parentPath = HostingEnvironment.MapPath(virtualParentUrl);
			return physicalPath.Replace(parentPath, virtualParentUrl).Replace('\\', '/');
		}

		public override SiteMapNode FindSiteMapNode(string rawUrl)
		{
			FileSiteMapNode fsmn = null;

			try
			{
				string[] pageQueryPair = rawUrl.Split('?');
				if (pageQueryPair.Length > 1)
				{
					NameValueCollection nvc = HttpUtility.ParseQueryString(pageQueryPair[1]);
					if (!string.IsNullOrEmpty(nvc["fileUrl"]))
						fsmn = NewNode(nvc["fileUrl"]);
				}
				else
					fsmn = NewNode(rawUrl);
			}
			catch (Exception ex)
			{
				HttpContext.Current.Trace.Write(ex.ToString());
			}

			return fsmn;
		}

		public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
		{
			List<string> folderPaths = new List<string>();

			if (node.Key == "/" || node is RootNode)
			{
				foreach (string folderUrl in N2.Context.Current.Resolve<IHost>().CurrentSite.UploadFolders)
				{
					if (!folderPaths.Contains(folderUrl))
						folderPaths.Add(folderUrl);
				}
				foreach (string folderUrl in N2.Context.Current.EditManager.UploadFolders)
				{
					if (!folderPaths.Contains(folderUrl))
						folderPaths.Add(folderUrl);
				}
			}
			else
			{
				string path = GetPathOnDisk(node.Url);
				if (Directory.Exists(path))
				{
					foreach (string dir in Directory.GetDirectories(path))
					{
						if ((new FileInfo(dir).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
							continue;

						string folderUrl = UnMapPath(dir, node.Url);
						folderPaths.Add(folderUrl);
					}
					foreach (string file in Directory.GetFiles(path))
					{
						string fileUrl = UnMapPath(file, node.Url);
						folderPaths.Add(fileUrl);
					}
				}
			}

			SiteMapNodeCollection nodes = new SiteMapNodeCollection();
			foreach (string folderPath in folderPaths)
				nodes.Add(NewNode(folderPath));
			return nodes;
		}

		public override SiteMapNode GetParentNode(SiteMapNode node)
		{
			string path = GetPathOnDisk(node.Url);
			return NewNode(UnMapPath(Directory.GetParent(path).FullName, node.Url));
		}

		protected override SiteMapNode GetRootNodeCore()
		{
			return new RootNode(this, "~/");
		}
	}
}
