using N2.Collections;
using N2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Edit
{
	[Versionable(AllowVersions.No)]
	[Throwable(AllowInTrash.No)]
	public class ChildGroupContainer : ContentItem, IUrlSource, ISystemNode
	{
		public ChildGroupContainer()
		{
		}

		public ChildGroupContainer(ContentItem parent, string title, string name, Func<IEnumerable<ContentItem>> children = null)
		{
			ID = -1;
			Title = title;
			Name = name;
			Parent = parent;

			if (children != null)
				Children = new ItemList(children);
		}

		public override string IconUrl
		{
			get { return "{IconsUrl}/sitemap.png"; }
		}

		public string DirectUrl
		{
			get { return Parent.Url; }
		}
	}
}