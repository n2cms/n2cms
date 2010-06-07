using System;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Collections.Generic;

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
			
			string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
			if (!string.IsNullOrEmpty(ifModifiedSince))
			{
				DateTimeOffset since;
				if (DateTimeOffset.TryParse(ifModifiedSince, out since))
				{
					bool wasModifiedSince = false;
					foreach (string file in GetFiles(context))
					{
						if (File.GetLastWriteTimeUtc(file) > since)
						{
							wasModifiedSince = true;
							break;
						}
					}

					if (!wasModifiedSince)
					{
						context.Response.Status = "304 Not Modified";
						context.Response.End();
					}
				}
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
			context.Response.Cache.SetExpires(DateTime.UtcNow.Add(CacheExpiration));
			context.Response.Cache.SetCacheability(HttpCacheability.Public);
			context.Response.Cache.SetLastModified(DateTime.UtcNow);
			context.Response.Cache.SetMaxAge(CacheExpiration);
			context.Response.Cache.SetValidUntilExpires(true);
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
