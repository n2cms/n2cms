using System;
using System.Collections.Generic;
using System.Text;

using N2;
using NUnit.Framework;
using N2.Persistence.NH;
using N2.Persistence;

namespace N2.Tests.Persistence
{
	[Obsolete]
	public class RelationFinderFixture : DatabasePreparingBase
	{
		//IVersionManager versioner;

		//public override void SetUp()
		//{
		//    base.SetUp();
		//    persister = GetNHibernatePersistenceManager();
		//    versioner = factory.Resolve<IVersionManager>();
		//}

		//private void CreateTheThreeItems()
		//{
		//    root = CreateRoot("root", "root item");
		//    item1 = CreateAndSaveItem("item1", "item one", root);
		//    item2 = CreateAndSaveItem("item2", "item two", root);
		//}

		//private void DeleteTheItems()
		//{
		//    persister.Delete(root);
		//    root = null;
		//    item1 = null;
		//    item2 = null;
		//}

		//DefaultPersistenceManager persister;
		//Definitions.PersistableItem root;
		//ContentItem item1;
		//ContentItem item2;

		//#region Adding references
		//[Test]
		//public void AddSingleReference()
		//{
		//    CreateTheThreeItems();

		//    item1["theOtherItem"] = item2;
		//    persister.Save(item1);

		//    Assert.AreEqual(item2, item1["theOtherItem"]);
		//    Assert.IsInstanceOfType(typeof(Details.LinkDetail), item1.Details["theOtherItem"]);
		//    Assert.AreEqual(item2, ((Details.LinkDetail)item1.Details["theOtherItem"]).LinkedItem);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void AddCollectionOfReferences()
		//{
		//    CreateTheThreeItems();

		//    root.GetDetailCollection("otherItems", true).Add(item1);
		//    root.GetDetailCollection("otherItems", true).Add(item2);
		//    persister.Save(item1);

		//    Assert.AreEqual(2, root.DetailCollections["otherItems"].Count);
		//    Assert.IsTrue(root.DetailCollections["otherItems"][0] == item1 && root.DetailCollections["otherItems"][1] == item2
		//        || root.DetailCollections["otherItems"][1] == item1 && root.DetailCollections["otherItems"][0] == item2);

		//    DeleteTheItems();
		//} 
		//#endregion

		//#region Referrers
		//[Test]
		//public void FindSingleReferrerInCollection()
		//{
		//    CreateTheThreeItems();

		//    DefaultPersistenceManager persister = GetNHibernatePersistenceManager();

		//    item1.GetDetailCollection("theOtherItem", true).Add(item2);
		//    persister.Save(item1);
		//    Assert.AreEqual(item1.GetDetailCollection("theOtherItem", false).Count, 1);
		//    Assert.AreEqual(item1.GetDetailCollection("theOtherItem", false)[0], item2);

		//    IList<ContentItem> referrers = persister.RelationFinder.ListReferrers(item2);
		//    Assert.AreEqual(referrers.Count, 1);
		//    Assert.AreEqual(referrers[0], item1);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindSingleReferrer()
		//{
		//    CreateTheThreeItems();

		//    item1["theOtherItem"] = item2;
		//    persister.Save(item1);

		//    IList<ContentItem> referrers = persister.RelationFinder
		//        .ListReferrers(item2);
		//    Assert.AreEqual(1, referrers.Count);
		//    Assert.AreEqual(item1, referrers[0]);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindTwoReferrers()
		//{
		//    CreateTheThreeItems();

		//    item1["theOtherItem"] = root;
		//    item2["theOtherItem"] = root;
		//    persister.Save(item1);
		//    persister.Save(item2);

		//    IList<ContentItem> referrers = persister.RelationFinder
		//        .ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 2);
		//    Assert.IsTrue(referrers[0] == item1 && referrers[1] == item2
		//        || referrers[1] == item1 && referrers[0] == item2);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindReferrersByReferenceName()
		//{
		//    CreateTheThreeItems();

		//    item1["theOtherItem"] = item2;
		//    persister.Save(item1);

		//    IList<ContentItem> referrers = persister.RelationFinder
		//        .SetDetailName("theOtherItem")
		//        .ListReferrers(item2);
		//    Assert.AreEqual(referrers.Count, 1);
		//    Assert.AreEqual(referrers[0], item1);

		//    referrers = persister.RelationFinder
		//        .SetDetailName("notTheOtherItem")
		//        .ListReferrers(item2);
		//    Assert.AreEqual(referrers.Count, 0);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindReferrersWithCertainNameOnly()
		//{
		//    CreateTheThreeItems();

		//    item1["theOtherItem"] = root;
		//    persister.Save(item1);
		//    item2["relationWithAnotherName"] = root;
		//    persister.Save(item2);

		//    IList<ContentItem> referrers = persister.RelationFinder
		//        .SetDetailName("theOtherItem")
		//        .ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 1);
		//    Assert.AreEqual(referrers[0], item1);

		//    referrers = persister.RelationFinder
		//        .ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 2);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindReferrersWithReferenceCollection()
		//{
		//    CreateTheThreeItems();

		//    item2.GetDetailCollection("theOtherItemCollection", true).Add(root);
		//    item1.GetDetailCollection("theOtherItemCollection", true).Add(root);

		//    persister.Save(item1);
		//    persister.Save(item2);

		//    IList<ContentItem> referrers = persister.RelationFinder
		//        .ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 2);
		//    Assert.IsTrue(referrers[0] == item1 && referrers[1] == item2
		//        || referrers[1] == item1 && referrers[0] == item2);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindUsingFirstAndMaxResult()
		//{
		//    CreateTheThreeItems();
		//    ContentItem item3 = CreateAndSaveItem("item3", "item three", root);
		//    ContentItem item4 = CreateAndSaveItem("item4", "item four", root);

		//    DefaultPersistenceManager persister = GetNHibernatePersistenceManager();

		//    item1.GetDetailCollection("theOtherItemCollection", true).Add(root);
		//    item2.GetDetailCollection("theOtherItemCollection", true).Add(root);
		//    item3.GetDetailCollection("theOtherItemCollection", true).Add(root);
		//    item4.GetDetailCollection("theOtherItemCollection", true).Add(root);

		//    persister.Save(item1);
		//    persister.Save(item2);
		//    persister.Save(item3);
		//    persister.Save(item4);

		//    IList<ContentItem> referrers = persister.RelationFinder.ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 4);

		//    referrers = persister.RelationFinder
		//        .SetMaxResults(3)
		//        .SetSortExpression("Name ASC")
		//        .ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 3);
		//    ContentItem thirdItem = referrers[2];

		//    referrers = persister.RelationFinder
		//        .SetFirstResult(2)
		//        .SetSortExpression("Name ASC")
		//        .ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 2);
		//    Assert.AreEqual(thirdItem, referrers[0]);

		//    referrers = persister.RelationFinder
		//        .SetFirstResult(1)
		//        .SetMaxResults(2)
		//        .SetSortExpression("Name ASC")
		//        .ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 2);
		//    Assert.AreEqual(thirdItem, referrers[1]);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void EnsureThatOldVersionsOfReferrersDoesntShowUpByDefault()
		//{
		//    CreateTheThreeItems();

		//    AddCrossReference();

		//    IList<ContentItem> referrers = persister.RelationFinder.ListReferrers(item1);
		//    Assert.AreEqual(referrers.Count, 1);

		//    SaveVersions();
		//    SaveVersions();

		//    referrers = persister.RelationFinder.ListReferrers(item1);
		//    Assert.AreEqual(referrers.Count, 1);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void EnsureThatReferencesInCollectionsOfPreviousVersionsDoesntShowUpByDefault()
		//{
		//    CreateTheThreeItems();

		//    CreateSomeVersions();

		//    IList<ContentItem> referrers = persister.RelationFinder.ListReferrers(root);
		//    Assert.AreEqual(referrers.Count, 2);

		//    SaveVersions();

		//    referrers = persister.RelationFinder.ListReferrers(root);
		//    Assert.AreEqual(2, referrers.Count);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void EnsureThatWeCanFindReferrersThatArePreviousVersions()
		//{
		//    CreateTheThreeItems();

		//    AddCrossReference();
		//    SaveVersions();
		//    SaveVersions();

		//    IList<ContentItem> referrers = persister.RelationFinder.SetIncludePreviousVersions(true).ListReferrers(item1);
		//    Assert.AreEqual(3, referrers.Count);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void EnsureThatWeCanFindReferrersWithReferencesInCollectionsThatArePreviousVersions()
		//{
		//    CreateTheThreeItems();

		//    CreateSomeVersions();

		//    CheckVersionedReferenceCount(4, 1, 1);

		//    SaveVersions();

		//    CheckVersionedReferenceCount(6, 2, 2);

		//    DeleteTheItems();
		//}



		//private void CheckVersionedReferenceCount(int expectedTotal, int expectedVersionsOf1, int expectedVersionsOf2)
		//{
		//    IList<ContentItem> referrers = persister.RelationFinder
		//        .SetIncludePreviousVersions(true)
		//        .ListReferrers(root);
		//    Assert.AreEqual(expectedTotal, referrers.Count);

		//    int versionOf1 = 0;
		//    int versionOf2 = 0;
		//    foreach (ContentItem item in referrers)
		//    {
		//        if (item.VersionOf == item1)
		//            versionOf1++;
		//        else if (item.VersionOf == item2)
		//            versionOf2++;
		//    }
		//    Assert.AreEqual(expectedVersionsOf1, versionOf1);
		//    Assert.AreEqual(expectedVersionsOf2, versionOf2);
		//}


		//#region Version Helper Methods
		//private void CreateSomeVersions()
		//{
		//    SaveVersions();

		//    item1.GetDetailCollection("theOtherItemCollection", true).Add(root);
		//    item2.GetDetailCollection("theOtherItemCollection", true).Add(root);

		//    persister.Save(item1);
		//    persister.Save(item2);

		//    SaveVersions();
		//}

		//private void SaveVersions()
		//{
		//    versioner.SaveVersion(item1);
		//    versioner.SaveVersion(item2);
		//}

		//private void AddCrossReference()
		//{
		//    item1["mySibling"] = item2;
		//    item2["mySibling"] = item1;

		//    persister.Save(item1);
		//    persister.Save(item2);
		//} 
		//#endregion

		//[Test]
		//public void FindABunchOfReferrers()
		//{
		//    CreateTheThreeItems();
		//    ContentItem item3 = CreateAndSaveItem("item3", "item three", root);
		//    ContentItem item4 = CreateAndSaveItem("item4", "item four", root);
		//    FindABunchOfReferrers_Add40ItemsAndABunchOfReferences(item3, item4);

		//    IList<ContentItem> referrers = persister.RelationFinder.ListReferrers(root);
		//    Assert.AreEqual(44, referrers.Count);

		//    referrers = persister.RelationFinder
		//        .SetDetailName("rootLink")
		//        .ListReferrers(root);
		//    Assert.AreEqual(40, referrers.Count);

		//    referrers = persister.RelationFinder
		//        .SetDetailName("rootLinkFirstLevel")
		//        .ListReferrers(root);
		//    Assert.AreEqual(4, referrers.Count);

		//    referrers = persister.RelationFinder.ListReferrers(item1);
		//    Assert.AreEqual(10, referrers.Count);

		//    DeleteTheItems();
		//}

		//private void FindABunchOfReferrers_Add40ItemsAndABunchOfReferences(ContentItem item3, ContentItem item4)
		//{
		//    foreach (ContentItem level1Item in root.Children)
		//    {
		//        for (int i = 0; i < 10; i++)
		//        {
		//            ContentItem level2Item = CreateAndSaveItem("item" + i, "item" + i + 1, level1Item);
		//            level2Item["rootLink"] = root;
		//            level2Item["parentlink"] = level1Item;
		//            persister.Save(level2Item);
		//        }
		//        level1Item["rootLinkFirstLevel"] = root;
		//    }
		//    persister.Save(item1);
		//    persister.Save(item2);
		//    persister.Save(item3);
		//    persister.Save(item4);
		//}

		//#endregion

		//#region References
		//[Test]
		//public void FindReference()
		//{
		//    CreateTheThreeItems();

		//    item1["theOtherItem"] = item2;
		//    persister.Save(item1);
		//    Assert.AreEqual(item1["theOtherItem"], item2);

		//    IList<Details.LinkDetail> references = persister.RelationFinder.ListReferences(item2);
		//    Assert.AreEqual(references.Count, 1);
		//    Assert.AreEqual(references[0], item1.Details["theOtherItem"]);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindReferencesWithName()
		//{
		//    CreateTheThreeItems();

		//    item1["theOtherItem"] = root;
		//    persister.Save(item1);
		//    item2["theOtherItem"] = root;
		//    persister.Save(item2);

		//    IList<Details.LinkDetail> references = persister.RelationFinder
		//        .SetDetailName("theOtherItem")
		//        .ListReferences(root);
		//    Assert.AreEqual(2, references.Count);
		//    Assert.IsTrue(references[0] == item1.Details["theOtherItem"] && references[1] == item2.Details["theOtherItem"]
		//        || references[1] == item1.Details["theOtherItem"] && references[0] == item2.Details["theOtherItem"]);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindReferenceInCollection()
		//{
		//    CreateTheThreeItems();

		//    DefaultPersistenceManager persister = GetNHibernatePersistenceManager();

		//    item1.GetDetailCollection("theOtherItem", true).Add(item2);
		//    persister.Save(item1);

		//    IList<Details.LinkDetail> references = persister.RelationFinder.ListReferences(item2);
		//    Assert.AreEqual(references.Count, 1);
		//    Assert.AreEqual(references[0], item1.GetDetailCollection("theOtherItem", false).Details[0]);

		//    DeleteTheItems();
		//}

		//[Test]
		//public void FindReferencesInCollectionWithName()
		//{
		//    CreateTheThreeItems();

		//    item1.GetDetailCollection("theOtherItem", true).Add(root);
		//    persister.Save(item1);
		//    item2.GetDetailCollection("theOtherItem", true).Add(root);
		//    persister.Save(item2);

		//    IList<Details.LinkDetail> references = persister.RelationFinder
		//        .SetDetailName("theOtherItem")
		//        .ListReferences(root);
		//    Assert.AreEqual(2, references.Count);
		//    Assert.IsTrue(references[0] == item1.GetDetailCollection("theOtherItem", true).Details[0]
		//        && references[1] == item2.GetDetailCollection("theOtherItem", true).Details[0]
		//        || references[1] == item1.GetDetailCollection("theOtherItem", true).Details[0]
		//        && references[0] == item2.GetDetailCollection("theOtherItem", true).Details[0]);

		//    DeleteTheItems();
		//}
		//#endregion
	}
}
