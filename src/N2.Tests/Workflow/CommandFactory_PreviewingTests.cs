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
    public class CommandFactory_PreviewingTests : CommandFactoryTestsBase
    {
        protected override CommandBase<CommandContext> CreateCommand(CommandContext context)
        {
            return commands.GetPreviewCommand(context);
        }


        [TestCase(Interfaces.Editing, false)]
        [TestCase(Interfaces.Editing, true)]
        public void RedirectsTo_PreviewUrl_OfNewVersion(string userInterface, bool ofVersion)
        {
            var version = ofVersion ? MakeVersion(item) : item;
            var context = new CommandContext(version, userInterface, CreatePrincipal("admin"), nullBinder, nullValidator);

            var command = CreateCommand(context);
            dispatcher.Execute(command, context);

            Assert.That(context.RedirectTo, Is.EqualTo(((INode)context.Data).PreviewUrl + "&preview=" + context.Data.ID + "&original=" + item.ID));
        }
    }
}
