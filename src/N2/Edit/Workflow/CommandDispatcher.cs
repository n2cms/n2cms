using System;
using System.Diagnostics;
using N2.Engine;
using N2.Persistence;

namespace N2.Edit.Workflow
{
    /// <summary>
    /// Encapsulates the execution of commands used to modify the state of content items.
    /// </summary>
    [Service]
    public class CommandDispatcher
    {
		readonly ICommandFactory commandFactory;
		readonly IPersister persister;

		public CommandDispatcher(ICommandFactory commandFactory, IPersister persister)
		{
			this.commandFactory = commandFactory;
			this.persister = persister;
        }

        /// <summary>Executes the supplied command</summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="context">The context passed to the command</param>
        public virtual void Execute(CommandBase<CommandContext> command, CommandContext context)
        {
            Trace.Write(command.Name + " processing " + context);
			using (var tx = persister.Repository.BeginTransaction())
			{
				try
				{
					command.Process(context);
					tx.Commit();
				}
				catch (StopExecutionException)
				{
					tx.Rollback();
				}
				catch (Exception ex)
				{
					tx.Rollback();
					Trace.WriteLine(ex);
					throw;
				}
				finally
				{
					Trace.WriteLine(" -> " + context);
				}
			}
		}

        /// <summary>Publishes the data specified by the provided context.</summary>
        /// <param name="context">Contains data and information used to for publishing an item.</param>
        public virtual void Publish(CommandContext context)
        {
            Execute(commandFactory.GetPublishCommand(context), context);
        }

        /// <summary>Saves the data specified by the provided context.</summary>
        /// <param name="context">Contains data and information used to for saving an item.</param>
        public virtual void Save(CommandContext context)
        {
            Execute(commandFactory.GetSaveCommand(context), context);
        }
    }
}
