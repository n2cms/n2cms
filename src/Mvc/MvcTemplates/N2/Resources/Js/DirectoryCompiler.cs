using System;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Collections.Generic;
using N2.Web;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace N2.Edit.Js
{
    /// <summary>
    /// Base handler that compiles multiple files in a directory into a single file.
    /// </summary>
    public abstract class DirectoryCompiler : IHttpHandler
    {
        public abstract string FolderUrl { get; }

        protected virtual TimeSpan CacheExpiration
        {
            get { return TimeSpan.FromHours(1); }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.Request == null) throw new ArgumentNullException("context.Request");
            if (context.Request.Headers == null) throw new ArgumentNullException("context.Request.Headers");
            if (context.Response == null) throw new ArgumentNullException("context.Response");

            if (context.Server != null && CacheUtility.IsUnmodifiedSince(context.Request, GetFiles(context).Select(vf => context.Server.MapPath(vf.VirtualPath))))
            {
                CacheUtility.NotModified(context.Response);
            }

            context.Response.ContentType = this.ContentType;
            SetCache(context);
            var response = context.Response;

            bool debug = Resources.Register.Debug;
            if (debug)
            {
                foreach (var file in GetFiles(context))
                {
                    response.Write(Environment.NewLine
                        + Environment.NewLine
                        + "/*** " + file.Name + " ***/"
                        + Environment.NewLine);
                    
                    response.Write(Environment.NewLine);

                    foreach (var line in ReadLines(file))
                    {
                        response.Write(line);
                        response.Write(Environment.NewLine);
                    }
                }
            }
            else
            {
                context.TrySetCompressionFilter();

                foreach (var file in GetFiles(context))
                {
                    response.Write(Environment.NewLine);

                    bool commenting = false;
                    foreach (var line in ReadLines(file))
                    {
                        WriteLine(response, line, ref commenting);
                    }
                }
            }
        }

        private static void WriteLine(HttpResponse response, string line, ref bool commenting)
        {
            string trimmed = line.Trim(' ', '\t');
            if (commenting)
            {
                var endCommentIndex = trimmed.IndexOf("*/");
                if (endCommentIndex >= 0)
                {
                    commenting = false;
                    response.Write(trimmed.Substring(endCommentIndex + 2));
                    response.Write(Environment.NewLine);
                }
            }
            else if (trimmed.StartsWith("/*"))
            {
                commenting = true;
            }
            else if (trimmed.Length > 0 && !trimmed.StartsWith("//"))
            {
                response.Write(trimmed);
                response.Write(Environment.NewLine);
            }
        }

        private IEnumerable<string> ReadLines(VirtualFile file)
        {
            using(var s = file.Open())
            using (var sr = new StreamReader(s))
            {
                while(sr.Peek() >= 0)
                {
                    yield return sr.ReadLine();
                }
            }
        }

        protected virtual IEnumerable<VirtualFile> GetFiles(HttpContext context)
        {
            var vpp = HostingEnvironment.VirtualPathProvider;
            if(vpp.DirectoryExists(FolderUrl))
            {
                foreach (VirtualFile file in vpp.GetDirectory(FolderUrl).Files)
                {
                    if (file == null)
                        continue;
                    if(FileMasks.Any(m => Regex.IsMatch(file.Name, m.Replace(".", "[.]").Replace("*", ".*") + "$", RegexOptions.Compiled | RegexOptions.IgnoreCase)))
                        yield return file;
                }
            }
        }
        
        protected virtual void SetCache(HttpContext context)
        {
            CacheUtility.SetValidUntilExpires(context.Response, CacheExpiration);
        }
        
        /// <summary>
        /// Reads a file and allows to process a file content before it will be served to the client or cached
        /// </summary>
        /// <param name="fileName">physical file name</param>
        /// <returns>processed file content, by default the original content is served</returns>
        protected virtual string ReadFileContent(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public bool IsReusable
        {
            get { return true; }
        }
        
        protected virtual string[] FileMasks { get { return new[] { "*.js" }; } }

        protected virtual string ContentType { get { return "application/javascript"; } }
    }
}
