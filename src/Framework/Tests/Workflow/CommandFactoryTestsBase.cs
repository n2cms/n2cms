using System.Linq;
using N2.Configuration;
using N2.Definitions;
using N2.Edit;
using N2.Edit.Workflow;
using N2.Security;
using N2.Tests.Fakes;
using N2.Tests.Workflow.Items;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace N2.Tests.Workflow
{
    public abstract class CommandFactoryTestsBase : ItemPersistenceMockingBase
    {
        protected CommandFactory commands;
        protected CommandDispatcher dispatcher;
        protected IDefinitionManager definitions;
        protected FakeVersionManager versions;
        protected ContentItem item, child;
        protected IContentForm<CommandContext> nullBinder = new NullBinder<CommandContext>();
        protected IValidator<CommandContext> nullValidator = new NullValidator<CommandContext>();
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var changer = new StateChanger();
            definitions = TestSupport.SetupDefinitions(typeof(StatefulPage), typeof(StatefulPart));
            versions = new FakeVersionManager(repository, changer, typeof(StatefulPage), typeof(StatefulPart));
            var editManager = new EditUrlManager(null, new EditSection());
            var security = new SecurityManager(new FakeWebContextWrapper(), new EditSection());
            commands = new CommandFactory(persister, security, versions, editManager, null, changer);
            dispatcher = new CommandDispatcher(commands, persister);
            item = CreateOneItem<StatefulPage>(1, "first", null);
            child = CreateOneItem<StatefulPage>(2, "child", item);
        }

        protected abstract CommandBase<CommandContext> CreateCommand(CommandContext context);

        [TestCase(Interfaces.Editing, false)]
        [TestCase(Interfaces.Editing, true)]
        public void IsCheckedForSecurity(string userInterface, bool useVersion)
        {
            DynamicPermissionMap.SetRoles(item, Permission.Read, "None");
            if (useVersion)
                item = MakeVersion(item);
            var context = new CommandContext(definitions.GetDefinition(item.GetContentType()), item, userInterface, CreatePrincipal("someone"), new NullBinder<CommandContext>(), new NullValidator<CommandContext>());

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(context.ValidationErrors.Count, Is.EqualTo(1));
            Assert.That(context.ValidationErrors.First().Name, Is.EqualTo("Unauthorized"));
        }

        [TestCase(Interfaces.Editing, false)]
        [TestCase(Interfaces.Editing, true)]
        public void IsValidated_WhenInterface_IsEditing(string userInterface, bool useVersion)
        {
            if (useVersion)
                item = MakeVersion(item);

            var validator = mocks.Stub<IValidator<CommandContext>>();
            mocks.ReplayAll();

            var context = new CommandContext(definitions.GetDefinition(item.GetContentType()), item, userInterface, CreatePrincipal("admin"), nullBinder, validator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            validator.AssertWasCalled(b => b.Validate(context));
        }

        [Test]
        public void DoesntMakeVersion_OfUnsavedItem()
        {
            var context = new CommandContext(definitions.GetDefinition(typeof(StatefulPage)), new StatefulPage(), Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            versions.Repository.Repository.Count().ShouldBe(0);
        }

        [Test]
        public void PreviouslyPublishedVersion_CausesNewVersion()
        {
            var version = MakeVersion(item);
            version.State = ContentState.Unpublished;

            var context = new CommandContext(definitions.GetDefinition(version), version, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(versions.GetVersionsOf(item).Count(), Is.EqualTo(3));
        }

        [Test]
        public void CreatesVersion_OfVersionableItem()
        {
            var context = new CommandContext(definitions.GetDefinition(item.GetContentType()), item, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            dispatcher.Execute(CreateCommand(context), context);

            versions.Repository.Repository.Count().ShouldBeGreaterThan(0);
        }

        [Test]
        public void DoesntCreateVersion_OfNonVersionableItem()
        {
            var unversionable = new UnversionableStatefulItem();
            var context = new CommandContext(definitions.GetDefinition(unversionable.GetContentType()), unversionable, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);
            dispatcher.Execute(CreateCommand(context), context);

            dispatcher.Execute(CreateCommand(context), context);
            versions.Repository.Repository.Count().ShouldBe(0);
        }

        protected ContentItem MakeVersion(ContentItem master)
        {
            return versions.AddVersion(master, asPreviousVersion : true);
        }
    }
}
