using System.Collections.Generic;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Web;

namespace N2.Edit.Installation
{
    /// <summary>
    /// Rebases links in properties.
    /// </summary>
    [Service]
    public class AppPathRebaser
    {
        IItemFinder finder;
        IPersister persister;
        IHost host;

        public AppPathRebaser(IItemFinder finder, IPersister persister, IHost host)
        {
            this.finder = finder;
            this.persister = persister;
            this.host = host;
        }

        /// <summary>
        /// Rebases all items.
        /// </summary>
        /// <param name="fromUrl"></param>
        /// <param name="toUrl"></param>
        /// <returns></returns>
        /// <remarks>The return enumeration must be enumerated in order for the changes to take effect.</remarks>
        public IEnumerable<RebaseInfo> Rebase(string fromUrl, string toUrl)
        {
            using(var tx = persister.Repository.BeginTransaction())
            {
                foreach (var item in finder.All.Select())
                {
                    bool changed = false;
                    foreach (var info in Rebase(item, fromUrl, toUrl))
                    {
                        changed = true;
                        yield return info;
                    }
                    if(changed)
                        persister.Repository.SaveOrUpdate(item);
                }

                ContentItem root = persister.Get(host.DefaultSite.RootItemID);
                root[InstallationManager.InstallationAppPath] = toUrl;
                persister.Repository.SaveOrUpdate(root);

                persister.Repository.Flush();
                tx.Commit();
            }
        }

        /// <summary>
        /// Rebases a single item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fromUrl"></param>
        /// <param name="toUrl"></param>
        /// <returns></returns>
        public static IEnumerable<RebaseInfo> Rebase(ContentItem item, string fromUrl, string toUrl)
        {
            var rebasedLinks = new List<RebaseInfo>();
            foreach (var pi in item.GetContentType().GetProperties())
            {
                if(pi.CanRead == false || pi.CanWrite == false || pi.PropertyType != typeof(string))
                    continue;

                foreach (IRelativityTransformer transformer in pi.GetCustomAttributes(typeof(IRelativityTransformer), false))
                {
                    if(transformer.RelativeWhen != RelativityMode.Always && transformer.RelativeWhen != RelativityMode.Rebasing)
                        continue;

                    string original = pi.GetValue(item, null) as string;
                    string rebased = transformer.Rebase(original, fromUrl, toUrl);
                    if(!string.Equals(original, rebased))
                    {
                        pi.SetValue(item, rebased, null);
                        rebasedLinks.Add(new RebaseInfo { ItemID = item.ID, ItemTitle = item.Title, ItemPath = item.Path, PropertyName = pi.Name });
                    }
                }
            }
            return rebasedLinks;
        }
    }
}
