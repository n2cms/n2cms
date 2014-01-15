using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions.Static;
using N2.Persistence.Finder;
using N2.Persistence.NH.Finder;

namespace N2.Tests.Fakes
{
    public class FakeItemFinder : IItemFinder
    {
        public FakeItemFinder()
            : this(() => Enumerable.Empty<ContentItem>())
        {
        }
        public FakeItemFinder(Func<IEnumerable<ContentItem>> selector)
        {
            Selector = selector; 
        }

        public Func<IEnumerable<ContentItem>> Selector { get; set; }

        #region IItemFinder Members

        public IQueryBuilder Where
        {
            get { return new FakeQueryBuilder(() => Selector()); }
        }

        public IQueryEnding All
        {
            get { return new FakeQueryBuilder(() => Selector()); }
        }

        #endregion

        class FakeQueryBuilder : QueryBuilder
        {
            public FakeQueryBuilder(Func<IEnumerable<ContentItem>> selector)
                : base(null, new DefinitionMap())
            {
                this.Selector = selector;
            }

            public Func<IEnumerable<ContentItem>> Selector { get; set; }

            public override System.Collections.Generic.IList<T> Select<T>()
            {
                return Selector().Cast<T>().ToList();
            }
            public override int Count()
            {
                return Selector().Count();
            }
        }
    }
}
