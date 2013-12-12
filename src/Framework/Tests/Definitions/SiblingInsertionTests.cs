using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Tests.Definitions.Items;
using N2.Definitions;
using Shouldly;
using N2.Persistence.Behaviors;

namespace N2.Tests.Definitions
{
    [TestFixture]
    public class SiblingInsertionTests
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
            var item = AddIt(new DefinitionOne { Parent = parent }, SortBy.CurrentOrder);

            parent.Children.Last().ShouldBe(item);
        }

        [Test]
        public void SortBy_Expression_First()
        {
            var item = AddIt(new DefinitionOne { Name = "_", Parent = parent }, SortBy.Expression, "Name");

            parent.Children.First().ShouldBe(item);
        }

        [Test]
        public void SortBy_Expression_Last()
        {
            var item = AddIt(new DefinitionOne { Name = "z", Parent = parent }, SortBy.Expression, "Name");

            parent.Children.Last().ShouldBe(item);
        }

        [Test]
        public void SortBy_Expression_Middle()
        {
            var item = AddIt(new DefinitionOne { Title = "b", Parent = parent }, SortBy.Expression, "Title");

            parent.Children[2].ShouldBe(item);
        }

        [Test]
        public void SortBy_Expression_DESC()
        {
            var item = AddIt(new DefinitionOne { Name = "z", Parent = parent }, SortBy.Expression, "Name DESC");

            parent.Children.First().ShouldBe(item);
        }

        [Test]
        public void SortBy_Published()
        {
            var item = AddIt(new DefinitionOne { Published = DateTime.MinValue, Parent = parent }, SortBy.Published);

            parent.Children.First().ShouldBe(item);
        }

        [Test]
        public void SortBy_Published_null()
        {
            var item = AddIt(new DefinitionOne { Published = null, Parent = parent }, SortBy.Published);

            parent.Children.First().ShouldBe(item);
        }

        [Test]
        public void SortBy_PublishedDescending()
        {
            var item = AddIt(new DefinitionOne { Published = DateTime.MinValue, Parent = parent }, SortBy.PublishedDescending);

            parent.Children.Last().ShouldBe(item);
        }

        [Test]
        public void SortBy_Title()
        {
            var item = AddIt(new DefinitionOne { Title = "z", Parent = parent }, SortBy.Title);

            parent.Children.Last().ShouldBe(item);
        }

        [Test]
        public void SortBy_Unordered()
        {
            var item = AddIt(new DefinitionOne { Parent = parent }, SortBy.Unordered);

            parent.Children.Contains(item).ShouldBe(false);
        }

        [Test]
        public void SortBy_Updated()
        {
            var item = AddIt(new DefinitionOne { Updated = N2.Utility.CurrentTime(), Parent = parent }, SortBy.Updated);

            parent.Children.Last().ShouldBe(item);
        }

        [Test]
        public void SortBy_UpdatedDescending()
        {
            var item = AddIt(new DefinitionOne { Updated = N2.Utility.CurrentTime(), Parent = parent }, SortBy.UpdatedDescending);

            parent.Children.First().ShouldBe(item);
        }

        private ContentItem AddIt(ContentItem item, SortBy order, string expression = null)
        {
            new SiblingInsertionAttribute(order) { SortExpression = expression }.OnSaving(new BehaviorContext { AffectedItem = item });
            return item;
        }
    }
}
