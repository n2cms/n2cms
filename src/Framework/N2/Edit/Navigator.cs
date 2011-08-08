using System;
using N2.Engine;
using N2.Persistence;
using N2.Web;

namespace N2.Edit
{
	[Service]
	public class Navigator
	{
		readonly IPersister persister;
		readonly IHost host;
		readonly VirtualNodeFactory virtualNodes;
		
		public Navigator(IPersister persister, IHost host, VirtualNodeFactory nodes)
		{
			this.persister = persister;
			this.host = host;
			this.virtualNodes = nodes;
		}

		public ContentItem Navigate(ContentItem startingPoint, string path)
		{
			return startingPoint.GetChild(path) 
				?? virtualNodes.Get(startingPoint.Path + path.TrimStart('/'))
				?? virtualNodes.Get(path);
		}

		public ContentItem Navigate(string path)
		{
			if (path == null) 
				return null;

			if (!path.StartsWith("/"))
			{
				if (path.StartsWith("~"))
				{
					return Navigate(persister.Get(host.CurrentSite.StartPageID), path.Substring(1))
						?? virtualNodes.Get(path);
				}
				throw new ArgumentException("The path must start with a slash '/', was '" + path + "'", "path");
			}

			return Navigate(persister.Get(host.CurrentSite.RootItemID), path);
		}
	}
}
