using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence;

namespace N2.Tests.Fakes
{
    public class FakeDependencyInjector : IDependencyInjector
    {
        public readonly List<IDependencySetter> injectors = new List<IDependencySetter>();
        #region IDependencyInjector Members

        public bool FulfilDependencies(ContentItem item)
        {
            foreach (var injector in injectors)
                injector.Fulfil(item);
            return true;
        }

        #endregion
    }
}
