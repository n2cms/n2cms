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

			var pageVersions = versions.GetVersionsOf(page);
			pageVersions.Count.ShouldBeGreaterThan(0);
			pageVersions.First().State.ShouldBe(ContentState.Draft);
			pageVersions.First().Children.Single().Title.ShouldBe("New part");
		}
    }
}
