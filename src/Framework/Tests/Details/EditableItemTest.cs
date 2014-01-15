using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Definitions;
using N2.Details;
using N2.Web.Parts;
using N2.Web.UI.WebControls;
using NUnit.Framework;
using N2.Tests.Details.Models;
using N2.Definitions.Static;
using Shouldly;
using N2.Web.UI;
using N2.Persistence;
using N2.Persistence.Proxying;
using N2.Engine;
using N2.Edit;
using N2.Tests.Web.WebControls;
using Rhino.Mocks;
using N2.Plugin;
using N2.Edit.Settings;
using N2.Edit.Workflow;
using N2.Edit.Versioning;

namespace N2.Tests.Details
{
    [TestFixture]
    public class EditableItemTest
    {
        private Fakes.FakeEngine engine;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            TestSupport.InitializeHttpContext("/", "");

            N2.Context.Replace(engine = new Fakes.FakeEngine(typeof(EditableAdapter), typeof(DecoratedItem), typeof(DecoratedItem2), typeof(OtherItem)));
            engine.Initialize();
            engine.AddComponentInstance<IEditManager>(new EditManager(
                engine.Definitions,
                engine.Persister,
                MockRepository.GenerateStub<IVersionManager>(),
                new Fakes.FakeSecurityManager(),
                MockRepository.GenerateStub<IPluginFinder>(),
                new Fakes.FakeEditUrlManager(),
                new N2.Edit.Workflow.StateChanger(),
                new EditableHierarchyBuilder(new Fakes.FakeSecurityManager(), new N2.Configuration.EngineSection()),
                new N2.Configuration.EditSection()));
            engine.AddComponentInstance(new N2.Web.Slug(new N2.Configuration.ConfigurationManagerWrapper()));
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            N2.Context.Replace(null);
        }

        [Test]
        public void ItemEditor_AddsNewChild()
        {
            var item = new DecoratedItem();
            var definition = new DefinitionMap().GetOrCreateDefinition(item);
            var editable = (EditableItemAttribute)definition.Properties["TheItem"].Editable;

            var page = new Page();
            var enclosingEditor = new ItemEditor();
            var editor = AddEditorAndInit(item, editable, page, enclosingEditor);

            editable.UpdateEditor(item, editor);
            
            ItemUtility.FindInChildren<N2.Web.UI.WebControls.NameEditor>(editor).Single().Text = "Hello child";

            enclosingEditor.UpdateObject(new N2.Edit.Workflow.CommandContext(definition, item, Interfaces.Editing, engine.RequestContext.User));

            item.Children.Single().ShouldBe(item.TheItem);
        }

        [Test]
        public void NewChild_IsSaved()
        {
            var item = new DecoratedItem();
            var definition = new DefinitionMap().GetOrCreateDefinition(item);
            var editable = (EditableItemAttribute)definition.Properties["TheItem"].Editable;

            var page = new Page();
            var enclosingEditor = new ItemEditor();
            var editor = AddEditorAndInit(item, editable, page, enclosingEditor);

            editable.UpdateEditor(item, editor);

            var ctx = new N2.Edit.Workflow.CommandContext(definition, item, Interfaces.Editing, engine.RequestContext.User);
            enclosingEditor.UpdateObject(ctx);

            ctx.GetItemsToSave().ShouldContain(item.TheItem);
        }

        [Test]
        public void AssignedItem_IsAddedAsChild_WithPropertyNameAsItemName()
        {
            var item = (DecoratedItem)engine.Resolve<ContentActivator>().CreateInstance(typeof(DecoratedItem), null, null, asProxy: true);

            var child = new OtherItem();
            item.TheItem = child;

            item.TheItem.ShouldBe(child);
            item.Children.Single().ShouldBe(child);
            child.Name.ShouldBe("TheItem");
            child.ZoneName.ShouldBe(null);
        }

        [Test]
        public void AssignedItem_IsAssigned_DefaultChildName()
        {
            var item = (DecoratedItem2)engine.Resolve<ContentActivator>().CreateInstance(typeof(DecoratedItem2), null, null, asProxy: true);

            var child = new OtherItem();
            item.EditableItemWithDefaultChildName = child;

            item.EditableItemWithDefaultChildName.ShouldBe(child);
            item.Children.Single().ShouldBe(child);
            child.Name.ShouldBe("OtherChildName");
            child.ZoneName.ShouldBe(null);
        }

        [Test]
        public void AssignedItem_IsAssigned_DefaultZoneName()
        {
            var item = (DecoratedItem2)engine.Resolve<ContentActivator>().CreateInstance(typeof(DecoratedItem2), null, null, asProxy: true);

            var child = new OtherItem();
            item.EditableItemWithDefaultChildZoneName = child;

            item.EditableItemWithDefaultChildZoneName.ShouldBe(child);
            item.Children.Single().ShouldBe(child);
            child.Name.ShouldBe("EditableItemWithDefaultChildZoneName");
            child.ZoneName.ShouldBe("OtherZone");
        }

        [Test]
        public void AssignedItem_IsAssigned_DefaultZoneName_BeforeSaving_NonProxiedItem()
        {
            var item = new DecoratedItem2();
            var child = new OtherItem();
            item.EditableItemWithDefaultChildZoneName = child;

            this.engine.Resolve<IProxyFactory>().OnSaving(item);

            item.EditableItemWithDefaultChildZoneName.ShouldBe(child);
            item.Children.Single().ShouldBe(child);
            child.Name.ShouldBe("EditableItemWithDefaultChildZoneName");
            child.ZoneName.ShouldBe("OtherZone");
        }

        private static Control AddEditorAndInit(DecoratedItem item, EditableItemAttribute editable, Page page, ItemEditor enclosingEditor)
        {
            page.Controls.Add(enclosingEditor);
            enclosingEditor.Page = page;
            enclosingEditor.CurrentItem = item;
            var editor = editable.AddTo(enclosingEditor);
            page.InitRecursive();
            return editor;
        }
    }
}
