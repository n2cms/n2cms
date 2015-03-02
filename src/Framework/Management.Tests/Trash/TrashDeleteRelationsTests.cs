using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Edit.Trash;
using N2.Edit.Tests.Trash;
using System.Security.Principal;
using Shouldly;
using N2.Persistence;
using N2.Edit.Versioning;

namespace N2.Management.Tests.Trash
{
    [TestFixture]
    public class TrashDeleteRelationsTests : N2.Tests.PersistenceAwareBase
    {
        private ITrashHandler trash;
        private ThrowableItem root, item, item2;
        private IPersister persister;
        private IVersionManager versions;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            persister = engine.Persister;
            versions = engine.Resolve<IVersionManager>();

            persister.Dispose();

            CreateDatabaseSchema();

            root = CreateOneItem<ThrowableItem>(0, "root", null);
            item = CreateOneItem<ThrowableItem>(0, "item", root);
            item2 = CreateOneItem<ThrowableItem>(0, "item2", root);

            trash = engine.Resolve<ITrashHandler>();

            System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin", "test"), new string[] { "Administrators" });
        }

        protected override T CreateOneItem<T>(int id, string name, ContentItem parent)
        {
            var item  = base.CreateOneItem<T>(id, name, parent);
            engine.Persister.Save(item);
            return item;
        }

        [Test]
        public void Trash_CanBePurged()
        {
            trash.Throw(item);
            trash.PurgeAll();
            persister.Dispose();

            engine.Persister.Get(item.ID).ShouldBe(null);
        }

        [Test]
        public void AncestralTrail_IsChangedTo_Trashcan()
        {
            trash.Throw(item);
            persister.Dispose();

            engine.Persister.Get(item.ID).AncestralTrail.ShouldBe((trash.TrashContainer as ContentItem).GetTrail());
        }

        [Test]
        public void AncestralTrail_OfDescendants_IsChanged()
        {
            var child = CreateOneItem<ThrowableItem>(0, "child", item);
            engine.Persister.Save(child);

            trash.Throw(item);

            persister.Dispose();

            engine.Persister.Get(child.ID).AncestralTrail.ShouldStartWith((trash.TrashContainer as ContentItem).GetTrail());
        }

        [Test]
        public void RelationTo_TrashedItem_IsRemoved()
        {
            item2["Relation"] = item;
            engine.Persister.Save(item2);
            
            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            trash.PurgeAll();

            persister.Dispose();
            item2 = persister.Get<ThrowableItem>(item2.ID);
            
            item2["Relation"].ShouldBe(null);
        }

        [Test]
        public void RelationTo_TrashedItem_IsRemoved_FromVersions()
        {
            item2["Relation"] = item;
            engine.Resolve<IVersionManager>().AddVersion(item2);
            engine.Persister.Save(item2);

            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            trash.PurgeAll();

            persister.Dispose();
            item2 = persister.Get<ThrowableItem>(item2.ID);
            
            var versions = engine.Resolve<IVersionManager>().GetVersionsOf(item2).ToList();
            versions[0].Content["Relation"].ShouldBe(null);
            versions[1].Content["Relation"].ShouldBe(null);
        }

        [Test]
        public void RelationTo_TrashedItemDescendant_IsRemoved()
        {
            var item1_1 = CreateOneItem<ThrowableItem>(0, "child", item);
            engine.Persister.Save(item1_1);

            item2["Relation"] = item;
            item2["Relation2"] = item1_1;
            persister.Save(item2);

            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            trash.PurgeAll();

            persister.Dispose();
            item2 = persister.Get<ThrowableItem>(item2.ID);
            
            item2["Relation"].ShouldBe(null);
            item2["Relation2"].ShouldBe(null);
        }

        [Test]
        public void Relation_InDetailCollection_IsRemoved()
        {
            // root
            //  item1 <- deleted
            //      item1_1
            //  item2 <- references item1 & item1_1

            var item1_1 = CreateOneItem<ThrowableItem>(0, "child", item);
            engine.Persister.Save(item1_1);

            var collection = item2.GetDetailCollection("Links", true);
            collection.Add(item);
            collection.Add(item1_1);
            engine.Persister.Save(item2);

            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            persister.Dispose();
            trash.PurgeAll();

            persister.Dispose();
            item2 = persister.Get<ThrowableItem>(item2.ID);
            
            item2.GetDetailCollection("Links", true).Count.ShouldBe(0);
        }

        [Test]
        public void Relation_InDetailCollection_OfVersion_IsRemoved()
        {
            var item1_1 = CreateOneItem<ThrowableItem>(0, "child", item);
            engine.Persister.Save(item1_1);

            var collection = item2.GetDetailCollection("Links", true);
            collection.Add(item);
            collection.Add(item1_1);
            engine.Persister.Save(item2);

            var version = versions.AddVersion(item2);

            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            trash.PurgeAll();

            persister.Dispose();
            item2 = persister.Get<ThrowableItem>(item2.ID);
			var repository = engine.Resolve<ContentVersionRepository>();
			version = repository.DeserializeVersion(repository.GetVersion(item2, version.VersionIndex)); 
            // persister.Get<ThrowableItem>(version.ID);
            
            item2.GetDetailCollection("Links", false).Count.ShouldBe(0);
            version.GetDetailCollection("Links", false).Count.ShouldBe(0);
        }

        [Test]
        public void Relations_WithinDeletedBranch_AreDeleted()
        {
            var item1_1 = CreateOneItem<ThrowableItem>(0, "child", item);
            item1_1["Relation"] = item;
            item1_1.GetDetailCollection("Links", true).Add(item);
            engine.Persister.Save(item1_1);

            item["Relation"] = item1_1;
            item.GetDetailCollection("Links", true).Add(item1_1);
            engine.Persister.Save(item);

            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            trash.PurgeAll();

            persister.Get(item.ID).ShouldBe(null);
            persister.Get(item1_1.ID).ShouldBe(null);
        }

        [Test]
        public void Multikulti()
        {
            var item1_1 = CreateOneItem<ThrowableItem>(0, "child", item);
            var item2_1 = CreateOneItem<ThrowableItem>(0, "child2", item2);

            Relate(item, item, item1_1, item2, item2_1);
            Relate(item2, item, item1_1, item2, item2_1);
            Relate(item1_1, item, item1_1, item2, item2_1);
            Relate(item2_1, item, item1_1, item2, item2_1);
            
            var version = versions.AddVersion(item2);

            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            trash.PurgeAll();

            persister.Dispose();

            persister.Get(item.ID).ShouldBe(null);
            persister.Get(item1_1.ID).ShouldBe(null);

            var db1 = persister.Get(item2.ID);
            var db2 = persister.Get(item2_1.ID);

            db1["Relation0"].ShouldBe(null);
            db1["Relation1"].ShouldBe(null);
            db1["Relation2"].ShouldBe(db1);
            db1["Relation3"].ShouldBe(db2);
            db1.GetDetailCollection("Links", false).Count.ShouldBe(2);
            db1.GetDetailCollection("Links", false).Contains(db1).ShouldBe(true);
            db1.GetDetailCollection("Links", false).Contains(db2).ShouldBe(true);
              
            db2["Relation0"].ShouldBe(null);
            db2["Relation1"].ShouldBe(null);
            db2["Relation2"].ShouldBe(db1);
            db2["Relation3"].ShouldBe(db2);
            db2.GetDetailCollection("Links", false).Count.ShouldBe(2);
            db2.GetDetailCollection("Links", false).Contains(db1).ShouldBe(true);
            db2.GetDetailCollection("Links", false).Contains(db2).ShouldBe(true);
        }

        [Test]
        public void Relations_InMultiValues_AreDeleted()
        {
            N2.Details.ContentDetail.Multi("Relation", true, 1, 1.1, N2.Utility.CurrentTime(), "hello", item, null).AddTo(item2);
            engine.Persister.Save(item2);

            persister.Dispose();
            item = persister.Get<ThrowableItem>(item.ID);
            
            trash.Throw(item);
            trash.PurgeAll();

            persister.Dispose();
            item2 = persister.Get<ThrowableItem>(item2.ID);
            
            item2.Details["Relation"].ShouldBe(null);
        }

        private void Relate(ThrowableItem item, params ThrowableItem[] to)
        {
            for (int i = 0; i < to.Length; i++)
            {
                item["Relation" + i] = to[i];
                item.GetDetailCollection("Links", true).Add(to[i]);
            }
            persister.Save(item);
        }
    }
}
