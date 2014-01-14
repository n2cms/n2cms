using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Tests.Definitions.Items;
using N2.Definitions;
using Shouldly;

namespace N2.Tests.Definitions
{
    [TestFixture]
    public class SortChildrenTests
    {
        ContentItem parent;

        [SetUp]
        public void SetUp()
        {
            parent = new DefinitionOne();
            new DefinitionOne { SortOrder = 1, Name = "c", Published = new DateTime(2010, 11, 10), Title = "b", Updated = new DateTime(2011, 01, 10) }.AddTo(parent);
            new DefinitionOne { SortOrder = 2, Name = "a", Published = new DateTime(2010, 10, 10), Title = "A", Updated = new DateTime(2010, 10, 10) }.AddTo(parent);
            new DefinitionOne { SortOrder = 0, Name = "b", Published = new DateTime(2010, 12, 10), Title = "C", Updated = new DateTime(2010, 12, 10) }.AddTo(parent);
        }

        [Test]
        public void SortBy_CurrentOrder()
        {
            new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

            parent.Children[0].SortOrder.ShouldBe(1);
            parent.Children[1].SortOrder.ShouldBe(2);
            parent.Children[2].SortOrder.ShouldBeGreaterThan(8);
        }

        [Test]
        public void SortBy_CurrentOrder_EqualStartingGround()
        {
            parent.Children[0].SortOrder = 0;
            parent.Children[1].SortOrder = 0;
            parent.Children[2].SortOrder = 0;
            new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

            parent.Children[0].SortOrder.ShouldBeLessThan(-9);
            parent.Children[1].SortOrder.ShouldBeLessThan(-4);
            parent.Children[2].SortOrder.ShouldBe(0);
        }

        [Test]
        public void SortBy_CurrentOrder_EqualStartingGround_MuchosItemos()
        {
            parent.Children[0].SortOrder = 0;
            parent.Children[1].SortOrder = 0;
            parent.Children[2].SortOrder = 0;
            for (int i = parent.Children.Count; i <= 15; i++)
            {
                new DefinitionOne { SortOrder = 0, Name = "x" + i }.AddTo(parent);
            }

            new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

            for (int i = 1; i < parent.Children.Count; i++)
            {
                parent.Children[i].SortOrder.ShouldBeGreaterThan(parent.Children[i - 1].SortOrder);
            }
        }

        [Test]
        public void SortBy_CurrentOrder_MinimallyReorders_FromLast()
        {
            var reorderedItems = new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

            reorderedItems.Single().Title.ShouldBe("C");
        }

        [Test]
        public void SortBy_AddsPaddingBetweenSortOrders()
        {
            var reorderedItems = new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

            reorderedItems.Single().SortOrder.ShouldBeGreaterThanOrEqualTo(parent.Children[1].SortOrder + 10);
        }

        [Test]
        public void SortBy_CurrentOrder_MinimallyReorders_FromBeginning()
        {
            parent.Children[1].SortOrder = -1;

            var reorderedItems = new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

            reorderedItems.Single().Title.ShouldBe("b");
        }

        [Test]
        public void SortBy_AddsPadding_BeforeNextItem_WhenAddingToBeginning()
        {
            parent.Children[1].SortOrder = -1;

            new SortChildrenAttribute(SortBy.CurrentOrder).ReorderChildren(parent);

            parent.Children[0].SortOrder.ShouldBeLessThan(parent.Children[1].SortOrder - 9);
        }

        [Test]
        public void SortBy_Expression()
        {
            new SortChildrenAttribute(SortBy.Expression) { SortExpression = "Name" }.ReorderChildren(parent);

            Assert.That(parent.Children[0].Name, Is.EqualTo("a"));
            Assert.That(parent.Children[1].Name, Is.EqualTo("b"));
            Assert.That(parent.Children[2].Name, Is.EqualTo("c"));
        }

        [Test]
        public void SortBy_Expression_DESC()
        {
            new SortChildrenAttribute(SortBy.Expression) { SortExpression = "Name DESC" }.ReorderChildren(parent);

            Assert.That(parent.Children[0].Name, Is.EqualTo("c"));
            Assert.That(parent.Children[1].Name, Is.EqualTo("b"));
            Assert.That(parent.Children[2].Name, Is.EqualTo("a"));
        }

        [Test]
        public void SortBy_Published()
        {
            new SortChildrenAttribute(SortBy.Published).ReorderChildren(parent);

            Assert.That(parent.Children[0].Published, Is.EqualTo(new DateTime(2010, 10, 10)));
            Assert.That(parent.Children[1].Published, Is.EqualTo(new DateTime(2010, 11, 10)));
            Assert.That(parent.Children[2].Published, Is.EqualTo(new DateTime(2010, 12, 10)));
        }

        [Test]
        public void SortBy_PublishedDescending()
        {
            new SortChildrenAttribute(SortBy.PublishedDescending).ReorderChildren(parent);

            Assert.That(parent.Children[0].Published, Is.EqualTo(new DateTime(2010, 12, 10)));
            Assert.That(parent.Children[1].Published, Is.EqualTo(new DateTime(2010, 11, 10)));
            Assert.That(parent.Children[2].Published, Is.EqualTo(new DateTime(2010, 10, 10)));
        }

        [Test]
        public void SortBy_Title()
        {
            new SortChildrenAttribute(SortBy.Title).ReorderChildren(parent);

            Assert.That(parent.Children[0].Title, Is.EqualTo("A"));
            Assert.That(parent.Children[1].Title, Is.EqualTo("b"));
            Assert.That(parent.Children[2].Title, Is.EqualTo("C"));
        }

        [Test]
        public void SortBy_Unordered()
        {
            new SortChildrenAttribute(SortBy.Unordered).ReorderChildren(parent);

            Assert.That(parent.Children[0].Name, Is.EqualTo("c"));
            Assert.That(parent.Children[1].Name, Is.EqualTo("a"));
            Assert.That(parent.Children[2].Name, Is.EqualTo("b"));
        }

        [Test]
        public void SortBy_Updated()
        {
            new SortChildrenAttribute(SortBy.Updated).ReorderChildren(parent);

            Assert.That(parent.Children[0].Updated, Is.EqualTo(new DateTime(2010, 10, 10)));
            Assert.That(parent.Children[1].Updated, Is.EqualTo(new DateTime(2010, 12, 10)));
            Assert.That(parent.Children[2].Updated, Is.EqualTo(new DateTime(2011, 01, 10)));
        }

        [Test]
        public void SortBy_UpdatedDescending()
        {
            new SortChildrenAttribute(SortBy.UpdatedDescending).ReorderChildren(parent);

            Assert.That(parent.Children[0].Updated, Is.EqualTo(new DateTime(2011, 01, 10)));
            Assert.That(parent.Children[1].Updated, Is.EqualTo(new DateTime(2010, 12, 10)));
            Assert.That(parent.Children[2].Updated, Is.EqualTo(new DateTime(2010, 10, 10)));
        }
    }
}
