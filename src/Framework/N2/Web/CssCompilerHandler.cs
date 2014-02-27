using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Text.RegularExpressions;

namespace N2.Web
{
    public class CssCompilerHandler : IHttpHandler
    {
		static Regex urlExpression = new Regex("url[(][\"']?(?<url>[^(\"')]+)[\"']?[)]");
        static Regex importUrlExpression = new Regex("@import (url)?[(]?[\"']?(?<url>[^(\"')]+)[\"']?[)]?");

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var vpp = HostingEnvironment.VirtualPathProvider;

            if (!vpp.FileExists(context.Request.AppRelativeCurrentExecutionFilePath))
            {
                context.Response.Status = "404 Not Found";
                return;
            }
            
            if (!Resources.Register.Debug)
                context.TrySetCompressionFilter();
            context.Response.ContentType = "text/css";

            Write(context, vpp, context.Request.AppRelativeCurrentExecutionFilePath);

            if (!Resources.Register.Debug)
            {
                context.Response.Cache.SetLastModifiedFromFileDependencies();
                context.Response.Cache.SetETagFromFileDependencies();
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
            }
        }

        private void Write(HttpContext context, VirtualPathProvider vpp, string cssPath)
        {
            var mappedPath = context.Server.MapPath(cssPath);
            if(File.Exists(mappedPath))
                context.Response.AddFileDependency(mappedPath);

            var file = vpp.GetFile(cssPath);
            using (var s = file.Open())
            using (var tr = new StreamReader(s))
            {
                while (tr.Peek() >= 0)
                {
                    var line = tr.ReadLine();
                    string importPath = GetImportedPath(context, vpp, cssPath, line);

                    if (string.IsNullOrEmpty(importPath))
                        // process all lines except imports
                        context.Response.Write(Process(line, VirtualPathUtility.GetDirectory(cssPath)));
                    else if (vpp.FileExists(importPath))
                        // recurse into imports and output
                        Write(context, vpp, importPath);
                    else
                        // fallback just write the line
                        context.Response.Write(line);

                    context.Response.Write(Environment.NewLine);
            
                }
            }
        }

        private static string Process(string line, string cssDirectory)
        {
            if(!line.Contains("url("))
                return Trim(line);
            return Trim(RebaseUrls(line, cssDirectory));
        }

        private static string RebaseUrls(string line, string cssDirectory)
        {
            return urlExpression.Replace(line, me =>
                {
                    return "url(\"" + RebaseUrl(me.Groups["url"].Value, cssDirectory) + "\")";
                });
        }

        private static string RebaseUrl(string url, string cssDirectory)
        {
            if (url.StartsWith("/"))
                return url;
            if (url.StartsWith("~"))
                return Url.ToAbsolute(url);

            return Normalize(url, cssDirectory);
        }

        private static string Trim(string line)
        {
            return line.Trim(' ', '\t');
        }

        private string GetImportedPath(HttpContext context, VirtualPathProvider vpp, string path, string line)
        {
            if (Resources.Register.Debug)
                return null;
            if (!line.StartsWith("@import"))
                return null;
            
            string url = importUrlExpression.Match(line).Groups["url"].Value;
            if (!string.IsNullOrEmpty(url))
            {
                bool isRelative = !url.StartsWith("~") && !url.StartsWith("/");
                if (!isRelative)
                    return null;

                url = Normalize(url, VirtualPathUtility.GetDirectory(path));
                if (vpp.FileExists(url))
                    return url;
            }
            return null;
        }

        private static string Normalize(string url, Url dir)
        {
            while (url.StartsWith("../"))
            {
                url = url.Substring(3);
                dir = dir.RemoveTrailingSegment();
            }
            return Url.Combine(dir, url);
        }

        #endregion
    }
}
