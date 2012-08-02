using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using N2.Persistence;
using System.Reflection;

namespace N2.Engine
{
	/// <summary>
	/// Caches information about assemblies to increase startup performance.
	/// </summary>
	public class AssemblyCache
	{
		Logger<AssemblyCache> logger;
		private Web.IWebContext webContext;
		private BasicTemporaryFileHelper temp;
		Assembly[] cache;

		public AssemblyCache(Web.IWebContext webContext, BasicTemporaryFileHelper temp)
		{
			this.webContext = webContext;
			this.temp = temp;
		}

		public IEnumerable<Type> GetTypes(string key, Func<IEnumerable<Type>> factory)
		{
			var settings = GetSettings();
			if (!settings.ContainsKey("timestamp"))
				return ReReadTypes(key, factory, "no timestamp");

			var timestamp = settings["timestamp"].FirstOrDefault();
			if (string.IsNullOrEmpty(timestamp) || IsModifiedAfter(timestamp))
				return ReReadTypes(key, factory, "changes after " + timestamp);

			if (!settings.ContainsKey(key))
				return ReReadTypes(key, factory, "no cached types");

			var types = settings[key]
				.Select(name => Type.GetType(name))
				.Where(t => t != null)
				.ToList();

			logger.DebugFormat("Reading {0} types for key {1} from cached settings with timestamp {2}", types.Count, key, timestamp);
			return types;
		}

		private IEnumerable<Type> ReReadTypes(string key, Func<IEnumerable<Type>> factory, string reason)
		{
			var types = factory().ToList();
			logger.InfoFormat("Reading {0} types for key {1}, reason: {2}", types.Count, key, reason);

			SaveSettings(key, types);
			return types;
		}

		private void SaveSettings(string key, IEnumerable<Type> types)
		{
			lock (this)
			{
				var settings = GetSettings();
				using (var sw = OverwriteAssemblyCache())
				{
					foreach (var existingKey in settings.Keys.Where(k => k != key))
						foreach (var setting in settings[existingKey])
							sw.WriteLine(existingKey + "=" + setting);

					foreach (var type in types)
					{
						sw.WriteLine(key + "=" + type.AssemblyQualifiedName);
					}
					fileCache = null;
					sw.Flush();
					sw.Close();
				}
			}
		}

		/// <summary>
		/// Gets assemblies from cached location. If files in the probing paths have changed assemblies are re-read.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IEnumerable<Assembly> GetAssemblies(Func<IEnumerable<Assembly>> factory)
		{
			if (cache != null)
				return cache;

			var settings = GetSettings();
			if (!settings.ContainsKey("timestamp"))
				return ReReadAssemblies(factory, "no timestamp");

			var timestamp = settings["timestamp"].FirstOrDefault();
			if (string.IsNullOrEmpty(timestamp) || IsModifiedAfter(timestamp))
				return ReReadAssemblies(factory, "changes after " + timestamp);

			if (!settings.ContainsKey("assembly"))
				return ReReadAssemblies(factory, "no cached assemblies");

			var assemblies = settings["assembly"].ToArray();
			logger.DebugFormat("Reading assemblies from cache with timestamp {0}, assemblies: {1}", timestamp, string.Join("; ", assemblies));

			try
			{
				return cache = assemblies.Select(name => AppDomain.CurrentDomain.Load(name)).ToArray();
			}
			catch (Exception ex)
			{
				logger.Warn(ex);
				return ReReadAssemblies(factory, ex.Message);
			}
		}

		private IEnumerable<Assembly> ReReadAssemblies(Func<IEnumerable<Assembly>> factory, string reason)
		{
			var assemblies = factory();
			logger.DebugFormat("Re-reading assemblies due to {0}", reason);
			SaveSettings(assemblies);
			return cache = assemblies.ToArray();
		}

		private void SaveSettings(IEnumerable<Assembly> assemblies)
		{
			var timestamp = GetMostRecentModifiedDate();
			logger.InfoFormat("Caching {0} assemblies with timestamp {1}", assemblies.Count(), timestamp);

			lock (this)
			{
				using (var sw = OverwriteAssemblyCache())
				{
					sw.WriteLine("timestamp=" + timestamp);
					foreach(var assembly in assemblies)
						sw.WriteLine("assembly=" + assembly.FullName);
					fileCache = null;
					sw.Flush();
					sw.Close();
				}
			}
		}

		IDictionary<string, IEnumerable<string>> fileCache;
		private IDictionary<string, IEnumerable<string>> GetSettings()
		{
			if (fileCache != null)
				return fileCache;
			
			var path = GetAssemblyCachePath();
			if (File.Exists(path))
			{
				lock (this)
				{
					var lines = File.ReadAllLines(path)
						.Select(line => new { Index = line.IndexOf('='), Line = line })
						.Where(line => line.Index >= 0);
					var settings = lines
						.Select(line => new { Key = line.Line.Substring(0, line.Index), Value = line.Line.Substring(line.Index + 1) })
						.GroupBy(line => line.Key, line => line.Value);
					fileCache = settings.ToDictionary(group => group.Key, group => (IEnumerable<string>)group.ToList());
				}
			}
			else
				fileCache = new Dictionary<string, IEnumerable<string>>();

			return fileCache;
		}

		private string GetAssemblyCachePath()
		{
			return Path.Combine(temp.GetSharedTemporaryDirectory(), "AssemblyCache.config");
		}

		private bool IsModifiedAfter(string timestampString)
		{
			DateTime timestamp;
			if (!DateTime.TryParse(timestampString, out timestamp))
				return true;

			var mostRecentAssembly = GetMostRecentModifiedDate();
			if (timestamp < mostRecentAssembly)
				return true;

			return false;
		}

		public DateTime GetMostRecentModifiedDate()
		{
			var max = GetAssemblyFiles().Max(fi => fi.LastWriteTimeUtc);
			return new DateTime(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second);
		}

		private IEnumerable<FileInfo> GetAssemblyFiles()
		{
			return GetProbingPaths().SelectMany(pp => new DirectoryInfo(pp).GetFiles("*.dll"));
		}

		private StreamWriter AppendAssemblyCache()
		{
			var path = GetAssemblyCachePath();
			return File.AppendText(path);
		}

		private StreamWriter OverwriteAssemblyCache()
		{
			var path = GetAssemblyCachePath();
			if (File.Exists(path))
				File.Delete(path);
			return File.CreateText(path);
		}

		public virtual IEnumerable<string> GetProbingPaths()
		{
			return new [] { webContext.MapPath("~/bin") };
		}
	}
}
