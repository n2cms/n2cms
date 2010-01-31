using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Edit
{
	[Service]
	public class VirtualNodeFactory
	{
		Dictionary<string, NodeFactoryInfo> pathToFactoryMap = new Dictionary<string, NodeFactoryInfo>(StringComparer.InvariantCultureIgnoreCase);
		static string EmptyPath = string.Empty;

		public virtual ContentItem FindNode(string path)
		{
			var map = pathToFactoryMap;

			if (map.ContainsKey(path))
				return map[path].FactoryMethod(EmptyPath);

			foreach (var info in map.Values)
			{
				if (path.StartsWith(info.Path, StringComparison.InvariantCultureIgnoreCase))
					return info.FactoryMethod(path.Substring(info.Path.Length));
			}

			return null;
		}

		public virtual IEnumerable<ContentItem> FindChildren(string path)
		{
			foreach (var info in pathToFactoryMap.Values)
			{
				if (string.Equals(info.ParentPath, path, StringComparison.InvariantCultureIgnoreCase))
					yield return info.FactoryMethod(EmptyPath);
			}
		}

		public virtual void Register(string path, NodeFactoryDelegate factoryMethod)
		{
			var next = new Dictionary<string, NodeFactoryInfo>(pathToFactoryMap, StringComparer.InvariantCultureIgnoreCase);
			next[path] = new NodeFactoryInfo(path, factoryMethod);
			pathToFactoryMap = next;
		}

		public virtual void Unregister(string path)
		{
			var next = new Dictionary<string, NodeFactoryInfo>(pathToFactoryMap, StringComparer.InvariantCultureIgnoreCase);

			if (next.ContainsKey(path))
			{
				next.Remove(path);
				pathToFactoryMap = next;
			}
		}

		class NodeFactoryInfo
		{
			public NodeFactoryInfo(string path, NodeFactoryDelegate factoryMethod)
			{
				Path = path;
				ParentPath = N2.Web.Url.RemoveLastSegment(Path);
				FactoryMethod = factoryMethod;
			}

			public NodeFactoryDelegate FactoryMethod { get; set; }
			public string Path { get; set; }
			public string ParentPath { get; set; }
		}
	}
}
