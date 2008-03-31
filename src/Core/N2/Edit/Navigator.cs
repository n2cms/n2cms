using System;
using System.Collections.Generic;
using System.Text;
using N2.Persistence;
using N2.Web;

namespace N2.Edit
{
	public class Navigator
	{
		IPersister persister;
		Site site;

		public Navigator(IPersister persister, Site site)
		{
			this.persister = persister;
			this.site = site;
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
					return Navigate(persister.Get(site.StartPageID), path.Substring(1));
				}
				throw new ArgumentException("The path must start with a slash '/', was '" + path + "'", "path");
			}

			return Navigate(persister.Get(site.RootItemID), path);
		}
	}
}
