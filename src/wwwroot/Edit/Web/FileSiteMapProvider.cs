using System;
using System.Web;
using System.Collections.Specialized;
using System.Web.Hosting;
using N2.Edit.FileSystem;
using N2.Web;
using System.Collections.Generic;

namespace N2.Edit.Web
{
	public class FileSiteMapProvider : SiteMapProvider
	{
		protected IFileSystem FileSystem
		{
			get { return Context.Current.Resolve<IFileSystem>(); }
		}

		private FileSiteMapNode NewNode(string url)
		{
			if (url == "~/")
				return new RootNode(this, url);
			if (FileSystem.FileExists(url))
				return new FileNode(this, url);
			return new DirectoryNode(this, url);

		}

		internal static string GetPathOnDisk(string rawUrl)
		{
			return HostingEnvironment.MapPath(rawUrl.Split('?')[0]);
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
				foreach(FileData file in FileSystem.GetFiles(node.Url))
					folderPaths.Add(file.VirtualPath);
				foreach(DirectoryData dir in FileSystem.GetDirectories(node.Url))
					folderPaths.Add(dir.VirtualPath);
			}

			SiteMapNodeCollection nodes = new SiteMapNodeCollection();
			foreach (string folderPath in folderPaths)
				nodes.Add(NewNode(folderPath));
			return nodes;
		}

		public override SiteMapNode GetParentNode(SiteMapNode node)
		{
			return NewNode(Url.Parse(node.Url).RemoveTrailingSegment(false).Path);
		}

		protected override SiteMapNode GetRootNodeCore()
		{
			return new RootNode(this, "~/");
		}
	}
}
