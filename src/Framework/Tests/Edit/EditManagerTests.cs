using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Edit;
using N2.Persistence;
using N2.Persistence.Proxying;
using N2.Security;
using N2.Tests.Edit.Items;
using N2.Tests.Fakes;
using N2.Web;
using N2.Web.UI.WebControls;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Edit.Versioning;

namespace N2.Tests.Edit
{
    public abstract class EditManagerTests : TypeFindingBase
    {
        protected DefinitionManager definitions;
        protected EditManager editManager;
        protected IVersionManager versioner;

        protected override Type[] GetTypes()
        {
            return new[]
                    {
                        typeof (ComplexContainersItem),
                        typeof (ItemWithRequiredProperty),
                        typeof (ItemWithModification),
                        typeof (NotVersionableItem),
                        typeof (LegacyNotVersionableItem),
                        typeof (ItemWithSecuredContainer)
                    };
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DefinitionBuilder builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], TestSupport.SetupEngineSection());
            IItemNotifier notifier = mocks.DynamicMock<IItemNotifier>();
            mocks.Replay(notifier);
            var changer = new N2.Edit.Workflow.StateChanger();
            definitions = new DefinitionManager(new[] { new DefinitionProvider(builder) }, new ContentActivator(changer, notifier, new EmptyProxyFactory()), changer, new DefinitionMap());

            versioner = mocks.StrictMock<IVersionManager>();
            var urls = new FakeEditUrlManager();
            editManager = new EditManager(definitions, persister, versioner, null, null, urls, changer, new EditableHierarchyBuilder(new SecurityManager(new ThreadContext(), new EditSection()), TestSupport.SetupEngineSection()), new EditSection());
            editManager.EnableVersioning = true;

            var engine = new FakeEngine();
            engine.Container.AddComponentInstance("editManager", typeof(IEditManager), editManager);

            engine.Container.AddComponentInstance("editSection", typeof(EditSection), new EditSection());

            Context.Replace(engine);
        }

        protected IDictionary<string, Control> AddEditors(ComplexContainersItem item)
        {
            Type itemType = item.GetContentType();
            Control editorContainer = new Control();
            IDictionary<string, Control> added = editManager.AddEditors(definitions.GetDefinition(itemType), item, editorContainer, CreatePrincipal("someone"));
            return added;
        }

        protected bool savingVersionEventInvoked = false;
        protected void editManager_SavingVersion(object sender, CancellableItemEventArgs e)
        {
            savingVersionEventInvoked = true;
        }

        protected void DoTheSaving(IPrincipal user, IItemEditor editor)
        {
            using (mocks.Playback())
            {
                editManager.Save(editor.CurrentItem, editor.AddedEditors, editor.VersioningMode, user);
            }
        }

        protected IItemEditor SimulateEditor(ContentItem item, ItemEditorVersioningMode versioningMode)
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

        protected static Dictionary<string, Control> CreateEditorsForComplexContainersItem()
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

        protected static void AssertItemHasValuesFromEditors(ComplexContainersItem item)
        {
            Assert.AreEqual("one", item.MyProperty0);
            Assert.AreEqual("two", item.MyProperty1);
            Assert.AreEqual("three", item.MyProperty2);
            Assert.AreEqual("rock", item.MyProperty3);
            Assert.IsTrue(item.MyProperty4);
        }
    }
}
