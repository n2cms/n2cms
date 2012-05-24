using System;
using N2.Edit.Workflow.Commands;
using N2.Engine;
using N2.Persistence;
using N2.Security;

namespace N2.Edit.Workflow
{
    /// <summary>
    /// Provides and executies commands used to change the state of content items.
    /// </summary>
    [Service(typeof(ICommandFactory))]
    public class CommandFactory : ICommandFactory
    {
        IPersister persister;
        CommandBase<CommandContext> makeVersionOfMaster;
        ReplaceMasterCommand replaceMaster;
        MakeVersionCommand makeVersion;
        UseNewVersionCommand useNewVersion;
        UpdateObjectCommand updateObject;
        DeleteCommand delete;
        RedirectToPreviewCommand showPreview;
        RedirectToEditCommand showEdit;
        UseMasterCommand useMaster;
        CloneCommand clone;
        ValidateCommand validate;
        SaveCommand save;
        IncrementVersionIndexCommand incrementVersionIndex;
        UpdateContentStateCommand draftState;
        UpdateContentStateCommand publishedState;
        ActiveContentSaveCommand saveActiveContent;
		MoveToPositionCommand moveToPosition;
		EnsureNotPublishedCommand unpublishedDate;
		EnsurePublishedCommand publishedDate;
		UpdateReferencesCommand updateReferences;
		ISecurityManager security;

        public CommandFactory(IPersister persister, ISecurityManager security, IVersionManager versionMaker, IEditUrlManager editUrlManager, IContentAdapterProvider adapters, StateChanger changer)
        {
            this.persister = persister;
            makeVersionOfMaster = On.Master(new MakeVersionCommand(versionMaker));
            replaceMaster = new ReplaceMasterCommand(versionMaker);
            makeVersion = new MakeVersionCommand(versionMaker);
            useNewVersion = new UseNewVersionCommand(versionMaker);
            updateObject = new UpdateObjectCommand();
            delete = new DeleteCommand(persister.Repository);
            showPreview = new RedirectToPreviewCommand(adapters);
            showEdit = new RedirectToEditCommand(editUrlManager);
            useMaster = new UseMasterCommand();
            clone = new CloneCommand();
            validate = new ValidateCommand();
            this.security = security;
            save = new SaveCommand(persister);
            incrementVersionIndex = new IncrementVersionIndexCommand(versionMaker);
            draftState = new UpdateContentStateCommand(changer, ContentState.Draft);
            publishedState = new UpdateContentStateCommand(changer, ContentState.Published);
            saveActiveContent = new ActiveContentSaveCommand();
			moveToPosition = new MoveToPositionCommand();
			unpublishedDate = new EnsureNotPublishedCommand();
			publishedDate = new EnsurePublishedCommand();
			updateReferences = new UpdateReferencesCommand();
        }

        /// <summary>Gets the command used to publish an item.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will publish an item.</returns>
		public virtual CompositeCommand GetPublishCommand(CommandContext context)
        {
            if (context.Interface == Interfaces.Editing)
            {
                if (context.Content is IActiveContent)
					return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, moveToPosition, saveActiveContent, updateReferences);
                
                // Editing
				if (!context.Content.VersionOf.HasValue)
				{
					if(context.Content.ID == 0)
						return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, incrementVersionIndex, publishedState, moveToPosition/*, publishedDate*/, save, updateReferences);

					if (context.Content.State == ContentState.Draft && context.Content.Published.HasValue == false)
						return Compose("Publish", Authorize(Permission.Publish), validate, makeVersion, updateObject, incrementVersionIndex, publishedState, moveToPosition, publishedDate, save, updateReferences);

					return Compose("Publish", Authorize(Permission.Publish), validate, makeVersion, updateObject, incrementVersionIndex, publishedState, moveToPosition/*, publishedDate*/, save, updateReferences);
				}

				// has been published before
				if (context.Content.State == ContentState.Unpublished)
					return Compose("Publish", Authorize(Permission.Publish), validate, updateObject,/* makeVersionOfMaster,*/ replaceMaster, useMaster, incrementVersionIndex, publishedState, moveToPosition/*, publishedDate*/, save, updateReferences);

                // has never been published before (remove old version)
				return Compose("Publish", Authorize(Permission.Publish), validate, updateObject,/* makeVersionOfMaster,*/ replaceMaster, delete, useMaster, publishedState, moveToPosition/*, publishedDate*/, save, updateReferences);
            }
            else if (context.Interface == Interfaces.Viewing)
            {
                // Viewing
                if (!context.Content.VersionOf.HasValue)
                {
                    if (context.Content.ID == 0)
						return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, incrementVersionIndex, publishedState, moveToPosition/*, publishedDate*/, save);

                    if (context.Content.State == ContentState.Draft && context.Content.Published.HasValue == false)
						return Compose("Publish", Authorize(Permission.Publish), validate, makeVersion, updateObject, incrementVersionIndex, publishedState, moveToPosition, publishedDate, save, updateReferences);

					return Compose("Publish", Authorize(Permission.Publish), validate, makeVersion, updateObject, incrementVersionIndex, publishedState, moveToPosition/*, publishedDate*/, save, updateReferences);
                }
                
                if (context.Content.State == ContentState.Unpublished)
					return Compose("Re-Publish", Authorize(Permission.Publish),/* makeVersionOfMaster,*/ replaceMaster, useMaster, incrementVersionIndex, publishedState, moveToPosition/*, publishedDate*/, save, updateReferences);

				if (context.Content.State == ContentState.Draft)
					return Compose("Re-Publish", Authorize(Permission.Publish),/* makeVersionOfMaster,*/ replaceMaster, useMaster, incrementVersionIndex, publishedState, moveToPosition, publishedDate, save, updateReferences);

				return Compose("Publish", Authorize(Permission.Publish),/* makeVersionOfMaster,*/ replaceMaster, delete, useMaster, publishedState, moveToPosition/*, publishedDate*/, save, updateReferences);
            }

            throw new NotSupportedException();
		}

		/// <summary>Gets the command to save changes to an item without leaving the editing interface.</summary>
		/// <param name="context">The command context used to determine which command to return.</param>
		/// <returns>A command that when executed will save an item.</returns>
		public virtual CompositeCommand GetSaveCommand(CommandContext context)
		{
			if (context.Interface != Interfaces.Editing)
				throw new NotSupportedException("Save is not supported while " + context.Interface);

			if (context.Content is IActiveContent)
				// handles it's own persistence
				return Compose("Save changes", Authorize(Permission.Write), validate, saveActiveContent);

			if (context.Content.ID != 0 && !context.Content.VersionOf.HasValue)
				// update a master version
				return Compose("Save changes", Authorize(Permission.Write), validate, useNewVersion, updateObject, incrementVersionIndex, draftState, unpublishedDate, save);

			if (context.Content.State == ContentState.Unpublished)
				// previously published
				return Compose("Save changes", Authorize(Permission.Write), validate, clone, updateObject, incrementVersionIndex, draftState, unpublishedDate, save);

			// has never been published before
			return Compose("Save changes", Authorize(Permission.Write), validate, updateObject, draftState, unpublishedDate, save);
		}

        private CommandBase<CommandContext> ReturnTo(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            return new RedirectCommand(url);
        }

        private CommandBase<CommandContext> Authorize(Permission permission)
        {
            return new AuthorizeCommand(security, permission);
        }

		protected virtual CompositeCommand Compose(string title, params CommandBase<CommandContext>[] commands)
        {
			var args = new CommandCreatedEventArgs { Command = new CompositeCommand(title, commands) };
			if (CreatedCommand != null)
				CreatedCommand.Invoke(this, args);
			return args.Command;
        }

		/// <summary>Invoked before returning a command to be executed.</summary>
		public event EventHandler<CommandCreatedEventArgs> CreatedCommand;
	}
   
}
