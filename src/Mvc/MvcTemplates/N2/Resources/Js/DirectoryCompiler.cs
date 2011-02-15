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
			this.Context = context;

			if (CacheUtility.IsModifiedSince(context.Request, GetFiles(context).Select(vf => context.Server.MapPath(vf.VirtualPath))))
			{
				CacheUtility.NotModified(context.Response);
			}

			context.Response.ContentType = this.ContentType;
			SetCache(context);
			var response = context.Response;

			bool debug = Resources.Register.Debug;
			if (!debug)
			{
				if (context.Request.Headers["Accept-Encoding"].Contains("gzip"))
				{
					response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
					response.AppendHeader("Content-Encoding", "gzip");
				}
				else if (context.Request.Headers["Accept-Encoding"].Contains("deflate"))
				{
					response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
					response.AppendHeader("Content-Encoding", "deflate");
				}
			}

			foreach (var file in GetFiles(context))
			{
				if (debug)
				{
					response.Write(Environment.NewLine
						+ Environment.NewLine
						+ "/*** " + file.Name + " ***/"
						+ Environment.NewLine);
				}
				response.Write(Environment.NewLine);

				foreach (var line in ReadLines(file))
				{
					if (debug)
						response.Write(line);
					else
					{
						string trimmed = line.Trim(' ', '\t');
						if (trimmed.Length > 0 && !trimmed.StartsWith("//"))
						{
							response.Write(trimmed);
							response.Write(Environment.NewLine);
						}
					}
				}
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
					if(FileMasks.Any(m => Regex.IsMatch(file.Name, m.Replace(".", "[.]").Replace("*", ".*") + "$")))
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
		
		protected virtual string ContentType { get { return "text/javascript"; } }
		
		protected HttpContext Context { get; private set; }
	}
}
