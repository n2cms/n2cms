using N2.Configuration;
using N2.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace N2.Persistence.Xml
{
	[Service(Configuration = "xml")]
	public class XmlFileSystem
	{
		Logger<XmlFileSystem> logger;

		public string RootDirectoryPath { get; set; }

		public XmlFileSystem(ConfigurationManagerWrapper config)
		{
			var virtualPath = "~/App_Data/XmlRepository/";
			try
			{
				var connectionString = config.GetConnectionString();
				virtualPath = connectionString.Split(';')
					.Where(x => x.StartsWith("XmlRepositoryPath="))
					.Select(x => x.Split('=')).Where(x => x.Length > 1).Select(x => x[1])
					.FirstOrDefault()
					?? virtualPath;
			}
			catch (ConfigurationErrorsException ex)
			{
				logger.Warn(ex);
			}

			RootDirectoryPath = HostingEnvironment.MapPath("~/" + virtualPath.Trim('~', '/') + "/")
				?? Environment.CurrentDirectory + "\\" + virtualPath.Trim('~', '/').Replace('/', '\\') + "\\";
		}

		public string GetDirectory<TEntity>()
		{
			string path = RootDirectoryPath + typeof(TEntity).Name;

			if (!Directory.Exists(path))
			{
				logger.InfoFormat("Creating {0}", path);
				Directory.CreateDirectory(path);
			}

			return path;
		}

		public virtual IEnumerable<string> GetFiles<TEntity>()
		{
			return Directory.GetFiles(GetDirectory<TEntity>(), "*.xml");
		}

		public virtual string GetPath<TEntity>(object id)
		{
			return Path.Combine(GetDirectory<TEntity>(), id + ".xml");
		}

		public virtual bool Exists<TEntity>(object id)
		{
			var path = GetPath<TEntity>(id);
			return File.Exists(path);
		}

		public virtual string Read<TEntity>(object id)
		{
			string path = GetPath<TEntity>(id);
			if (!File.Exists(path))
				return null;
			var xml = File.ReadAllText(path);
			return xml;
		}

		public virtual void Delete<TEntity>(object id)
		{
			var path = GetPath<TEntity>(id);
			if (File.Exists(path))
				File.Delete(path);
		}

		public virtual void Write<TEntity>(object id, string xml)
		{
			var path = GetPath<TEntity>(id);
			File.WriteAllText(path, xml);
		}

		public virtual void EnsureRootDirectory()
		{
			if (!Directory.Exists(RootDirectoryPath))
			{
				logger.InfoFormat("Creating {0}", RootDirectoryPath);
				Directory.CreateDirectory(RootDirectoryPath);
			}
		}

		public virtual IEnumerable<string> GetEntityNames()
		{
			return Directory.GetDirectories(RootDirectoryPath).Select(d => d.Substring(RootDirectoryPath.Length));
		}

		public virtual void DeleteEntityDirectories()
		{
			foreach (var subdir in Directory.GetDirectories(RootDirectoryPath))
			{
				logger.InfoFormat("Deleting {0}", subdir);
				Directory.Delete(subdir, recursive: true);
			}
		}

		internal int GetMaxId<TEntity>()
		{
			return GetFiles<TEntity>()
				.Select(path => (int)ExtractId(path))
				.DefaultIfEmpty()
				.Max();
		}

		public static object ExtractId(string path)
		{
			return int.Parse(System.IO.Path.GetFileNameWithoutExtension(path));
		}

		internal int Count<TEntity>()
		{
			return GetFiles<TEntity>().Count();
		}

		public override string ToString()
		{
			try
			{
				return string.Format("FileSystem: Path = \"{0}\"", this.RootDirectoryPath);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return ex.Message;
			}
		}
	}
}
