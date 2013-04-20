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
		private readonly Engine.Logger<CommandDispatcher> logger;
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
			var args = new CommandProcessEventArgs { Command = command, Context = context };
			if (CommandExecuting != null)
				CommandExecuting.Invoke(this, args);

			logger.Info(args.Command.Name + " processing " + args.Context);
			using (var tx = persister.Repository.BeginTransaction())
			{
				try
				{
					args.Command.Process(args.Context);
					tx.Commit();

					if (CommandExecuted != null)
						CommandExecuted.Invoke(this, args);
				}
				catch (StopExecutionException)
				{
					tx.Rollback();
				}
				catch (Exception ex)
				{
					tx.Rollback();
					logger.Error(ex);
					throw;
				}
				finally
				{
					logger.Info(" -> " + args.Context);
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

		/// <summary>Invoked before a command is executed.</summary>
		public EventHandler<CommandProcessEventArgs> CommandExecuting;

		/// <summary>Invoked after a command was executed.</summary>
		public EventHandler<CommandProcessEventArgs> CommandExecuted;
    }
}
