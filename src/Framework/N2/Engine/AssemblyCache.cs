using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using N2.Persistence;
using System.Reflection;

namespace N2.Engine
{
	public class AssemblyCache
	{
		Logger<AssemblyCache> logger;
		private Web.IWebContext webContext;
		private TemporaryFileHelper temp;
		Assembly[] cache;

		public AssemblyCache(Web.IWebContext webContext, TemporaryFileHelper temp)
		{
			this.webContext = webContext;
			this.temp = temp;
		}

		public IEnumerable<Assembly> GetAssemblies(Func<IEnumerable<Assembly>> factory)
		{
			if (cache != null)
				return cache;

			var settings = GetSettings();
			if (settings == null || !settings.ContainsKey("timestamp") || !settings.ContainsKey("assemblies"))
				return ReReadAssemblies(factory);

			DateTime timestamp;
			if (!DateTime.TryParse(settings["timestamp"], out timestamp))
				return ReReadAssemblies(factory);

			if (timestamp > GetTimeStamp())
				return ReReadAssemblies(factory);

			string assemblies = settings["assemblies"];
			logger.DebugFormat("Reading assemblies from cache with timestamp {0}, assemblies: {1}", timestamp, assemblies);

			return cache = assemblies.Split(';').Select(name => AppDomain.CurrentDomain.Load(name)).ToArray();
		}

		private IEnumerable<Assembly> ReReadAssemblies(Func<IEnumerable<Assembly>> factory)
		{
			var assemblies = factory();
			SaveSettings(assemblies);
			return cache = assemblies.ToArray();
		}

		private void SaveSettings(IEnumerable<Assembly> assemblies)
		{
			var path = GetAssemblyCachePath();
			var timestamp = GetTimeStamp();
			logger.InfoFormat("Caching {0} assemblies with timestamp {1} to path '{2}'", assemblies.Count(), timestamp, path);
			
			File.WriteAllLines(path, new[] {
				"timestamp=" + timestamp,
				"assemblies=" + string.Join(";", assemblies.Select(a => a.FullName).ToArray())
			});
		}

		private IDictionary<string, string> GetSettings()
		{
			var path = GetAssemblyCachePath();
			if (File.Exists(path))
			{
				return File.ReadAllLines(path)
					.Select(line => new { Index = line.IndexOf('='), Line = line })
					.Where(line => line.Index >= 0)
					.ToDictionary(line => line.Line.Substring(0, line.Index), line => line.Line.Substring(line.Index + 1));
			}
			return null;
		}

		private string GetAssemblyCachePath()
		{
			return Path.Combine(temp.GetSharedTemporaryDirectory(), "AssemblyCache.config");
		}

		public DateTime GetTimeStamp()
		{
			var max = DateTime.MinValue;
			foreach (var fi in GetAssemblyFiles())
			{
				if (fi.LastWriteTimeUtc > max)
					max = fi.LastWriteTimeUtc;
			}
			return max;
		}

		private IEnumerable<FileInfo> GetAssemblyFiles()
		{
			return GetProbingPaths().SelectMany(pp => new DirectoryInfo(pp).GetFiles());
		}

		public virtual IEnumerable<string> GetProbingPaths()
		{
			return new [] { webContext.MapPath("~/bin") };
		}
	}
}
