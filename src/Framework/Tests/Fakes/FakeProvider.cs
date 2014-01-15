using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Tests.Fakes
{
    public class FakeProvider<T> : IProvider<T>
        where T : class
    {
        private Func<T> factory;
        private Func<IEnumerable<T>> allFactory;

        public FakeProvider(Func<T> factory, Func<IEnumerable<T>> allFactory = null)
        {
            this.factory = factory;
            this.allFactory = allFactory;
        }

        public T Get()
        {
            return factory();
        }

        public IEnumerable<T> GetAll()
        {
            return allFactory();
        }
    }
}
