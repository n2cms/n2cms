using System;

namespace N2.Tests.Persistence
{
    [Obsolete]
    public class FinderFixture : DatabasePreparingBase
    {
        //#region Members
        //Definitions.PersistableItem root;
        //Definitions.PersistableItem[] children; 
        //#endregion

        //#region SetUp
        //[SetUp]
        //public override void SetUp()
        //{
        //    base.SetUp();
        //    ((Fakes.FakeWebContextWrapper)factory.Resolve<N2.Web.IWebContext>()).currentUser = SecurityUtilities.CreatePrincipal("Tester");

        //    using (factory.Persister)
        //    {
        //        root = CreateRoot("root", "Root Item");

        //        root.Created = new DateTime(2007, 03, 17);
        //        root.Expires = new DateTime(2077, 03, 17);
        //        root.Published = new DateTime(2007, 03, 17);
        //        root.SortOrder = 666;
        //        root.Visible = false;
        //        root.ZoneName = "zone of pain";

        //        root.BoolProperty = false;
        //        root.DateTimeProperty = new DateTime(1978, 12, 02);
        //        root.DoubleProperty = 3.1412;
        //        root.IntProperty = 42;
        //        root.ObjectProperty = new string[] { "one", "two", "three" };
        //        root.StringProperty = "dida";

        //        children = new Definitions.PersistableItem[6];
        //        for (int i = 0; i < 6; i++)
        //        {
        //            children[i] = CreateAndSaveItem("item" + (i + 1), "Item " + (i + 1), root);
        //            children[i].IntProperty = i;
        //        }
        //        root.LinkProperty = children[0];

        //        factory.Persister.Save(root);
        //        foreach (ContentItem item in children)
        //            factory.Persister.Save(item);
        //    }
        //} 
        //#endregion

        //#region Methods
        //private void AssertContainsOnlyRoot(IList<ContentItem> listWithRoot)
        //{
        //    Assert.AreEqual(1, listWithRoot.Count);
        //    Assert.AreEqual(root, listWithRoot[0]);
        //} 
        //#endregion

        //#region Find Single ContentItem Properties
        //[Test]
        //public void FindNameEq()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Name", Comparison.Equal, "root")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindNameLike()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Name", Comparison.Like, "root")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindNameLikeBeginning()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Name", Comparison.Like, "ro%")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindNameLikeEnd()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Name", Comparison.Like, "%ot")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindNameLikeContains()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Name", Comparison.Like, "%oo%")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindTitle()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Title", Comparison.Equal, "Root Item")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindCreated()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Created", Comparison.Equal, new DateTime(2007, 03, 17))
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindExpires()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Expires", Comparison.Equal, new DateTime(2077, 03, 17))
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindExpiresGe()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Expires", Comparison.GreaterOrEqual, new DateTime(2077, 3, 17))
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindPublished()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Published", Comparison.Equal, new DateTime(2007, 03, 17))
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}


        //[Test]
        //public void FindPublishedLe()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Published", Comparison.LessOrEqual, new DateTime(2007, 03, 17))
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindSortOrder()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("SortOrder", Comparison.Equal, 666)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}
        //[Test]
        //public void FindVisible()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Visible", Comparison.Equal, false)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindZoneName()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("ZoneName", Comparison.Equal, "zone of pain")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindUpdatedLt()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("Updated", Comparison.LessThan, new DateTime(2077, 1, 1))
        //        .List();
        //    Assert.AreEqual(7, listWithRoot.Count);
        //}

        //[Test]
        //public void FindUpdatedGt()
        //{
        //    IList<ContentItem> list = factory.Persister.Finder
        //        .SetExpression("Updated", Comparison.GreaterThan, new DateTime(2007, 3, 18))
        //        .List();
        //    Assert.AreEqual(7, list.Count);
        //}

        //[Test]
        //public void FindSavedBy()
        //{
        //    IList<ContentItem> list = factory.Persister.Finder
        //        .SetExpression("SavedBy", Comparison.Equal, "Tester")
        //        .List();
        //    Assert.AreEqual(7, list.Count);
        //} 
        //#endregion

        //#region Find Single ContentItem Details
        //[Test]
        //public void FindBoolDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("BoolProperty", Comparison.Equal, false)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindDateTimeDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("DateTimeProperty", Comparison.Equal, new DateTime(1978, 12, 02))
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindDoubleDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("DoubleProperty", Comparison.Equal, 3.1412)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindIntDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("IntProperty", Comparison.Equal, 42)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindLinkDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("LinkProperty", Comparison.Equal, children[0])
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindObjectDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("ObjectProperty", Comparison.Equal, new string[] { "one", "two", "three" })
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindStringDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetExpression("StringProperty", Comparison.Equal, "dida")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}
        //#endregion

        //#region Find Nameless ContentItem Detail
        //[Test]
        //public void FindAnyBoolDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.Equal, false)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindAnyDateTimeDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.Equal, new DateTime(1978, 12, 02))
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindAnyDoubleDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.Equal, 3.1412)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindAnyIntDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.Equal, 42)
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindAnyLinkDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.Equal, children[0])
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindAnyObjectDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.Equal, new string[] { "one", "two", "three" })
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}

        //[Test]
        //public void FindAnyStringDetail()
        //{
        //    IList<ContentItem> listWithRoot = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.Equal, "dida")
        //        .List();
        //    AssertContainsOnlyRoot(listWithRoot);
        //}
        //#endregion

        //#region Sort

        //[Test]
        //public void SortTitleAscending()
        //{
        //    IList<ContentItem> items = factory.Persister.Finder
        //        .SetExpression("Title", Comparison.Like, "%")
        //        .SetSortExpression("Title ASC")
        //        .List();
        //    Assert.AreEqual(7, items.Count);
        //    Assert.AreEqual("Item 1", items[0].Title);
        //    Assert.AreEqual("Root Item", items[6].Title);
        //}

        //[Test]
        //public void SortTitleDescending()
        //{
        //    IList<ContentItem> items = factory.Persister.Finder
        //        .SetExpression("Title", Comparison.Like, "%")
        //        .SetSortExpression("Title DESC")
        //        .List();
        //    Assert.AreEqual(7, items.Count);
        //    Assert.AreEqual("Root Item", items[0].Title);
        //    Assert.AreEqual("Item 1", items[6].Title);
        //}

        ////sorting on details isn't supported
        ////[Test, ExpectedException(typeof(NotSupportedException))]
        ////public void SortDetailDescendingCausesException()
        ////{
        ////    IList<ContentItem> items = Factory.Persister.Finder
        ////        .SetExpression("Title", Comparison.Like, "%")
        ////        .SetSortExpression("IntDetail DESC")
        ////        .List();
        ////}
        //#endregion

        //[Test]
        //public void FindSeveralIntDetails()
        //{
        //    IList<ContentItem> list = factory.Persister.Finder
        //        .SetNamelessDetailExpression(Comparison.GreaterOrEqual, 5)
        //        .List();
        //    Assert.AreEqual(2, list.Count);
        //}

    }
}
