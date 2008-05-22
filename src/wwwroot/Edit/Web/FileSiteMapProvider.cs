using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Collections.Specialized;

namespace N2.Edit.Web
{
    public class FileSiteMapProvider : System.Web.SiteMapProvider
    {
        private FileSiteMapNode NewNode(string url)
        {
            return new FileSiteMapNode(this, url);
        }

        internal static string GetPathOnDisk(string rawUrl)
        {
            return HttpContext.Current.Server.MapPath(rawUrl.Split('?')[0]);
        }

        private static string UnMapPath(string physicalPath)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/");
            return physicalPath.Replace(rootPath, "~/").Replace('\\', '/');
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
            string path = GetPathOnDisk(node.Url);
            SiteMapNodeCollection nodes = new SiteMapNodeCollection();
            if (Directory.Exists(path))
            {
                foreach (string dir in Directory.GetDirectories(path))
                    nodes.Add(NewNode(UnMapPath(dir)));
                foreach (string file in Directory.GetFiles(path))
                    nodes.Add(NewNode(UnMapPath(file)));
            }
            return nodes;
        }

        public override SiteMapNode GetParentNode(SiteMapNode node)
        {
            string path = GetPathOnDisk(node.Url);
            return NewNode(UnMapPath(Directory.GetParent(path).FullName));
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return NewNode(N2.Context.Current.EditManager.GetUploadFolderUrl());
        }
    }
}
