using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Edit
{
	[Service]
	public class VirtualNodeFactory : INodeProvider
	{
		INodeProvider[] providers = new INodeProvider[0];
		static string EmptyPath = string.Empty;

		public virtual ContentItem Get(string path)
		{
			foreach (var provider in providers)
			{
				ContentItem item = provider.Get(path);
				if (item != null)
					return item;
			}

			return null;
		}

		public virtual IEnumerable<ContentItem> GetChildren(string path)
		{
			foreach (var provider in providers)
			{
				foreach (var item in provider.GetChildren(path))
					yield return item;
			}
		}

		public virtual void Register(INodeProvider provider)
		{
			List<INodeProvider> temp = new List<INodeProvider>(providers);
			temp.Add(provider);
			providers = temp.ToArray();
		}

		public virtual void Unregister(INodeProvider provider)
		{
			List<INodeProvider> temp = new List<INodeProvider>(providers);
			temp.RemoveAll(p => p == provider);
			providers = temp.ToArray();
		}
	}
}
