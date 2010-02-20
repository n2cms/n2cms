using System;
using System.Collections.Generic;
using System.Text;
using N2.Collections;
using N2.Persistence;
using N2.Web;
using N2.Web.UI;
using N2.Engine;

namespace N2.Edit
{
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
				?? virtualNodes.Find(path);
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
						?? virtualNodes.Find(path);
				}
				throw new ArgumentException("The path must start with a slash '/', was '" + path + "'", "path");
			}

			return Navigate(persister.Get(host.CurrentSite.RootItemID), path);
		}
	}
}
