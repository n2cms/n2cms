using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Persistence;
using N2.Tests.Workflow.Items;
using N2.Web.UI.WebControls;
using NUnit.Framework;
using N2.Edit.Versioning;

namespace N2.Tests.Workflow
{
    [TestFixture]
    public class ContentStateTests : PersistenceAwareBase
    {
        ContentActivator activator;
        IEditManager editManager;
        IVersionManager versionManager;
        IPersister persister;

        Dictionary<string, Control> editors;
        IPrincipal admin = CreatePrincipal("admin");

        [SetUp]
        public override void SetUp()
        {
            CreateDatabaseSchema();
            base.SetUp();

            persister = engine.Resolve<IPersister>();
            activator = engine.Resolve<ContentActivator>();
            editManager = engine.Resolve<IEditManager>();
            versionManager = engine.Resolve<IVersionManager>();

            editors = new Dictionary<string, Control>();
            editors["Title"] = new TextBox { Text = "New title" };
        }

        // definition manager

        [Test]
        public void CreateInstance_Generic_SetsItemState_New()
        {
            var item = activator.CreateInstance<StatefulPage>(null);

            Assert.That(item.State, Is.EqualTo(ContentState.New));
        }

        [Test]
        public void CreateInstance_TypeParameter_SetsItemState_New()
        {
            var item = activator.CreateInstance(typeof(StatefulPage), null);

            Assert.That(item.State, Is.EqualTo(ContentState.New));
        }

        // edit manater

        [Test]
        [Obsolete]
        public void VersionAndSave_SetsItemStateTo_Published()
        {
            var item = activator.CreateInstance<StatefulPage>(null);
            
            editManager.Save(item, editors, ItemEditorVersioningMode.VersionAndSave, admin);

            Assert.That(item.State, Is.EqualTo(ContentState.Published));
        }

        [Test]
        [Obsolete]
        public void SaveOnly_SetsItemStateTo_Published()
        {
            var item = activator.CreateInstance<StatefulPage>(null);

            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);

            Assert.That(item.State, Is.EqualTo(ContentState.Published));
        }

        [Test]
        [Obsolete]
        public void SaveOnly_OnVersion_SetsItemStateTo_Draft()
        {
            var item = activator.CreateInstance<StatefulPage>(null);
            persister.Save(item);
            var version = versionManager.AddVersion(item);

            var result = editManager.Save(version, editors, ItemEditorVersioningMode.SaveOnly, admin);

            Assert.That(result.State, Is.EqualTo(ContentState.Draft));
        }

        [Test]
        [Obsolete]
        public void SaveVersion_OnPublishedItem_SetsVersionedItemStateTo_Unpublished()
        {
            var item = activator.CreateInstance<StatefulPage>(null);
            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
            var version = versionManager.AddVersion(item);

            Assert.That(version.State, Is.EqualTo(ContentState.Unpublished));
        }

        [Test]
        [Obsolete]
        public void SaveVersion_OnDraft_SetsVersionedItemStateTo_Unpublished()
        {
            var item = activator.CreateInstance<StatefulPage>(null);
            persister.Save(item);
            var version = versionManager.AddVersion(item);

            Assert.That(version.State, Is.EqualTo(ContentState.Draft));
        }

        [Test]
        [Obsolete]
        public void SaveAsMaster_SetsItemState_Published()
        {
            var item = activator.CreateInstance<StatefulPage>(null);
            persister.Save(item);
            var version = versionManager.AddVersion(item);

            var result = editManager.Save(version, editors, ItemEditorVersioningMode.SaveAsMaster, admin);

            Assert.That(result.State, Is.EqualTo(ContentState.Published));
        }

        [Test]
        [Obsolete]
        public void VersionOnly_SetsVersionedItemStateTo_Draft()
        {
            var item = activator.CreateInstance<StatefulPage>(null);
            persister.Save(item);
            
            var result = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

            Assert.That(result.State, Is.EqualTo(ContentState.Draft));
        }

        [Test]
        [Obsolete]
        public void VersionOnly_DoesntAffect_MasterVersionState()
        {
            var item = activator.CreateInstance<StatefulPage>(null);
            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
            var version = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

            Assert.That(item.State, Is.EqualTo(ContentState.Published));
        }

        //[Test]
        //[Obsolete]
        //public void VersionOnly_DoesntAffect_MasterVersionIndex()
        //{
        //    var item = activator.CreateInstance<StatefulItem>(null);
        //    editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
        //    int initialIndex = item.VersionIndex;
        //    var version = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

        //    Assert.That(item.VersionIndex, Is.EqualTo(initialIndex));
        //}

        //[Test]
        //[Obsolete]
        //public void VersionOnly_SavedItem_IncrementsVersionIndex()
        //{
        //    var item = activator.CreateInstance<StatefulItem>(null);
        //    editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
        //    editors["Title"] = new TextBox { Text = "New title 2" };
        //    var version = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

        //    Assert.That(version.VersionIndex, Is.EqualTo(item.VersionIndex + 1));
        //}

        //[Test]
        //[Obsolete]
        //public void Publish_DoesntAffect_OldVersionsIndex()
        //{
        //    var item = activator.CreateInstance<StatefulItem>(null);
        //    editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
        //    int initialIndex = item.VersionIndex;
        //    editors["Title"] = new TextBox { Text = "New title 2" };
        //    var nextVersion = editManager.Save(item, editors, ItemEditorVersioningMode.VersionAndSave, admin);

        //    Assert.That(nextVersion.VersionIndex, Is.EqualTo(initialIndex + 1));
        //}

    }
}
