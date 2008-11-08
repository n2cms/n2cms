using System;
using System.Collections.Generic;
using System.Text;
using N2.Collections;
using N2.Persistence;
using N2.Web;
using N2.Web.UI;

namespace N2.Edit
{
	public class Navigator
	{
		private readonly IPersister persister;
		private readonly IHost host;

		public Navigator(IPersister persister, IHost host)
		{
			this.persister = persister;
			this.host = host;
		}

		public ContentItem Navigate(ContentItem startingPoint, string path)
		{
			return startingPoint.GetChild(path);
		}	

		public ContentItem Navigate(string path)
		{
			if (path == null) throw new ArgumentNullException("path");
			if (!path.StartsWith("/"))
			{
				if (path.StartsWith("~"))
				{
					return Navigate(persister.Get(host.CurrentSite.StartPageID), path.Substring(1));
				}
				throw new ArgumentException("The path must start with a slash '/', was '" + path + "'", "path");
			}

			return Navigate(persister.Get(host.CurrentSite.RootItemID), path);
		}
	}
}
