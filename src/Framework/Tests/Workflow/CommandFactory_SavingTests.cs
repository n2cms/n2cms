using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Edit.Workflow.Commands;
using N2.Edit.Workflow;
using N2.Tests.Workflow.Items;

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
			var item = new StatefulItem();
			var context = new CommandContext(definitions.GetDefinition(item.GetContentType()), item, Interfaces.Editing, CreatePrincipal("admin"), nullBinder, nullValidator);

			var command = CreateCommand(context);
			dispatcher.Execute(command, context);

			Assert.That(item.Published, Is.Null);
		}
    }
}
