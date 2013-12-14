using System;
using N2.Engine;

namespace N2.Tests
{
    public abstract class TypeFindingBase : ItemPersistenceMockingBase
    {
        protected ITypeFinder typeFinder;

        protected abstract Type[] GetTypes();
        public override void SetUp()
        {
            base.SetUp();

            typeFinder = new Fakes.FakeTypeFinder(typeof (TypeFindingBase).Assembly, GetTypes());
        }
    }
}
