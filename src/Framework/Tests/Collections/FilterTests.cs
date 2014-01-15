using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Collections;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class FilterTests : FilterTestsBase
    {
        ContentItem one;
        ContentItem two;
        List<ContentItem> items;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            one = CreateOneItem<FirstItem>(1, "one", null);
            two = CreateOneItem<FirstItem>(2, "two", null);
            items = new List<ContentItem> { one, two };
        }

        [Test]
        public void AnyFilter_MatchesForAnyMatch()
        {
            new AnyFilter(new VisibleFilter(), new StateFilter(ContentState.Deleted)).Filter(items);
            Assert.That(items.Count, Is.EqualTo(2));
        }

        [Test]
        public void StateFilter_FiltersNotOfState()
        {
            one.State = ContentState.Deleted;

            new StateFilter(ContentState.Deleted).Filter(items);
            Assert.That(items.Single(), Is.EqualTo(one));
        }

        [Test]
        public void PartFilter_FiltersPages()
        {
            var part = CreateOneItem<NonPageItem>(3, "three", null);
            items.Add(part);

            new PartFilter().Filter(items);
            Assert.That(items.Single(), Is.EqualTo(part));
        }
    }
}
