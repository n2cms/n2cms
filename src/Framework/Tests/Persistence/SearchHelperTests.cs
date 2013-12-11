using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence;
using Shouldly;

namespace N2.Tests.Persistence
{
    [TestFixture]
    public class SearchHelperTests : DatabasePreparingBase
    {
        [Test]
        public void PublishedPagesBelow_ShouldFilter_ByState()
        {
            var root = CreateOneItem<Definitions.PersistableItem2>(0, "root", null);
            var item1 = CreateOneItem<Definitions.PersistableItem>(0, "item1", root);
            var item2 = CreateOneItem<Definitions.PersistableItem>(0, "item2", root);
            var itemNew = CreateOneItem<Definitions.PersistableItem>(0, "itemNew", root);
            itemNew.State = ContentState.New;
            var itemDeleted = CreateOneItem<Definitions.PersistableItem>(0, "itemDeleted", root);
            itemDeleted.State = ContentState.Deleted;
            var itemUnpublished = CreateOneItem<Definitions.PersistableItem>(0, "itemUnpublished", root);
            itemUnpublished.State = ContentState.Unpublished;
            var itemWaiting = CreateOneItem<Definitions.PersistableItem>(0, "itemWaiting", root);
            itemWaiting.State = ContentState.Waiting;

            engine.Persister.Repository.SaveOrUpdate(root);
            engine.Persister.Repository.SaveOrUpdate(root.Children.ToArray());

            new SearchHelper(() => engine).Find.All.Count().ShouldBe(7);
                             
            new SearchHelper(() => engine).Items.Count().ShouldBe(7);
            new SearchHelper(() => engine).Pages.Count().ShouldBe(7);
            new SearchHelper(() => engine).Pages.Where(p => p.State == ContentState.Published).Count().ShouldBe(3);
            new SearchHelper(() => engine).PublishedPages.Count().ShouldBe(3);
            new SearchHelper(() => engine).PublishedPagesBelow(root).Count().ShouldBe(3);
            new SearchHelper(() => engine).PublishedPagesBelow(root).OfType<Definitions.PersistableItem>().ToList().Count().ShouldBe(2);
        }
    }
}
