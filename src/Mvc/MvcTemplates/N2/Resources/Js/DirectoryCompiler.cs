using System;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Collections.Generic;
using N2.Web;

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
			this.Context = context;

			if (CacheUtility.IsModifiedSince(context.Request, GetFiles(context)))
			{
				CacheUtility.NotModified(context.Response);
			}

			context.Response.ContentType = this.ContentType;
			context.Response.Buffer = false;
			SetCache(context);

			foreach (string file in GetFiles(context))
			{
#if DEBUG
				context.Response.Write(Environment.NewLine
					+ Environment.NewLine
					+ "/*** " + Path.GetFileName(file) + " ***/"
					+ Environment.NewLine
					+ Environment.NewLine);
#else
				context.Response.Write(Environment.NewLine + Environment.NewLine);
#endif

				context.Response.Write(this.ReadFileContent(file));
			}
		}

		protected virtual IEnumerable<string> GetFiles(HttpContext context)
		{
			string dir = context.Server.MapPath(FolderUrl);
			List<string> _files = new List<string>();
			
			foreach(string _mask in this.FileMasks) {
				_files.AddRange(Directory.GetFiles(dir, _mask));
			}
			
			return _files.AsReadOnly();
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
		
		protected virtual string ContentType { get { return "text/javascript"; } }
		
		protected HttpContext Context { get; private set; }
	}
}
