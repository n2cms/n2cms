using N2.Edit.Workflow;
using N2.Tests.Workflow.Items;
using NUnit.Framework;
using N2.Edit;
using Shouldly;
using System.Linq;

namespace N2.Tests.Workflow
{
    [TestFixture]
    public class CommandFactory_SavingTests : CommandFactoryTestsBase
    {
        protected override CommandBase<CommandContext> CreateCommand(CommandContext context)
        {
            return commands.GetSaveCommand(context);
        }

        [Test]
        public void Clears_PublishedDate()
        {
            var item = new StatefulPage();
            var context = new CommandContext(definitions.GetDefinition(item.GetContentType()), item, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(item.Published, Is.Null);
        }

        [Test]
        public void UnsavedPart_IsSavedOnNewPageVersion()
        {
            var page = new StatefulPage();
            page.Title = "The page";
            persister.Save(page);
            
            var part = new StatefulPart();
            part.Title = "New part";
            part.Parent = page;
            part.ZoneName = "TheZone";

            var context = new CommandContext(definitions.GetDefinition(page.GetContentType()), part, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            var pageVersions = versions.GetVersionsOf(page).ToList();
            pageVersions.Count.ShouldBeGreaterThan(0);
            pageVersions.First().State.ShouldBe(ContentState.Draft);
            pageVersions.First().Content.Children.Single().Title.ShouldBe("New part");
        }

        [Test]
        public void UnsavedPart_IsAppendedLast_ToParent()
        {
            var page = CreatePageWithPart();

            var part2 = new StatefulPart();
            part2.Title = "New part 2";
            part2.Parent = page;
            part2.ZoneName = "TheZone";
            var context = new CommandContext(definitions.GetDefinition(page.GetContentType()), part2, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);
            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            var pageVersion = versions.GetVersion(page, context.Content.VersionIndex);
            var partVersion = pageVersion.Children[0];
            var part2Version = pageVersion.Children[1];
            partVersion.SortOrder.ShouldBeLessThan(part2Version.SortOrder);
        }

        [Test]
        public void UnsavedPart_CanBeInserted_BeforeSortOrder()
        {
            var page = CreatePageWithPart();

            var part2 = new StatefulPart();
            part2.Title = "New part";
            part2.Name = "NewPart";
            part2.Parent = page;
            part2.ZoneName = "TheZone";
            var context = new CommandContext(definitions.GetDefinition(page.GetContentType()), part2, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);
            context.Parameters["MoveBeforeSortOrder"] = "0";
            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            var pageVersion = versions.GetVersion(page, context.Content.VersionIndex);
            var partVersion = pageVersion.Children["ThePart"];
            var part2Version = pageVersion.Children["NewPart"];
            part2Version.SortOrder.ShouldBeLessThan(partVersion.SortOrder);
        }

        [Test]
        public void UnsavedPart_CanBeInserted_BeforeVersionKey()
        {
            var page = CreatePageWithPart();

            var part2 = new StatefulPart();
            part2.Title = "New part";
            part2.Name = "NewPart";
            part2.Parent = page;
            part2.ZoneName = "TheZone";
            var context = new CommandContext(definitions.GetDefinition(page.GetContentType()), part2, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);
            context.Parameters["MoveBeforeSortOrder"] = "0";
            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            var pageVersion = versions.GetVersion(page, context.Content.VersionIndex);
            var partVersion = pageVersion.Children["ThePart"];
            var part2Version = pageVersion.Children["NewPart"];
            part2Version.SortOrder.ShouldBeLessThan(partVersion.SortOrder);
        }

        private StatefulPage CreatePageWithPart()
        {
            var page = new StatefulPage();
            page.Title = "The page";
            persister.Save(page);

            var part = new StatefulPart();
            part.Title = "The part";
            part.Name = "ThePart";
            part.Parent = page;
            part.ZoneName = "TheZone";
            persister.Save(part);
            return page;
        }

        private CommandContext ExecuteSave(StatefulPage page, StatefulPart part)
        {
            var context = new CommandContext(definitions.GetDefinition(page.GetContentType()), part, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);
            var command = CreateCommand(context);
            dispatcher.Execute(command, context);
            return context;
        }
    }
}
