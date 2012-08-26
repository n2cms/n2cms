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
	public class TypeCache
	{
		#region CachedAssembly
		class CachedAssembly
		{
			Logger<TypeCache> logger;
			public Assembly Assembly { get; set; }
			public Guid ModuleId;
			public Dictionary<string, List<Type>> Queries = new Dictionary<string, List<Type>>();

			public IEnumerable<Type> GetOrCreateCache(string key, Func<Assembly, IEnumerable<Type>> factory, ref bool factoryInvoked)
			{
				lock (this)
				{
					if (!Queries.ContainsKey(key))
					{
						factoryInvoked = true;
						var types = factory(Assembly).ToList();
						logger.DebugFormat("Recreating cache for key {0}, found {1} types in assembly {2}/{3}", key, types.Count, Assembly, ModuleId);
						Queries[key] = types;
						return types;
					}
					else
						return Queries[key];
				}
			}
		}
		#endregion

		Logger<TypeCache> logger;
		private BasicTemporaryFileHelper temp;
		private List<CachedAssembly> cache;

		public TypeCache(BasicTemporaryFileHelper temp)
		{
			this.temp = temp;
		}

		public IEnumerable<Type> GetTypes(string key, Func<IEnumerable<Assembly>> assemblyFactory, Func<Assembly, IEnumerable<Type>> factory)
		{
			bool changesMade = false;
			foreach (var cachedAssembly in GetCache(assemblyFactory))
			{
				foreach (var type in cachedAssembly.GetOrCreateCache(key, factory, ref changesMade))
					yield return type;
			}

			if (changesMade)
				SaveCache();
		}

		/// <summary>
		/// Gets assemblies from cached location. If files in the probing paths have changed assemblies are re-read.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public virtual IEnumerable<Assembly> GetAssemblies(Func<IEnumerable<Assembly>> factory)
		{
			return GetCache(factory).Select(a => a.Assembly);
		}

		private IEnumerable<CachedAssembly> GetCache(Func<IEnumerable<Assembly>> factory)
		{
			if (cache != null)
				return cache;

			var path = GetAssemblyCachePath();

			var assemblies = factory().Select(a => new CachedAssembly { Assembly = a, ModuleId = a.ManifestModule.ModuleVersionId }).ToList();
			if (File.Exists(path))
			{
				lock (this)
				{
					if (cache != null)
						return cache;

					var lines = File.ReadAllLines(path)
						.Select(line => new { Index = line.IndexOf('='), Line = line })
						.Where(line => line.Index >= 0);
					var settings = lines
						.Select(line => new { Key = line.Line.Substring(0, line.Index), Value = line.Line.Substring(line.Index + 1) })
						.GroupBy(setting => setting.Key)
						.ToDictionary(setting => setting.Key, setting => setting.Select(s => s.Value));

					foreach (var assemblyWithQueries in assemblies)
					{
						if (settings.ContainsKey(assemblyWithQueries.ModuleId.ToString()))
						{
							foreach (var value in settings[assemblyWithQueries.ModuleId.ToString()])
							{
								var semicolonIndex = value.IndexOf(';');
								if (semicolonIndex < 0)
									continue;

								string query = value.Substring(0, semicolonIndex);
								if (!assemblyWithQueries.Queries.ContainsKey(query))
									assemblyWithQueries.Queries[query] = new List<Type>();

								string typeName = value.Substring(semicolonIndex + 1);
								if (typeName == "none")
									continue;

								var type = assemblyWithQueries.Assembly.GetType(typeName);
								if (type == null)
									continue;

								assemblyWithQueries.Queries[query].Add(type);
							}
						}
						logger.DebugFormat("Added {0} queries to assembly {1}", assemblyWithQueries.Queries.Count, assemblyWithQueries.Assembly);
					}
				}
			}
				
			return cache = assemblies;
		}

		//private List<CachedAssembly> CreateCachedAssemblies(Func<IEnumerable<Assembly>> factory, string reason)
		//{
		//    logger.InfoFormat("Retrieving assemblies from factory due to {0}", reason);
		//    var assemblies = factory().Select(a => new CachedAssembly { Assembly = a, ModuleId = a.ManifestModule.ModuleVersionId }).ToList();
		//    logger.DebugFormat("Got {0} assemblies", assemblies.Count);
		//    return assemblies;
		//}

		private void SaveCache()
		{
			logger.InfoFormat("Storing {0} assemblies to disk", cache.Count);
			
			lock (this)
			{
				using (var sw = OverwriteAssemblyCache())
				{
					sw.AppendSetting("timestamp", DateTime.UtcNow.ToString());
					sw.AppendSetting("version", "1");

					foreach (var assembly in cache)
					{
						logger.DebugFormat("Caching assembly {0} with {1} queries", assembly.Assembly, assembly.Queries.Count);
						sw.AppendSetting("assembly", assembly.ModuleId + ";" + assembly.Assembly.FullName);
						foreach (var query in assembly.Queries)
						{
							if (query.Value.Count == 0)
								sw.AppendSetting(assembly.ModuleId.ToString(), query.Key + ";none");

							foreach (var type in query.Value)
								sw.AppendSetting(assembly.ModuleId.ToString(), query.Key + ";" + type.FullName);
						}
					}
					sw.Flush();
					sw.Close();
				}
			}
		}

		private string GetAssemblyCachePath()
		{
			return Path.Combine(temp.GetSharedTemporaryDirectory(), "AssemblyCache.config");
		}

		public void Clear()
		{
			cache = null;
			var path = GetAssemblyCachePath();
			if (File.Exists(path))
				File.Delete(path);
		}

		private StreamWriter OverwriteAssemblyCache()
		{
			var path = GetAssemblyCachePath();
			lock (this)
			{
				if (File.Exists(path))
					File.Delete(path);
				return File.CreateText(path);
			}
		}

		public virtual IEnumerable<string> GetProbingPaths()
		{
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\"))
				return new[] { AppDomain.CurrentDomain.BaseDirectory + "bin\\" };
			else
				return new[] { AppDomain.CurrentDomain.BaseDirectory };
		}
	}

	internal static class TypeCacheExtensions
	{
		public static TextWriter AppendSetting(this TextWriter writer, string key, string value)
		{
			writer.Write(key);
			writer.Write("=");
			writer.WriteLine(value);
			return writer;
		}
	}
}
