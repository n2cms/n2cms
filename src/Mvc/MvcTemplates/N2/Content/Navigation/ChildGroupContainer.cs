using N2.Collections;
using N2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Content.Navigation
{
    [Versionable(AllowVersions.No)]
    [Throwable(AllowInTrash.No)]
    public class ChildGroupContainer : ContentItem, IUrlSource, ISystemNode
    {
        public ChildGroupContainer()
        {
        }

        public ChildGroupContainer(ContentItem parent, string title, string name, Func<IEnumerable<ContentItem>> childFactory = null)
        {
            ID = -1;
            Title = title;
            Name = name;
            Parent = parent;

            if (childFactory != null)
                Children = new ItemList(childFactory);
        }

        public override string IconUrl
        {
            get { return "{IconsUrl}/folder_page.png"; }
        }

        public string DirectUrl
        {
            get { return Parent.Url; }
        }

        internal static ContentItem Create(ContentItem parent, string title, string name, Func<IEnumerable<ContentItem>> childFactory)
        {
            return new ChildGroupContainer(parent, title, name, childFactory);
        }
    }
}
