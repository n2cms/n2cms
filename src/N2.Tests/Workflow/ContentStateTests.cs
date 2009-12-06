using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Persistence;
using N2.Definitions;
using N2.Edit;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;
using System.Web.UI;
using System.Security.Principal;
using N2.Details;
using N2.Workflow;
using N2.Workflow.Commands;
using N2.Security;
using Rhino.Mocks;
using N2.Tests.Fakes;
using N2.Configuration;
using N2.Tests.Workflow.Items;

namespace N2.Tests.Workflow
{
    [TestFixture]
    public class ContentStateTests : PersistenceAwareBase
    {
        IDefinitionManager definitions;
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
            definitions = engine.Resolve<IDefinitionManager>();
            editManager = engine.Resolve<IEditManager>();
            versionManager = engine.Resolve<IVersionManager>();

            editors = new Dictionary<string, Control>();
            editors["Title"] = new TextBox { Text = "New title" };
        }

        // definition manager

        [Test]
        public void CreateInstance_Generic_SetsItemState_New()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);

            Assert.That(item.State, Is.EqualTo(ContentState.New));
        }

        [Test]
        public void CreateInstance_TypeParameter_SetsItemState_New()
        {
            var item = definitions.CreateInstance(typeof(StatefulItem), null);

            Assert.That(item.State, Is.EqualTo(ContentState.New));
        }

        // edit manater

        [Test]
        public void VersionAndSave_SetsItemStateTo_Published()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            
            editManager.Save(item, editors, ItemEditorVersioningMode.VersionAndSave, admin);

            Assert.That(item.State, Is.EqualTo(ContentState.Published));
        }

        [Test]
        public void SaveOnly_SetsItemStateTo_Published()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);

            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);

            Assert.That(item.State, Is.EqualTo(ContentState.Published));
        }

        [Test]
        public void SaveOnly_OnVersion_SetsItemStateTo_Draft()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            persister.Save(item);
            var version = versionManager.SaveVersion(item);

            var result = editManager.Save(version, editors, ItemEditorVersioningMode.SaveOnly, admin);

            Assert.That(result.State, Is.EqualTo(ContentState.Draft));
        }

        [Test]
        public void SaveVersion_OnPublishedItem_SetsVersionedItemStateTo_Unpublished()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
            var version = versionManager.SaveVersion(item);

            Assert.That(version.State, Is.EqualTo(ContentState.Unpublished));
        }

        [Test]
        public void SaveVersion_OnDraft_SetsVersionedItemStateTo_Unpublished()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            persister.Save(item);
            var version = versionManager.SaveVersion(item);

            Assert.That(version.State, Is.EqualTo(ContentState.Draft));
        }

        [Test]
        public void SaveAsMaster_SetsItemState_Published()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            persister.Save(item);
            var version = versionManager.SaveVersion(item);

            var result = editManager.Save(version, editors, ItemEditorVersioningMode.SaveAsMaster, admin);

            Assert.That(result.State, Is.EqualTo(ContentState.Published));
        }

        [Test]
        public void VersionOnly_SetsVersionedItemStateTo_Draft()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            persister.Save(item);
            
            var result = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

            Assert.That(result.State, Is.EqualTo(ContentState.Draft));
        }

        [Test]
        public void VersionOnly_DoesntAffect_MasterVersionState()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
            var version = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

            Assert.That(item.State, Is.EqualTo(ContentState.Published));
        }

        [Test]
        public void VersionOnly_DoesntAffect_MasterVersionIndex()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
            int initialIndex = item.VersionIndex;
            var version = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

            Assert.That(item.VersionIndex, Is.EqualTo(initialIndex));
        }

        [Test]
        public void VersionOnly_SavedItem_IncrementsVersionIndex()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
            editors["Title"] = new TextBox { Text = "New title 2" };
            var version = editManager.Save(item, editors, ItemEditorVersioningMode.VersionOnly, admin);

            Assert.That(version.VersionIndex, Is.EqualTo(item.VersionIndex + 1));
        }

        [Test]
        public void Publish_DoesntAffect_OldVersionsIndex()
        {
            var item = definitions.CreateInstance<StatefulItem>(null);
            editManager.Save(item, editors, ItemEditorVersioningMode.SaveOnly, admin);
            int initialIndex = item.VersionIndex;
            editors["Title"] = new TextBox { Text = "New title 2" };
            var nextVersion = editManager.Save(item, editors, ItemEditorVersioningMode.VersionAndSave, admin);

            Assert.That(nextVersion.VersionIndex, Is.EqualTo(initialIndex + 1));
        }

    }
}
