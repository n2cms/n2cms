﻿using System.Linq;
using N2.Edit.Workflow;
using N2.Tests.Workflow.Items;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Edit;

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

			var validator = MockRepository.GenerateStub<IValidator<CommandContext>>();
            mocks.ReplayAll();

			var context = new CommandContext(definitions.GetDefinition(item.GetContentType()), item, Interfaces.Viewing, CreatePrincipal("admin"), nullBinder, validator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            validator.AssertWasNotCalled(b => b.Validate(context));
        }

        [Test]
        public void MakesVersion_OfCurrent()
        {
			var context = new CommandContext(definitions.GetDefinition(item.GetContentType()), item, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(repository.database.Values.Count(v => v.VersionOf.Value == item), Is.EqualTo(1));
        }

        [Test]
        public void Version_MakesVersion_OfCurrentMaster()
        {
            var version = MakeVersion(item);
			var context = new CommandContext(definitions.GetDefinition(version.GetContentType()), version, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(repository.database.Values.Count(v => v.VersionOf.Value == item), Is.EqualTo(1));
            Assert.That(item.Title, Is.EqualTo("version"));
        }

        [Test]
        public void Version_Replaces_MasterVersion()
        {
            var version = MakeVersion(item);
            version.Name = "tha masta";
			var context = new CommandContext(definitions.GetDefinition(version.GetContentType()), version, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(context.Content.Name, Is.EqualTo("tha masta"));
            Assert.That(context.Content.VersionOf.HasValue, Is.False);
        }

        [Test]
        public void PreviouslyPublishedVersion_CausesNewVersion_FromView()
        {
            var version = MakeVersion(item);
            version.State = ContentState.Unpublished;

			var context = new CommandContext(definitions.GetDefinition(version.GetContentType()), version, Interfaces.Viewing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(versions.GetVersionsOf(item).Count, Is.EqualTo(3));
        }

		[Test]
		public void CanMoveItem_ToBefore_Item()
		{
			var child2 = CreateOneItem<StatefulItem>(0, "child2", item);
			var context = new CommandContext(definitions.GetDefinition(child2.GetContentType()), child2, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);
			context.Parameters["MoveBefore"] = child.Path;
			var command = CreateCommand(context);
			dispatcher.Execute(command, context);

			Assert.That(item.Children.Count, Is.EqualTo(2));
			Assert.That(item.Children[0], Is.EqualTo(child2));
			Assert.That(item.Children[1], Is.EqualTo(child));
		}

		//What's the point, really?
		//[Test]
		//public void Sets_PublishedDate()
		//{
		//    var item = new StatefulItem();
		//    item.Published = null;
		//    var context = new CommandContext(item, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

		//    var command = CreateCommand(context);
		//    dispatcher.Execute(command, context);

		//    Assert.That(item.Published, Is.Not.Null);
		//    Assert.That(item.Published.Value, Is.GreaterThanOrEqualTo(DateTime.Now.AddSeconds(-10)));
		//}
    }
}
