using N2.Engine;
using N2.Persistence.Finder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    [Service(typeof(IItemFinder),
        Configuration = "mongo",
        Replaces = typeof(N2.Persistence.NH.Finder.ItemFinder))]
    public class MongoFinder : IItemFinder
    {
        public IQueryBuilder Where
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryEnding All
        {
            get { throw new NotImplementedException(); }
        }
    }
}
