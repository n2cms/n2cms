using System.Collections.Generic;
using System.Linq;
using N2.Persistence;

namespace N2.Tests.Fakes
{
    public class FakeDescendantItemFinder : DescendantItemFinder
    {
        public FakeDescendantItemFinder()
            : base(null, null)
        {
        }

        public override IEnumerable<T> Find<T>(ContentItem root)
        {
            return N2.Find.EnumerateChildren(root, true).OfType<T>();
        }
    }
}
