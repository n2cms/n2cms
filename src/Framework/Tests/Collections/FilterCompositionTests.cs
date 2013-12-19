using N2.Collections;
using NUnit.Framework;
using System;
using System.Linq;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class FilterCompositionTests : FilterTestsBase
    {
        ItemList list;
        ContentItem item1, item2, item3;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            list = new ItemList();
            list.Add(item1 = CreateOneItem<FirstItem>(1, "one", null));
            list.Add(item2 = CreateOneItem<SecondItem>(2, "two", item1));
            list.Add(item3 = CreateOneItem<NonPageItem>(3, "three", item2));

            item1.Published = N2.Utility.CurrentTime().AddSeconds(10);
        }

        [Test]
        public void Filter_And()
        {
            var filter = new PageFilter() & new PublishedFilter();

            var results = filter.Pipe(list);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results, Is.EquivalentTo(new[] { item2 }));
        }

        [Test]
        public void Filter_And_ViaPlus()
        {
            var filter = new PageFilter() + new PublishedFilter();

            var results = filter.Pipe(list);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results, Is.EquivalentTo(new[] { item2 }));
        }

        [Test]
        public void Filter_Or()
        {
            var filter = N2.Content.Is.Part() | N2.Content.Is.Page();

            var results = filter.Pipe(list);

            Assert.That(results.Count(), Is.EqualTo(3));
            Assert.That(results, Is.EquivalentTo(new[] { item1, item2, item3 }));
        }

        [Test]
        public void Filter_AndOr()
        {
            var filter = (N2.Content.Is.Part() | N2.Content.Is.Page()) & N2.Content.Is.Published();

            var results = filter.Pipe(list);

            Assert.That(results.Count(), Is.EqualTo(2));
            Assert.That(results, Is.EquivalentTo(new[] { item2, item3 }));
        }

        [Test]
        public void Filter_Not()
        {
            var filter = N2.Content.Is.Page() - N2.Content.Is.Published();

            var results = filter.Pipe(list);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results, Is.EquivalentTo(new[] { item1 }));
        }
    }
}
