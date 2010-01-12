using System;
using System.Diagnostics;
using N2.Engine;

namespace N2.Workflow
{
    /// <summary>
    /// Encapsulates the execution of commands used to modify the state of content items.
    /// </summary>
    [Service]
    public class CommandDispatcher
    {
        ICommandFactory commandFactory;

        public CommandDispatcher(ICommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        /// <summary>Executes the supplied command</summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="context">The context passed to the command</param>
        public virtual void Execute(CommandBase<CommandContext> command, CommandContext context)
        {
            Trace.Write(command.Name + " processing " + context);
            try
            {
                command.Process(context);
            }
            catch (StopExecutionException)
            {
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
            finally
            {
                Trace.WriteLine(" -> " + context);
            }
        }

        /// <summary>Publishes the data specified by the provided context.</summary>
        /// <param name="context">Contains data and information used to for publishing an item.</param>
        public virtual void Publish(CommandContext context)
        {
            Execute(commandFactory.GetPublishCommand(context), context);
        }

        /// <summary>Saves and previews the data specified by the provided context.</summary>
        /// <param name="context">Contains data and information used to for saving and previewing an item.</param>
        public virtual void Preview(CommandContext context)
        {
            Execute(commandFactory.GetPreviewCommand(context), context);
        }

        /// <summary>Saves the data specified by the provided context.</summary>
        /// <param name="context">Contains data and information used to for saving an item.</param>
        public virtual void Save(CommandContext context)
        {
            Execute(commandFactory.GetSaveCommand(context), context);
        }
    }
}
