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
		Dictionary<string, Function<string, ContentItem>> pathToFactory = new Dictionary<string, Function<string, ContentItem>>(StringComparer.InvariantCultureIgnoreCase);

		public virtual ContentItem FindNode(string path)
		{
			if (pathToFactory.ContainsKey(path))
				return pathToFactory[path]("");

			foreach (var pair in pathToFactory)
			{
				if (path.StartsWith(pair.Key, StringComparison.InvariantCultureIgnoreCase))
					return pair.Value(path.Substring(pair.Key.Length));
			}

			return null;
		}

		public void Register(string path, Function<string, ContentItem> factoryMethod)
		{
			pathToFactory[path] = factoryMethod;
		}

		public void Unregister(string path)
		{
			if(pathToFactory.ContainsKey(path))
				pathToFactory.Remove(path);
		}
	}
}
