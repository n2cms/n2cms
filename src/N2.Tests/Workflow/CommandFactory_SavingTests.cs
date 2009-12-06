using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Workflow.Commands;
using N2.Workflow;

namespace N2.Tests.Workflow
{
    [TestFixture]
    public class CommandFactory_SavingTests : CommandFactoryTestsBase
    {
        protected override CommandBase<CommandContext> CreateCommand(CommandContext context)
        {
            return commands.GetSaveCommand(context);
        }
    }
}
