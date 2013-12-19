using System;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using N2.Persistence.Sources;

namespace N2.Edit
{
    [Service]
    public class Navigator
    {
        private IPersister persister;
        private IHost host;
        private VirtualNodeFactory virtualNodes;
        private ContentSource sources;
        
        public Navigator(IPersister persister, IHost host, VirtualNodeFactory nodes, ContentSource sources)
        {
            this.persister = persister;
            this.host = host;
            this.virtualNodes = nodes;
            this.sources = sources;
        }

        public virtual ContentItem Navigate(ContentItem startingPoint, string path)
        {
            return startingPoint.GetChild(path)
                ?? sources.ResolvePath(startingPoint, path).CurrentItem
                ?? virtualNodes.Get(startingPoint.Path + path.TrimStart('/'))
                ?? virtualNodes.Get(path);
        }

        public virtual ContentItem Navigate(string path)
        {
            if (path == null) 
                return null;

            if (!path.StartsWith("/"))
            {
                if (path.StartsWith("~"))
                {
                    return Navigate(persister.Get(host.CurrentSite.StartPageID), path.Substring(1))
                        ?? sources.ResolvePath(path).CurrentItem
                        ?? virtualNodes.Get(path);
                }
                throw new ArgumentException("The path must start with a slash '/', was '" + path + "'", "path");
            }

            return Navigate(persister.Get(host.CurrentSite.RootItemID), path);
        }
    }
}
