using System;
using N2.Persistence;
using NUnit.Framework;
using N2.Edit.Versioning;
using System.Linq;

namespace N2.Tests.Persistence
{
    [TestFixture, Category("Integration")]
    public class PersistFixture : DatabasePreparingBase
    {
        [Test]
        public void InsertRootNode()
        {
            Assert.IsNotNull(engine);
            Assert.IsNotNull(engine.Persister);

            Definitions.PersistableItem item = CreateRoot("root", "root item");
        }

        [Test]
        public void LoadRoot()
        {
            ContentItem item;
            using (engine.Persister)
            {
                item = CreateRoot("root", "root item");
            }
            using (engine.Persister)
            {
                ContentItem loadedItem = engine.Persister.Get(item.ID);
                Assert.AreEqual(item, loadedItem);
            }
        }

        [Test]
        public void StoreVariousTypesOfDetails()
        {
            Definitions.PersistableItem root;
            ContentItem item1;
            using (engine.Persister)
            {
                root = CreateRoot("root", "root item");
                item1 = CreateAndSaveItem("item1", "item one", root);

                root.BoolProperty = false;
                root.DateTimeProperty = new DateTime(1978, 12, 02);
                root.DoubleProperty = 3.1412;
                root.IntProperty = 42;
                root.LinkProperty = item1;
                root.ObjectProperty = new string[] { "one", "two", "three" };
                root.StringProperty = "dida";
                engine.Persister.Save(root);
            }

            using (engine.Persister)
            {
                root = (Definitions.PersistableItem)engine.Persister.Get(root.ID);
                item1 = (Definitions.PersistableItem)engine.Persister.Get(item1.ID);

                Assert.AreEqual(root.BoolProperty, false);
                Assert.AreEqual(root.DateTimeProperty, new DateTime(1978, 12, 02));
                Assert.AreEqual(root.DoubleProperty, 3.1412);
                Assert.AreEqual(root.IntProperty, 42);
                Assert.AreEqual(root.LinkProperty, item1);
                Assert.IsNotNull(root.ObjectProperty);
                Assert.AreEqual(((string[])root.ObjectProperty).Length, 3);
                Assert.AreEqual(((string[])root.ObjectProperty)[0], "one");
                Assert.AreEqual(root.StringProperty, "dida");
                Assert.AreEqual(root.Name, "root");
                Assert.AreEqual(root.Title, "root item");
            }
        }

        [Test]
        public void ModifyRoot()
        {
            ContentItem root;
            int rootItemID;
            DateTime referenceDate;

            using (engine.Persister)
            {
                root = CreateRoot("root", "root item");
                rootItemID = root.ID;
                referenceDate = N2.Utility.CurrentTime();
                root["DateDetail"] = referenceDate;
                root["StringDetail"] = "time 4 test";
                engine.Persister.Save(root);
            }

            using (engine.Persister)
            {
                root = engine.Persister.Get(rootItemID);
                Assert.AreEqual(root.Name, "root");
                Assert.AreEqual(root.Title, "root item");
                Assert.AreEqual(root["DateDetail"].ToString(), referenceDate.ToString());
                Assert.AreEqual(root["StringDetail"], "time 4 test");

                root.Name = "changed root";
                root.Title = "changed title";
                root["DateDetail"] = referenceDate.AddMinutes(1);
                root["StringDetail"] = "time 2 test";
                engine.Persister.Save(root);
            }

            using (engine.Persister)
            {
                root = engine.Persister.Get(rootItemID);
                Assert.AreEqual(root.Name, "changed root");
                Assert.AreEqual(root.Title, "changed title");
                Assert.AreEqual(root["DateDetail"].ToString(), referenceDate.AddMinutes(1).ToString());
                Assert.AreEqual(root["StringDetail"], "time 2 test");
            }
        }

        [Test]
        public void CreateChildren()
        {
            ContentItem root;

            using (engine.Persister)
            {
                root = CreateRoot("root", "root item");

                CreateAndSaveItem("item1", "item one", root);
                CreateAndSaveItem("item2", "item two", root);

                Assert.AreEqual(root.Children.Count, 2);
                Assert.IsNotNull(root.GetChild("item1"));
                Assert.IsNotNull(root.GetChild("item2"));
            }

            using (engine.Persister)
            {
                // check that everything is still there in the next session
                root = engine.Persister.Get(root.ID);
                Assert.AreEqual(root.Children.Count, 2);
                Assert.AreEqual(root.Children.Where(new N2.Collections.AccessFilter(null, engine.SecurityManager)).Count(), 2);
                Assert.IsNotNull(root.GetChild("item1"));
                Assert.IsNotNull(root.GetChild("item2"));
            }
        }

        [Test]
        public void Move()
        {
            using (engine.Persister)
            {
                ContentItem root = CreateRoot("root", "root item");

                ContentItem item1 = CreateAndSaveItem("item1", "item one", root);
                ContentItem item2 = CreateAndSaveItem("item2", "item two", root);

                engine.Persister.Move(item1, item2);

                Assert.AreEqual(1, root.Children.Count);
                Assert.AreEqual(0, item1.Children.Count);
                Assert.AreEqual(1, item2.Children.Count);

                Assert.AreEqual(item2, root.GetChild("item2"));
                Assert.AreEqual(item1, item2.GetChild("item1"));
            }
        }

        [Test]
        public void Copy()
        {
            ContentItem root, item1, item2;

            using (engine.Persister)
            {
                root = CreateRoot("root", "root item");

                item1 = CreateAndSaveItem("item1", "item one", root);
                item2 = CreateAndSaveItem("item2", "item two", root);

                engine.Persister.Copy(item1, item2);
            }

            using (engine.Persister)
            {
                root = engine.Persister.Get(root.ID);
                item1 = engine.Persister.Get(item1.ID);
                item2 = engine.Persister.Get(item2.ID);

                Assert.AreEqual(root.Children.Count, 2);
                Assert.AreEqual(item1.Children.Count, 0);
                Assert.AreEqual(item2.Children.Count, 1);

                ContentItem item1copy = item2.GetChild("item1");
                Assert.IsNotNull(item1copy);
                Assert.AreNotEqual(item1copy, item1);
            }
        }

        [Test]
        public void CopyWithChildren()
        {
            ContentItem root = CreateRoot("root", "root item");

            ContentItem item1 = CreateAndSaveItem("item1", "item one", root);
            ContentItem item2 = CreateAndSaveItem("item2", "item two", root);

            using (engine.Persister)
            {
                engine.Persister.Copy(item1, item2);
                engine.Persister.Copy(item2, item1);

                Assert.AreEqual(root.Children.Count, 2);
                Assert.AreEqual(item1.Children.Count, 1);
                Assert.AreEqual(item2.Children.Count, 1);

                ContentItem item1copy = item2.GetChild("item1");
                Assert.IsNotNull(item1copy);
                Assert.AreNotEqual(item1copy, item1);

                ContentItem item2copy = item1.GetChild("item2");
                Assert.IsNotNull(item2copy);
                Assert.AreNotEqual(item2copy, item2);

                ContentItem item1copyofcopy = item2copy.GetChild("item1");
                Assert.IsNotNull(item1copyofcopy);
                Assert.AreNotEqual(item1copyofcopy, item1copy);
            }
        }

        [Test]
        public void Delete()
        {
            ContentItem root = CreateRoot("root", "root item");
            Assert.AreEqual(root.Children.Count, 0);
            using (engine.Persister)
            {
                ContentItem item1 = CreateAndSaveItem("item1", "item one", root);
                Assert.AreEqual(root.Children.Count, 1);

                int item1ID = item1.ID;
                engine.Persister.Delete(item1);
                Assert.AreEqual(root.Children.Count, 0);

                item1 = engine.Persister.Get(item1ID);
                Assert.IsNull(item1);
            }
        }

        [Test]
        public void DeleteWithChildren()
        {
            ContentItem root = CreateRoot("root", "root item");
            using (engine.Persister)
            {
                ContentItem item1 = CreateAndSaveItem("item1", "item one", root);
                int item1ID = item1.ID;
                ContentItem item2 = CreateAndSaveItem("item2", "item two", item1);
                int item2ID = item2.ID;

                engine.Persister.Delete(item1);

                Assert.AreEqual(root.Children.Count, 0);
                item1 = engine.Persister.Get(item1ID);
                Assert.IsNull(item1);
                item2 = engine.Persister.Get(item2ID);
                Assert.IsNull(item2);
            }
        }

        [Test]
        public void SaveVersion()
        {
            ContentItem root = CreateRoot("root", "root item");
            using (engine.Persister)
            {
                ContentItem previousVersion = engine.Resolve<IVersionManager>().AddVersion(root);
                Assert.AreNotEqual(root, previousVersion);
                Assert.AreEqual(root, previousVersion.VersionOf.Value);
            }
        }

        [Test]
        public void SaveVersionDoesntClutterParentCollection()
        {
            ContentItem root = CreateRoot("root", "root item");
            using (engine.Persister)
            {
                ContentItem item1 = CreateAndSaveItem("item1", "item one", root);

                ContentItem previousVersion = engine.Resolve<IVersionManager>().AddVersion(item1);
                Assert.AreNotEqual(item1, previousVersion);
                Assert.AreEqual(item1, previousVersion.VersionOf.Value);

                Assert.AreEqual(root.Children.Count, 1);
            }
        }

        [Test]
        public void ReplaceVersion()
        {
            ContentItem root = CreateRoot("root", "root item");

            using (engine.Persister)
            {
                // Create an item with some values
                ContentItem item1 = CreateAndSaveItem("item1", "item one", root);
                DateTime d = N2.Utility.CurrentTime();
                item1["DateDetail"] = d;
                item1["StringDetail"] = "time 4 test";
                engine.Persister.Save(item1);

                // Store a version of that item
                ContentItem previousVersion = engine.Resolve<IVersionManager>().AddVersion(item1);

                // Change the initial item
                item1.Name = "item1.2";
                item1.Title = "item one changed once";
                item1["DateDetail"] = d.AddMinutes(1);
                item1["StringDetail"] = "nunit is the shit";
                engine.Persister.Save(item1);

                // Check that the previous version and the current item are different
                Assert.AreNotEqual(item1.Name, previousVersion.Name);
                Assert.AreNotEqual(item1.Title, previousVersion.Title);
                Assert.AreNotEqual(item1["DateDetail"], previousVersion["DateDetail"]);
                Assert.AreNotEqual(item1["StringDetail"], previousVersion["StringDetail"]);

                // Restore the previous version
                engine.Resolve<IVersionManager>().ReplaceVersion(item1, previousVersion, true);

                // Check that the values are unchanged
                Assert.AreEqual(item1.Name, previousVersion.Name);
                Assert.AreEqual(item1.Title, previousVersion.Title);
                Assert.AreEqual(item1["DateDetail"], previousVersion["DateDetail"]);
                Assert.AreEqual(item1["StringDetail"], previousVersion["StringDetail"]);
            }
        }

        [Test]
        public void RestoreVersionKeepsExpiresDate()
        {
            ContentItem root = CreateRoot("root", "root item");
            ContentItem previousVersion;
            using (engine.Persister)
            {
                root["VersionIndex"] = 1;
                previousVersion = engine.Resolve<IVersionManager>().AddVersion(root);
                root["VersionIndex"] = 2;
                engine.Persister.Save(root);
            }
            using (engine.Persister)
            {
                root = engine.Persister.Get(root.ID);
                previousVersion = engine.Resolve<IVersionManager>().GetVersion(root, previousVersion.VersionIndex);
                // engine.Persister.Get(previousVersion.ID);

                engine.Resolve<IVersionManager>().ReplaceVersion(root, previousVersion, true);
            }
            using (engine.Persister)
            {
                root = engine.Persister.Get(root.ID);
                Assert.IsNull(root.Expires, "Published wasn't null.");
            }
        }

        [Test]
        public void CloneItem()
        {
            // TODO: moveme
            Definitions.PersistableItem root = CreateRoot("root", "root item");
            root.BoolProperty       = false;
            root.DateTimeProperty   = new DateTime(1978, 12, 02);
            root.DoubleProperty     = 3.1412;
            root.IntProperty        = 42;
            root.LinkProperty       = root;
            root.ObjectProperty     = new string[] { "one", "two", "three" };
            root.StringProperty     = "dida";
            engine.Persister.Save(root);

            Definitions.PersistableItem clonedRoot = (Definitions.PersistableItem)root.Clone(true);
            Assert.AreNotEqual(root.ID, clonedRoot.ID);
            
            engine.Persister.Save(clonedRoot);
            Assert.AreEqual(root.BoolProperty, clonedRoot.BoolProperty);
            Assert.AreEqual(root.DateTimeProperty, clonedRoot.DateTimeProperty);
            Assert.AreEqual(root.DoubleProperty, clonedRoot.DoubleProperty);
            Assert.AreEqual(root.IntProperty, clonedRoot.IntProperty);
            Assert.AreEqual(root.LinkProperty, clonedRoot.LinkProperty);
            Assert.AreEqual(root.ObjectProperty, clonedRoot.ObjectProperty);
            Assert.AreEqual(root.StringProperty, clonedRoot.StringProperty);
            Assert.AreEqual(root.Name, clonedRoot.Name);
            Assert.AreEqual(root.Title, clonedRoot.Title);
            Assert.AreNotEqual(root.ID, clonedRoot.ID);

            foreach (N2.Details.ContentDetail detail in root.Details)
            {
                Assert.IsNotNull(clonedRoot[detail.Name]);
                Assert.AreNotSame(detail, clonedRoot[detail.Name]);
            }
        }

        [Test]
        public void OrphanedDetailIsReallyDeleted()
        {
            int id;
            using (engine.Persister)
            {
                ContentItem root = CreateRoot("root", "root item");
                root["DetailToBeOrphaned"] = "hello am I still here?";
                engine.Persister.Save(root);
                id = root.ID;
                Assert.AreEqual("hello am I still here?", root["DetailToBeOrphaned"]);
                root["DetailToBeOrphaned"] = null;
                engine.Persister.Save(root);
            }
            using (engine.Persister)
            {
                ContentItem root = engine.Persister.Get(id);
                Assert.IsNull(root["DetailToBeOrphaned"], "Was: " + root["DetailToBeOrphaned"]);
            }
        }

        [Test]
        public void RestoreVersionDoesntBreakRelations()
        {
            ContentItem root = CreateRoot("root", "root item");
            ContentItem item = CreateAndSaveItem("item", "item", root);
            ContentItem previousVersion;
            using (engine.Persister)
            {
                root["item"] = item;
                engine.Persister.Save(root);
            }
            using (engine.Persister)
            {
                item = engine.Persister.Get(item.ID);
                previousVersion = engine.Resolve<IVersionManager>().AddVersion(item);
                item.Name = "item2";
                engine.Persister.Save(item);
            }
            using (engine.Persister)
            {
                item = engine.Persister.Get(item.ID);
                root = engine.Persister.Get(root.ID);

                Assert.AreEqual("item2", item.Name);
                Assert.AreEqual(root["item"], item);
            }
            using (engine.Persister)
            {
                item = engine.Persister.Get(item.ID);
                root = engine.Persister.Get(root.ID);
                previousVersion = engine.Resolve<IVersionManager>().GetVersion(item, previousVersion.VersionIndex);

                engine.Resolve<IVersionManager>().ReplaceVersion(item, previousVersion, true);
            }

            using (engine.Persister)
            {
                item = engine.Persister.Get(item.ID);
                root = engine.Persister.Get(root.ID);

                Assert.AreEqual("item", item.Name);
                Assert.AreEqual(root["item"], item);
            }
        }
    }
}
