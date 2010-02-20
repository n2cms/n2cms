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
		INodeProvider[] providers = new INodeProvider[0];
		static string EmptyPath = string.Empty;

		public virtual ContentItem Find(string path)
		{
			foreach (var provider in providers)
			{
				ContentItem item = provider.Get(path);
				if (item != null)
					return item;
			}

			return null;
		}

		public virtual IEnumerable<ContentItem> FindChildren(string path)
		{
			foreach (var provider in providers)
			{
				foreach (var item in provider.GetChildren(path))
					yield return item;
			}
		}

		public virtual void Register(INodeProvider provider)
		{
			List<INodeProvider> next = new List<INodeProvider>(providers);
			next.Add(provider);
			providers = next.ToArray();
		}

		public virtual void Unregister(string path)
		{
			List<INodeProvider> next = new List<INodeProvider>(providers);
			next.RemoveAll(p => p.Get(path) != null);
			providers = next.ToArray();
		}
	}
}
