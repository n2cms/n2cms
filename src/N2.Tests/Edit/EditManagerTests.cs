using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Details;
using N2.Edit;
using N2.Engine;
using N2.Definitions;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using Rhino.Mocks;
using N2.Persistence;
using N2.Tests.Edit.Items;
using System.Security.Principal;
using System.Web;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class EditManagerTests : TypeFindingBase
	{
		EditManager editManager;
		IPersister persister;
		IVersionManager versioner;

		protected override Type[] GetTypes()
		{
			return new Type[]{
				typeof(ComplexContainersItem),
				typeof(ItemWithRequiredProperty),
				typeof(ItemWithModification)
			};
		}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			EditableHierarchyBuilder<IEditable> hierarchyBuilder = new EditableHierarchyBuilder<IEditable>();
			DefinitionBuilder builder = new DefinitionBuilder(typeFinder, hierarchyBuilder, new AttributeExplorer<EditorModifierAttribute>(), new AttributeExplorer<IDisplayable>(), new AttributeExplorer<IEditable>(), new AttributeExplorer<IEditableContainer>());
			IItemNotifier notifier = mocks.DynamicMock<IItemNotifier>();
			mocks.Replay(notifier);
			DefinitionManager definitions = new DefinitionManager(builder, notifier);
			
			persister = mocks.StrictMock<IPersister>();
			versioner = mocks.StrictMock<IVersionManager>();
			editManager = new EditManager(typeFinder, definitions, persister, versioner, null, null, null);
			editManager.EnableVersioning = true;
		}
		
		[Test]
		public void CanAddEditors()
		{
			Type itemType = typeof(ComplexContainersItem);
			Control editorContainer = new Control();
			IDictionary<string, Control> added = editManager.AddEditors(itemType, editorContainer, null);
			Assert.AreEqual(5, added.Count);
			TypeAssert.Equals<TextBox>(added["MyProperty0"]);
			TypeAssert.Equals<TextBox>(added["MyProperty1"]);
			TypeAssert.Equals<TextBox>(added["MyProperty2"]);
			TypeAssert.Equals<FreeTextArea>(added["MyProperty3"]);
			TypeAssert.Equals<CheckBox>(added["MyProperty4"]);

			WebControlAssert.Contains(typeof(FieldSet), editorContainer);
			WebControlAssert.Contains(typeof(TextBox), editorContainer);
			WebControlAssert.Contains(typeof(FreeTextArea), editorContainer);
			WebControlAssert.Contains(typeof(CheckBox), editorContainer);
		}

		[Test]
		public void CanUpdateEditors()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			IDictionary<string, Control> added = AddEditors(item);

			TextBox tbp0 = added["MyProperty0"] as TextBox;
			TextBox tbp1 = added["MyProperty1"] as TextBox;
			TextBox tbp2 = added["MyProperty2"] as TextBox;
			FreeTextArea ftap3 = added["MyProperty3"] as FreeTextArea;
			CheckBox cbp4 = added["MyProperty4"] as CheckBox;

			Assert.IsEmpty(tbp0.Text);
			Assert.IsEmpty(tbp1.Text);
			Assert.IsEmpty(tbp2.Text);
			Assert.IsEmpty(ftap3.Text);
			Assert.IsFalse(cbp4.Checked);

			item.MyProperty0 = "one";
			item.MyProperty1 = "two";
			item.MyProperty2 = "three";
			item.MyProperty3 = "rock";
			item.MyProperty4 = true;

			editManager.UpdateEditors(item, added, null);

			Assert.AreEqual("one", tbp0.Text);
			Assert.AreEqual("two", tbp1.Text);
			Assert.AreEqual("three", tbp2.Text);
			Assert.AreEqual("rock", ftap3.Text);
			Assert.IsTrue(cbp4.Checked);
		}

		[Test]
		public void CanUpdateItem()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			IDictionary<string, Control> added = AddEditors(item);

			TextBox tbp0 = added["MyProperty0"] as TextBox;
			TextBox tbp1 = added["MyProperty1"] as TextBox;
			TextBox tbp2 = added["MyProperty2"] as TextBox;
			FreeTextArea ftap3 = added["MyProperty3"] as FreeTextArea;
			CheckBox cbp4 = added["MyProperty4"] as CheckBox;

			Assert.IsEmpty(item.MyProperty0);
			Assert.IsEmpty(item.MyProperty1);
			Assert.IsEmpty(item.MyProperty2);
			Assert.IsEmpty(item.MyProperty3);
			Assert.IsFalse(item.MyProperty4);

			tbp0.Text = "one";
			tbp1.Text = "two";
			tbp2.Text = "three";
			ftap3.Text = "rock";
			cbp4.Checked = true;

			editManager.UpdateItem(item, added, null);

			Assert.AreEqual("one", item.MyProperty0);
			Assert.AreEqual("two", item.MyProperty1);
			Assert.AreEqual("three", item.MyProperty2);
			Assert.AreEqual("rock", item.MyProperty3);
			Assert.IsTrue(item.MyProperty4);
		}

		private IDictionary<string, Control> AddEditors(ComplexContainersItem item)
		{
			Type itemType = item.GetType();
			Control editorContainer = new Control();
			IDictionary<string, Control> added = editManager.AddEditors(itemType, editorContainer, null);
			return added;
		}

		[Test]
		public void UpdateItem_WithChanges_ReturnsTrue()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			IDictionary<string, Control> added = AddEditors(item);

			TextBox tbp0 = added["MyProperty0"] as TextBox;
			TextBox tbp1 = added["MyProperty1"] as TextBox;
			TextBox tbp2 = added["MyProperty2"] as TextBox;
			FreeTextArea ftap3 = added["MyProperty3"] as FreeTextArea;
			CheckBox cbp4 = added["MyProperty4"] as CheckBox;

			tbp0.Text = "one";
			tbp1.Text = "two";
			tbp2.Text = "three";
			ftap3.Text = "rock";
			cbp4.Checked = true;

			bool result = editManager.UpdateItem(item, added, null);

			Assert.IsTrue(result, "UpdateItem didn't return true even though the editors were changed.");
		}

		[Test]
		public void UpdateItem_WithNoChanges_ReturnsFalse()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			Type itemType = item.GetType();
			Control editorContainer = new Control();
			IDictionary<string, Control> added = editManager.AddEditors(itemType, editorContainer, null);

			item.MyProperty0 = "one";
			item.MyProperty1 = "two";
			item.MyProperty2 = "three";
			item.MyProperty3 = "rock";
			item.MyProperty4 = true;
			editManager.UpdateEditors(item, added, null);

			bool result = editManager.UpdateItem(item, added, null);

			Assert.IsFalse(result, "UpdateItem didn't return false even though the editors were unchanged.");
		}

		[Test]
		public void CanSaveItem()
		{
			ComplexContainersItem item = new ComplexContainersItem();

			persister.Save(item);
			mocks.Replay(persister);

			IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.SaveOnly);
			DoTheSaving(null, editor);

			AssertItemHasValuesFromEditors(item);
		}

		[Test]
		public void DoesntSaveVersionForNewItems()
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
		public void CanSaveItemAndVersion()
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
		public void CanSaveVersionOnly()
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
		public void SavingWithLimitedPrincipal_DoesntChange_SecuredProperties()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			item.ID = 0;
			item.MyProperty1 = "I'm available";
			item.MyProperty5 = true;
			item.MyProperty6 = "I'm secure!";

			persister.Save(item);
			mocks.Replay(persister);

			IPrincipal user = CreateUser("Joe");

			Control editorContainer = new Control();
			IDictionary<string, Control> added = editManager.AddEditors(typeof(ComplexContainersItem), editorContainer, null);
			Assert.AreEqual(5, added.Count);

			IItemEditor editor = mocks.StrictMock<IItemEditor>();
			using (mocks.Record())
			{
				Expect.Call(editor.CurrentItem).Return(item).Repeat.Any();
				Expect.Call(editor.AddedEditors).Return(added);
				Expect.Call(editor.VersioningMode).Return(ItemEditorVersioningMode.VersionAndSave);
			}

			DoTheSaving(user, editor);

			Assert.AreEqual("", item.MyProperty0);
			Assert.AreEqual("I'm secure!", item.MyProperty6);
			Assert.IsTrue(item.MyProperty5);
		}


		[Test]
		public void Using_PrivilegedUser_AddsAllEditors()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			
			IPrincipal user = CreateUser("Joe", "ÜberEditor");

			Control editorContainer = new Control();
			IDictionary<string, Control> added = editManager.AddEditors(typeof(ComplexContainersItem), editorContainer, user);
			Assert.AreEqual(7, added.Count);
		}

		[Test]
		public void ValidatorsAreAddedToPage()
		{
			Page p = new Page();
			Assert.AreEqual(0, p.Validators.Count);

			editManager.AddEditors(typeof(ItemWithRequiredProperty), p, null);

			Assert.AreEqual(2, p.Validators.Count);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void UpdateEditors_PukesOnNullItem()
		{
			Dictionary<string, Control> editors = CreateEditorsForComplexContainersItem();
			editManager.UpdateEditors(null, editors, null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void UpdateEditors_PukesOnNullAddedEditors()
		{
			ContentItem item = new ComplexContainersItem();
			editManager.UpdateEditors(item, null, null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void UpdateItem_PukesOnNullItem()
		{
			Dictionary<string, Control> editors = CreateEditorsForComplexContainersItem();
			editManager.UpdateItem(null, editors, null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void UpdateItem_PukesOnNullAddedEditors()
		{
			ContentItem item = new ComplexContainersItem();
			editManager.UpdateItem(item, null, null);
		}

		[Test]
		public void AppliesModifications()
		{
			Control editorContainer = new Control();
			IDictionary<string, Control> added = editManager.AddEditors(typeof(ItemWithModification), editorContainer, null);
			
			ItemWithModification item = new ItemWithModification();
			editManager.UpdateEditors(item, added, null);

			TextBox tb = added["Essay"] as TextBox;

			Assert.AreEqual(10, tb.Rows);
			Assert.AreEqual(TextBoxMode.MultiLine, tb.TextMode);
		}

		[Test]
		public void AddingEditor_InvokesEvent()
		{
			Control editorContainer = new Control();
			editManager.AddedEditor += new EventHandler<N2.Web.UI.ControlEventArgs>(editManager_AddedEditor);
			IDictionary<string, Control> added = editManager.AddEditors(typeof(ComplexContainersItem), editorContainer, null);

			Assert.AreEqual(5, noticedByEvent.Count);
			EnumerableAssert.Contains(noticedByEvent, added["MyProperty0"]);
			EnumerableAssert.Contains(noticedByEvent, added["MyProperty0"]);
			EnumerableAssert.Contains(noticedByEvent, added["MyProperty0"]);
			EnumerableAssert.Contains(noticedByEvent, added["MyProperty0"]);
			EnumerableAssert.Contains(noticedByEvent, added["MyProperty0"]);
		}

		private List<Control> noticedByEvent = new List<Control>();
		void editManager_AddedEditor(object sender, N2.Web.UI.ControlEventArgs e)
		{
			noticedByEvent.Add(e.Control);
		}

		[Test]
		public void SavingVersionInvokesEvent()
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

		[Test]
		public void UpdateItem_WithNoChanges_IsNotSaved()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			item.ID = 22;
			item.MyProperty0 = "one";
			item.MyProperty1 = "two";
			item.MyProperty2 = "three";
			item.MyProperty3 = "rock";
			item.MyProperty4 = true;

			Expect.On(versioner).Call(versioner.SaveVersion(item)).Return(item.Clone(false));
			mocks.Replay(versioner);
			persister.Save(item);
			LastCall.Repeat.Never();
			mocks.Replay(persister);

			IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);

			DoTheSaving(null, editor);
		}

		[Test]
		public void NewItem_WithNoChanges_IsSaved()
		{
			ComplexContainersItem item = new ComplexContainersItem();
			item.ID = 0;
			item.MyProperty0 = "one";
			item.MyProperty1 = "two";
			item.MyProperty2 = "three";
			item.MyProperty3 = "rock";
			item.MyProperty4 = true;

			Expect.On(versioner).Call(versioner.SaveVersion(null)).Repeat.Never();
			mocks.Replay(versioner);
			persister.Save(item);
			LastCall.Repeat.Once();
			mocks.Replay(persister);

			IItemEditor editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);

			DoTheSaving(null, editor);
		}

		[Test]
		public void DisabledVersioningDoesntSaveVersion()
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
		public void GetEditUrl_OfPublishedRoot_UsesPath()
		{
			ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
			string editUrl = this.editManager.GetEditExistingItemUrl(root);

			Assert.AreEqual("~/edit/edit.aspx?selected=" + HttpUtility.UrlEncode("/"), editUrl);
		}

		[Test]
		public void GetEditUrl_OfPublishedSubPage_UsesPath()
		{
			ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
			ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);
			string editUrl = this.editManager.GetEditExistingItemUrl(item);

			Assert.AreEqual("~/edit/edit.aspx?selected=" + HttpUtility.UrlEncode("/child/"), editUrl);
		}

		[Test]
		public void GetEditUrl_OfUnpublishedVersion_RevertsToIdentity()
		{
			ContentItem root = CreateOneItem<ComplexContainersItem>(1, "root", null);
			ContentItem item = CreateOneItem<ComplexContainersItem>(2, "child", root);
			ContentItem versionOfItem = CreateOneItem<ComplexContainersItem>(3, "child", null);
			versionOfItem.VersionOf = item;

			string editUrl = this.editManager.GetEditExistingItemUrl(versionOfItem);

			Assert.AreEqual("~/edit/edit.aspx?selectedUrl=" + HttpUtility.UrlEncode("/default.aspx?page=3"), editUrl);
		}

		bool savingVersionEventInvoked = false;
		void editManager_SavingVersion(object sender, CancellableItemEventArgs e)
		{
			savingVersionEventInvoked = true;
		}

		private void DoTheSaving(IPrincipal user, IItemEditor editor)
		{
			using (mocks.Playback())
			{
				editManager.Save(editor, user);
			}
		}

		private IItemEditor SimulateEditor(ContentItem item, ItemEditorVersioningMode versioningMode)
		{
			IItemEditor editor = mocks.StrictMock<IItemEditor>();

			Dictionary<string, Control> editors = CreateEditorsForComplexContainersItem();

			using (mocks.Record())
			{
				Expect.On(editor).Call(editor.CurrentItem).Return(item).Repeat.Any();
				Expect.On(editor).Call(editor.AddedEditors).Return(editors);
				Expect.On(editor).Call(editor.VersioningMode).Return(versioningMode).Repeat.Any();
			}
			return editor;
		}

		private static Dictionary<string, Control> CreateEditorsForComplexContainersItem()
		{
			Dictionary<string, Control> editors = new Dictionary<string, Control>();

			editors["MyProperty0"] = new TextBox();
			editors["MyProperty1"] = new TextBox();
			editors["MyProperty2"] = new TextBox();
			editors["MyProperty3"] = new FreeTextArea();
			editors["MyProperty4"] = new CheckBox();
			
			((TextBox)editors["MyProperty0"]).Text = "one";
			((TextBox)editors["MyProperty1"]).Text = "two";
			((TextBox)editors["MyProperty2"]).Text = "three";
			((FreeTextArea)editors["MyProperty3"]).Text = "rock";
			((CheckBox)editors["MyProperty4"]).Checked = true;

			return editors;
		}

		private static void AssertItemHasValuesFromEditors(ComplexContainersItem item)
		{
			Assert.AreEqual("one", item.MyProperty0);
			Assert.AreEqual("two", item.MyProperty1);
			Assert.AreEqual("three", item.MyProperty2);
			Assert.AreEqual("rock", item.MyProperty3);
			Assert.IsTrue(item.MyProperty4);
		}
	}
}
