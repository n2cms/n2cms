using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Tests.Edit.Items;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Tests.Edit
{
    [TestFixture]
    public class WhileEditingContentData : EditManagerTests
    {
        [Test]
        public void AddedEditors_AreReturned()
        {
            Type itemType = typeof(ComplexContainersItem);
            Control editorContainer = new Control();
            IDictionary<string, Control> added = editManager.AddEditors(definitions.GetDefinition(itemType), new ComplexContainersItem(), editorContainer, CreatePrincipal("someone"));
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
            var item = new ComplexContainersItem();
            var added = AddEditors(item);

            var tbp0 = added["MyProperty0"] as TextBox;
            var tbp1 = added["MyProperty1"] as TextBox;
            var tbp2 = added["MyProperty2"] as TextBox;
            var ftap3 = added["MyProperty3"] as FreeTextArea;
            var cbp4 = added["MyProperty4"] as CheckBox;

            Assert.That(tbp0 != null);
            Assert.That(tbp1 != null);
            Assert.That(tbp2 != null);
            Assert.That(ftap3 != null);
            Assert.That(cbp4 != null);

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

            editManager.UpdateEditors(definitions.GetDefinition(item.GetContentType()), item, added, CreatePrincipal("someone"));

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

            var tbp0 = added["MyProperty0"] as TextBox;
            var tbp1 = added["MyProperty1"] as TextBox;
            var tbp2 = added["MyProperty2"] as TextBox;
            var ftap3 = added["MyProperty3"] as FreeTextArea;
            var cbp4 = added["MyProperty4"] as CheckBox;

            Assert.That(tbp0 != null);
            Assert.That(tbp1 != null);
            Assert.That(tbp2 != null);
            Assert.That(ftap3 != null);
            Assert.That(cbp4 != null);

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

            editManager.UpdateItem(definitions.GetDefinition(item.GetContentType()), item, added, CreatePrincipal("someone"));

            Assert.AreEqual("one", item.MyProperty0);
            Assert.AreEqual("two", item.MyProperty1);
            Assert.AreEqual("three", item.MyProperty2);
            Assert.AreEqual("rock", item.MyProperty3);
            Assert.IsTrue(item.MyProperty4);
        }

        [Test]
        public void UpdateItemWithChangesReturnsTrue()
        {
            var item = new ComplexContainersItem();
            var added = AddEditors(item);

            var tbp0 = added["MyProperty0"] as TextBox;
            var tbp1 = added["MyProperty1"] as TextBox;
            var tbp2 = added["MyProperty2"] as TextBox;
            var ftap3 = added["MyProperty3"] as FreeTextArea;
            var cbp4 = added["MyProperty4"] as CheckBox;

            Assert.That(tbp0 != null, "tbp0 != null");
            Assert.That(tbp1 != null, "tbp1 != null");
            Assert.That(tbp2 != null, "tbp2 != null");
            Assert.That(ftap3 != null, "ftap3 != null");
            Assert.That(cbp4 != null, "cbp4 != null");
            tbp0.Text = "one";
            tbp1.Text = "two";
            tbp2.Text = "three";
            ftap3.Text = "rock";
            cbp4.Checked = true;

            var result = editManager.UpdateItem(definitions.GetDefinition(item.GetContentType()), item, added, CreatePrincipal("someone"));

            Assert.That(result.Length, Is.GreaterThan(0), "UpdateItem didn't return true even though the editors were changed.");
        }

        [Test]
        public void UpdateItemWithNoChangesReturnsFalse()
        {
            var item = new ComplexContainersItem();
            var itemType = item.GetContentType();
            var editorContainer = new Control();
            var added = editManager.AddEditors(definitions.GetDefinition(itemType), item, editorContainer, CreatePrincipal("someone"));

            item.MyProperty0 = "one";
            item.MyProperty1 = "two";
            item.MyProperty2 = "three";
            item.MyProperty3 = "rock";
            item.MyProperty4 = true;
            editManager.UpdateEditors(definitions.GetDefinition(item.GetContentType()), item, added, CreatePrincipal("someone"));

            var result = editManager.UpdateItem(definitions.GetDefinition(item.GetContentType()), item, added, null);

            Assert.IsFalse(result.Length > 0, "UpdateItem didn't return false even though the editors were unchanged.");
        }

        [Test]
        public void CanSaveItem()
        {
            var item = new ComplexContainersItem();
            var editor = SimulateEditor(item, ItemEditorVersioningMode.SaveOnly);
            DoTheSaving(CreatePrincipal("someone"), editor);
            AssertItemHasValuesFromEditors(item);
        }

        [Test]
        public void NewItemWithNoChangesIsSaved()
        {
            var item = new ComplexContainersItem
            {
                ID = 0,
                MyProperty0 = "one",
                MyProperty1 = "two",
                MyProperty2 = "three",
                MyProperty3 = "rock",
                MyProperty4 = true
            };

            Expect.On(versioner).Call(versioner.AddVersion(null)).Repeat.Never();
            mocks.Replay(versioner);

            var editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);
            DoTheSaving(CreatePrincipal("someone"), editor);
            Assert.That(item.ID, Is.GreaterThan(0));
        }

        [Test]
        [Obsolete]
        public void SavingWithLimitedPrincipalDoesntChangeSecuredProperties()
        {
            var item = new ComplexContainersItem
            {
                ID = 0,
                MyProperty1 = "I'm available",
                MyProperty5 = true,
                MyProperty6 = "I'm secure!"
            };

            var user = CreatePrincipal("Joe");
            var editorContainer = new Control();
            var added = editManager.AddEditors(definitions.GetDefinition(typeof(ComplexContainersItem)), item, editorContainer, CreatePrincipal("someone"));
            Assert.AreEqual(5, added.Count);

            var editor = mocks.StrictMock<IItemEditor>();
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
        public void WillNotAddOrUpdateEditorInSecuredContainerWhenUntrustedUser()
        {
            var item = new ItemWithSecuredContainer {HiddenText = "No way"};
            var user = CreatePrincipal("Joe", "Editor");
            var editorContainer = new Control();
            var added = editManager.AddEditors(definitions.GetDefinition(typeof(ItemWithSecuredContainer)), item, editorContainer, user);
            editManager.UpdateEditors(definitions.GetDefinition(item.GetContentType()), item, added, user);
            Assert.That(added.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanUpdateEditorInSecuredContainerWhenUserIsTrusted()
        {
            var item = new ItemWithSecuredContainer {HiddenText = "Yes way"};
            var user = CreatePrincipal("Joe", "Administrators");
            var editorContainer = new Control();
            var added = editManager.AddEditors(definitions.GetDefinition(typeof(ItemWithSecuredContainer)), item, editorContainer, user);
            editManager.UpdateEditors(definitions.GetDefinition(item.GetContentType()), item, added, user);
            Assert.That(((TextBox)added["HiddenText"]).Text, Is.EqualTo("Yes way"));
        }

        [Test]
        public void UsingPrivilegedUserAddsAllEditors()
        {
            var item = new ComplexContainersItem();
            var user = CreatePrincipal("Joe", "ÜberEditor");
            var editorContainer = new Control();
            var added = editManager.AddEditors(definitions.GetDefinition(typeof(ComplexContainersItem)), item, editorContainer, user);
            Assert.AreEqual(7, added.Count);
        }

        [Test]
        public void ValidatorsAreAddedToPage()
        {
            var p = new Page();
            Assert.AreEqual(0, p.Validators.Count);
            editManager.AddEditors(definitions.GetDefinition(typeof(ItemWithRequiredProperty)), new ItemWithRequiredProperty(), p, CreatePrincipal("someone"));
            p.InitRecursive();
            Assert.AreEqual(2, p.Validators.Count);
        }

        [Test]
        public void UpdateEditorsPukesOnNullItem()
        {
            var editors = CreateEditorsForComplexContainersItem();
            Assert.Throws<ArgumentNullException>(() => editManager.UpdateEditors(null, null, editors, null));
        }

        [Test]
        public void UpdateEditorsPukesOnNullAddedEditors()
        {
            var item = new ComplexContainersItem();
            Assert.Throws<ArgumentNullException>(() => editManager.UpdateEditors(null, item, null, null));
        }

        [Test]
        public void UpdateItemPukesOnNullItem()
        {
            var editors = CreateEditorsForComplexContainersItem();
            Assert.Throws<ArgumentNullException>(() => editManager.UpdateItem(null, null, editors, null));
        }

        [Test]
        public void UpdateItemPukesOnNullAddedEditors()
        {
            var item = new ComplexContainersItem();
            Assert.Throws<ArgumentNullException>(() => editManager.UpdateItem(null, item, null, null));
        }

        [Test]
        public void AppliesModifications()
        {
            var editorContainer = new Control();
            var added = editManager.AddEditors(definitions.GetDefinition(typeof(ItemWithModification)), new ItemWithModification(), editorContainer, CreatePrincipal("someone"));
            var item = new ItemWithModification();
            editManager.UpdateEditors(definitions.GetDefinition(item.GetContentType()), item, added, CreatePrincipal("someone"));
            var tb = added["Essay"] as TextBox;
            Assert.That(tb != null);
            Assert.AreEqual(10, tb.Rows);
            Assert.AreEqual(TextBoxMode.MultiLine, tb.TextMode);
        }

        [Test]
        public void AddingEditorInvokesEvent()
        {
            var editorContainer = new Control();
            var noticedByEvent = new List<Control>();
            editManager.AddedEditor += (sender, e) => noticedByEvent.Add(e.Control);
            var added = editManager.AddEditors(definitions.GetDefinition(typeof(ComplexContainersItem)), new ComplexContainersItem(), editorContainer, CreatePrincipal("someone"));
            Assert.AreEqual(5, noticedByEvent.Count);
            EnumerableAssert.Contains(noticedByEvent, added["MyProperty0"]);
            EnumerableAssert.Contains(noticedByEvent, added["MyProperty1"]);
            EnumerableAssert.Contains(noticedByEvent, added["MyProperty2"]);
            EnumerableAssert.Contains(noticedByEvent, added["MyProperty3"]);
            EnumerableAssert.Contains(noticedByEvent, added["MyProperty4"]);
        }

        [Test]
        public void UpdateItemWithNoChangesIsNotSaved()
        {
            var item = new ComplexContainersItem
            {
                ID = 22,
                MyProperty0 = "one",
                MyProperty1 = "two",
                MyProperty2 = "three",
                MyProperty3 = "rock",
                MyProperty4 = true
            };

            Expect.On(versioner).Call(versioner.AddVersion(item)).Return(item.Clone(false));
            versioner.Expect(v => v.TrimVersionCountTo(item, 100)).IgnoreArguments().Repeat.Any();
            versioner.Expect(v => v.IsVersionable(item)).Return(true);
            mocks.Replay(versioner);

            var editor = SimulateEditor(item, ItemEditorVersioningMode.VersionAndSave);
            DoTheSaving(CreatePrincipal("someone"), editor);
            Assert.That(persister.Repository.Get(22), Is.Null);
        }
    }
}
