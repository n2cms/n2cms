using System;
using System.Collections.Generic;
using N2.Persistence;
using N2.Persistence.Finder;

namespace N2.Addons.MyAddon.Services
{
    /// <summary>
    /// A component is a piece of code that is thrown into the IoC container by MyInitializer. 
    /// It's a good idea to pass it's dependencies through the constructor.
    /// </summary>
    public class MyComponent
    {
        private readonly IItemFinder finder;
        private readonly IPersister persister;

        public MyComponent(IItemFinder finder, IPersister persister)
        {
            this.finder = finder;
            this.persister = persister;
        }

        public virtual void VisitParts()
        {
            IList<Items.MyPart> parts = MyParts()
                .Select<Items.MyPart>();

            foreach (Items.MyPart part in parts)
            {
                part.TimesVisited++;
                part.LastVisited = DateTime.Now;
                persister.Save(part);
            }
        }

        public virtual IQueryAction MyParts()
        {
            return finder
                .Where.Type.Eq(typeof (Items.MyPart));
        }
    }
}
