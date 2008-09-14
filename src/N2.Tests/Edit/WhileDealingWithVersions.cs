using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Tests.Edit.Items;
using N2.Web.UI.WebControls;

namespace N2.Tests.Edit
{
    [TestFixture]
    public class WhileDealingWithVersions : EditManagerTests
    {
        [Test]
        public void DoesntSaveVersion_ForNewItems()
        {
            ComplexContainersItem item = new ComplexContainersItem();
            item.ID = 0;

            Expect.On(versioner).Call(versioner.SaveVersion(item)).Repeat.Never();
            persister.Save(item);
            mocks.Replay(persister);

            IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);
            DoTheSaving(null, editor);

            AssertItemHasValuesFromEditors(item);
        }

        [Test]
        public void CanSaveVersion()
        {
            ComplexContainersItem item = new ComplexContainersItem();
            item.ID = 28;

            Expect.On(versioner).Call(versioner.SaveVersion(item)).Return(item.Clone(false));
            mocks.Replay(versioner);
            persister.Save(item);
            mocks.Replay(persister);

            IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);
            DoTheSaving(null, editor);

            AssertItemHasValuesFromEditors(item);
        }

        [Test]
        public void CanSave_ItemAndVersion()
        {
            ComplexContainersItem item = new ComplexContainersItem();
            item.ID = 28;

            Expect.On(versioner).Call(versioner.SaveVersion(item)).Return(item.Clone(false));
            mocks.Replay(versioner);
            persister.Save(item);
            mocks.Replay(persister);

            IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);
            DoTheSaving(null, editor);

            AssertItemHasValuesFromEditors(item);
        }

        [Test]
        public void CanSave_VersionOnly()
        {
            ComplexContainersItem item = new ComplexContainersItem();
            item.ID = 28;
            ComplexContainersItem version = item.Clone(false) as ComplexContainersItem;

            Expect.On(versioner).Call(versioner.SaveVersion(item)).Return(version);
            mocks.Replay(versioner);
            persister.Save(item);
            LastCall.Repeat.Never();
            persister.Save(version);
            LastCall.Repeat.Once();
            mocks.Replay(persister);

            IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionOnly);
            DoTheSaving(null, editor);

            Assert.AreEqual("", item.MyProperty0);
            Assert.AreEqual("", item.MyProperty1);
            Assert.AreEqual("", item.MyProperty2);
            Assert.AreEqual("", item.MyProperty3);
            Assert.IsFalse(item.MyProperty4);

            AssertItemHasValuesFromEditors(version);
        }

        [Test]
        public void DisabledVersioning_DoesntSaveVersion()
        {
            editManager.EnableVersioning = false;

            ComplexContainersItem item = new ComplexContainersItem();
            item.ID = 29;

            Expect.On(versioner).Call(versioner.SaveVersion(item)).Repeat.Never();
            mocks.Replay(versioner);
            persister.Save(item);
            mocks.Replay(persister);

            IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);

            DoTheSaving(null, editor);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveVersionAsMaster_IsMadeMasterVersion()
        {
            ComplexContainersItem currentMaster = new ComplexContainersItem(1, "current master");

            ComplexContainersItem versionToBeMaster = new ComplexContainersItem(29, "version of current");
            versionToBeMaster.VersionOf = currentMaster;

            Expect.Call(versioner.SaveVersion(currentMaster)).Return(null);
            Expect.Call(delegate { persister.Save(currentMaster); });

            mocks.ReplayAll();

            IItemEditor editor = SimulateEditor(versionToBeMaster, ItemEditorVersioningMode.SaveAsMaster);
            DoTheSaving(null, editor);
        }

        [Test]
        public void VersionOnly_WhenNewItem_SavesIt_ButUnpublishesIt()
        {
            ComplexContainersItem newItem = new ComplexContainersItem(0, "an item");
            newItem.Published = DateTime.Now;

            Expect.Call(delegate { persister.Save(newItem); });
            mocks.ReplayAll();


            IItemEditor editor = SimulateEditor(newItem, ItemEditorVersioningMode.VersionOnly);
            DoTheSaving(null, editor);

            Assert.IsNull(newItem.Published);
        }

        [Test]
        public void Save_WhenUnpublished_PublishesItem()
        {
            ComplexContainersItem newItem = new ComplexContainersItem(1, "an item");
            newItem.Published = null;

            Expect.Call(versioner.SaveVersion(newItem)).Return(new ComplexContainersItem(2, "ignored"));
            Expect.Call(delegate { persister.Save(newItem); });
            mocks.ReplayAll();

            IItemEditor editor = SimulateEditor(newItem, ItemEditorVersioningMode.VersionAndSave);
            DoTheSaving(null, editor);

            Assert.IsNotNull(newItem.Published, "Unpublished item should have been published.");
            Assert.Greater(newItem.Published, DateTime.Now.AddSeconds(-10));
        }

        [Test]
        public void SaveVersionAsMaster_WhenMasterIsUnpublished_PublishesItem()
        {
            ComplexContainersItem currentMaster = new ComplexContainersItem(1, "current master");
            currentMaster.Published = null;

            ComplexContainersItem versionToBeMaster = new ComplexContainersItem(29, "version of current");
            versionToBeMaster.VersionOf = currentMaster;
            versionToBeMaster.Published = null;

            Expect.Call(versioner.SaveVersion(currentMaster)).Return(null);
            Expect.Call(delegate { persister.Save(currentMaster); });

            mocks.ReplayAll();

            IItemEditor editor = SimulateEditor(versionToBeMaster, ItemEditorVersioningMode.SaveAsMaster);
            DoTheSaving(null, editor);

            Assert.IsNotNull(currentMaster.Published);
            Assert.Greater(currentMaster.Published, DateTime.Now.AddSeconds(-10));
        }

        [Test]
        public void SavingVersion_InvokesEvent()
        {
            savingVersionEventInvoked = false;
            ComplexContainersItem item = new ComplexContainersItem();
            item.ID = 29;

            Expect.On(versioner).Call(versioner.SaveVersion(item)).Return(item.Clone(false));
            mocks.Replay(versioner);
            persister.Save(item);
            mocks.Replay(persister);

            editManager.SavingVersion += new EventHandler<CancellableItemEventArgs>(editManager_SavingVersion);
            IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);

            DoTheSaving(null, editor);

            Assert.IsTrue(savingVersionEventInvoked, "The saving version event wasn't invoked");
        }

        [Test]
        public void SavingItem_ThatIsNotVersionable_DoesntStoreVersion()
        {
            NotVersionableItem item = new NotVersionableItem();
            item.ID = 123;

            Expect.Call(versioner.SaveVersion(item)).Repeat.Never();
            mocks.ReplayAll();

            var editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);
            DoTheSaving(null, editor);

            mocks.VerifyAll();
            //versioner.AssertWasNotCalled(delegate(VersionManager vm) { vm.SaveVersion(item); });
        }

        [Test]
        public void SavingVersionEvent_IsNotInvoked_WhenNewItem()
        {
            savingVersionEventInvoked = false;
            ComplexContainersItem item = new ComplexContainersItem();
            item.ID = 0;

            persister.Save(item);
            mocks.Replay(persister);

            editManager.SavingVersion += editManager_SavingVersion;
            IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);

            DoTheSaving(null, editor);

            Assert.IsFalse(savingVersionEventInvoked, "The saving version event should not have been invoked.");
        }
    }
}
