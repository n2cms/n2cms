using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Workflow;
using N2.Security;
using N2.Edit;
using Rhino.Mocks;
using N2.Workflow.Commands;

namespace N2.Tests.Workflow
{
    [TestFixture]
    public class CommandFactory_PublishingTests : CommandFactoryTestsBase
    {

        protected override CommandBase<CommandContext> CreateCommand(CommandContext context)
        {
            return commands.GetPublishCommand(context);
        }

        [Test]
        public void IsNotValidated_WhenInterface_IsViewing()
        {
            item = MakeVersion(item);

            var validator = MockRepository.GenerateStub<IValidator<ContentItem>>();
            mocks.ReplayAll();

            var context = new CommandContext(item, Interfaces.Viewing, CreatePrincipal("admin"), nullBinder, validator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            validator.AssertWasNotCalled(b => b.Validate(item));
        }

        [Test]
        public void MakesVersion_OfCurrent()
        {
            var context = new CommandContext(item, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(repository.database.Values.Count(v => v.VersionOf == item), Is.EqualTo(1));
        }

        [Test]
        public void Version_MakesVersion_OfCurrentMaster()
        {
            var version = MakeVersion(item);
            var context = new CommandContext(version, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(repository.database.Values.Count(v => v.VersionOf == item), Is.EqualTo(1));
            Assert.That(item.Title, Is.EqualTo("version"));
        }

        [Test]
        public void Version_Replaces_MasterVersion()
        {
            var version = MakeVersion(item);
            version.Name = "tha masta";
            var context = new CommandContext(version, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(context.Data.Name, Is.EqualTo("tha masta"));
            Assert.That(context.Data.VersionOf, Is.Null);
        }

        [TestCase(Interfaces.Viewing, true)]
        [TestCase(Interfaces.Editing, false)]
        [TestCase(Interfaces.Editing, true)]
        public void RedirectsTo_PreviewUrl(string userInterface, bool useVersion)
        {
            var version = useVersion ? MakeVersion(item) : item;
            var context = new CommandContext(version, userInterface, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(context.RedirectTo, Is.EqualTo(((INode)item).PreviewUrl));
        }

        [Test]
        public void PreviouslyPublishedVersion_CausesNewVersion_FromView()
        {
            var version = MakeVersion(item);
            version.State = ContentState.Unpublished;

            var context = new CommandContext(version, Interfaces.Viewing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(versions.GetVersionsOf(item).Count, Is.EqualTo(3));
        }
    }
}
