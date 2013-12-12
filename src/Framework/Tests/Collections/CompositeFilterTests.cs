using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class CompositeFilterTests : FilterTestsBase
    {
        public ItemList CreateItems(int numberOfItems)
        {
            ItemList items = new ItemList();
            for (int i = 0; i < numberOfItems; i++)
            {
                items.Add(CreateOneItem<FirstItem>(i + 1, i.ToString(), null));
            }
            return items;
        }

        [Test]
        public void Wrap_Creates_NullFilter_ForNull()
        {
            var filter = AllFilter.Wrap(null);
            Assert.That(filter, Is.InstanceOf<NullFilter>());
        }

        [Test]
        public void Wrap_Creates_NullFilter_ForZeroLengthArray()
        {
            var filter = AllFilter.Wrap(new ItemFilter[0]);
            Assert.That(filter, Is.InstanceOf<NullFilter>());
        }

        [Test]
        public void Wrap_Creates_NullFilter_ForZeroParameters()
        {
            var filter = AllFilter.Wrap();
            Assert.That(filter, Is.InstanceOf<NullFilter>());
        }

        [Test]
        public void Wrap_Creates_NullFilter_ForZeroCountList()
        {
            var filter = AllFilter.Wrap(new List<ItemFilter>());
            Assert.That(filter, Is.InstanceOf<NullFilter>());
        }

        [Test]
        public void Wrap_Creates_SameFilter_ForOneParameter()
        {
            var filter = AllFilter.Wrap(new CountFilter(0, 1));
            Assert.That(filter, Is.InstanceOf<CountFilter>());
        }

        [Test]
        public void Wrap_Creates_SameFilter_ForOneCountList()
        {
            var filter = AllFilter.Wrap(new List<ItemFilter> { new CountFilter(0, 1) });
            Assert.That(filter, Is.InstanceOf<CountFilter>());
        }

        [Test]
        public void Wrap_Creates_CompositeFilter_ForTwoParameters()
        {
            var filter = AllFilter.Wrap(new CountFilter(0, 1), new TypeFilter(typeof(INameable)));
            Assert.That(filter, Is.InstanceOf<AllFilter>());
            Assert.That((filter as AllFilter).Filters.Length, Is.EqualTo(2));
        }

        [Test]
        public void Wrap_Creates_CompositeFilter_ForTwoCountList()
        {
            var filter = AllFilter.Wrap(new List<ItemFilter> { new CountFilter(0, 1), new TypeFilter(typeof(INameable)) });
            Assert.That(filter, Is.InstanceOf<AllFilter>());
            Assert.That((filter as AllFilter).Filters.Length, Is.EqualTo(2));
        }

        [Test]
        public void CompositeFilter_PipesItem_MatchedByAllFilters()
        {
            var filter = AllFilter.Wrap(new DelegateFilter(i => true), new CountFilter(1, 1));
            var items = filter.Pipe(CreateItems(3)).ToList();
            Assert.That(items.Count(), Is.EqualTo(1));
            Assert.That(items.First().Name, Is.EqualTo("1"));
        }

        [Test]
        public void CompositeFilter_ItemsMustMatchByAll()
        {
            var filter = AllFilter.Wrap(new DelegateFilter(i => false), new CountFilter(1, 1));
            var items = filter.Pipe(CreateItems(3)).ToList();
            Assert.That(items.Count(), Is.EqualTo(0));
        }
    }
}
