using System;
using N2.Edit.Workflow.Commands;
using N2.Engine;
using N2.Persistence;
using N2.Security;
using System.Linq;
using N2.Edit.Versioning;

namespace N2.Edit.Workflow
{
    /// <summary>
    /// Provides and executies commands used to change the state of content items.
    /// </summary>
    [Service(typeof(ICommandFactory))]
    public class CommandFactory : ICommandFactory
    {
        IPersister persister;
        ISecurityManager security;
        
        CommandBase<CommandContext> makeVersionOfMaster;
        ReplaceMasterCommand replaceMaster;
        MakeVersionCommand makeVersion;
        UseDraftCommand useNewVersion;
        UpdateObjectCommand updateObject;
        DeleteCommand delete;
        RedirectToEditCommand showEdit;
        UseMasterCommand useMaster;
        CloneCommand clone;
        ValidateCommand validate;
        SaveCommand save;
        UpdateContentStateCommand draftState;
        UpdateContentStateCommand publishedState;
        ActiveContentSaveCommand saveActiveContent;
        MoveToPositionCommand moveToPosition;
        EnsureNotPublishedCommand unpublishedDate;
        EnsurePublishedCommand ensurePublishedDate;
        UpdateReferencesCommand updateReferences;
        SaveOnPageVersionCommand saveOnPageVersion;

        public CommandFactory(IPersister persister, ISecurityManager security, IVersionManager versionMaker, IEditUrlManager editUrlManager, IContentAdapterProvider adapters, StateChanger changer)
        {
            this.persister = persister;
			this.security = security;

			makeVersionOfMaster = On.Master(new MakeVersionCommand(versionMaker));
            replaceMaster = new ReplaceMasterCommand(versionMaker);
            makeVersion = new MakeVersionCommand(versionMaker);
            useNewVersion = new UseDraftCommand(versionMaker);
            updateObject = new UpdateObjectCommand();
            delete = new DeleteCommand(persister.Repository);
            showEdit = new RedirectToEditCommand(editUrlManager);
            useMaster = new UseMasterCommand();
            clone = new CloneCommand();
            validate = new ValidateCommand();
            save = new SaveCommand(persister);
            draftState = new UpdateContentStateCommand(changer, ContentState.Draft);
            publishedState = new UpdateContentStateCommand(changer, ContentState.Published);
            saveActiveContent = new ActiveContentSaveCommand();
            moveToPosition = new MoveToPositionCommand();
            unpublishedDate = new EnsureNotPublishedCommand();
            ensurePublishedDate = new EnsurePublishedCommand();
            updateReferences = new UpdateReferencesCommand();
            saveOnPageVersion = new SaveOnPageVersionCommand(versionMaker);
        }

        /// <summary>Gets the command used to publish an item.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will publish an item.</returns>
        public virtual CompositeCommand GetPublishCommand(CommandContext context)
        {
            var item = context.Content;

            if (item is IActiveContent)
                return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, moveToPosition, saveActiveContent, updateReferences);
                
            // Editing
            if (!item.VersionOf.HasValue)
            {
                if(item.ID == 0)
                    return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, publishedState, moveToPosition, save, updateReferences);

                return Compose("Publish", Authorize(Permission.Publish), validate, MakeVersionIfPublished(item), updateObject, publishedState, moveToPosition, ensurePublishedDate, save, updateReferences);
            }

            // has been published before
            if (item.State == ContentState.Unpublished)
                return Compose("Re-Publish", Authorize(Permission.Publish), validate, replaceMaster, useMaster, publishedState, moveToPosition, ensurePublishedDate, save, updateReferences);

            // has never been published before (remove old version)
            return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, replaceMaster, delete, useMaster, publishedState, moveToPosition, ensurePublishedDate, save, updateReferences);
            
            throw new NotSupportedException();
        }

        private CommandBase<CommandContext> MakeVersionIfPublished(ContentItem item)
        {
            if (item.State == ContentState.Published || item.State == ContentState.Unpublished)
                return makeVersion;
            return null;
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

            if (context.Content.IsPage)
            {
                if (context.Content.ID != 0 && !context.Content.VersionOf.HasValue)
                {
                    // is master version
                    if (context.Content.State == ContentState.Published || context.Content.State == ContentState.Unpublished)
                        // update of a master version
                        return Compose("Save changes", Authorize(Permission.Write), validate, useNewVersion, updateObject, draftState, unpublishedDate, saveOnPageVersion);
                    else
                        return Compose("Save changes", Authorize(Permission.Write), validate, updateObject, draftState, unpublishedDate, save);
                }
                else if (context.Content.State == ContentState.Published || context.Content.State == ContentState.Unpublished)
                    return Compose("Save changes", Authorize(Permission.Write), validate, useNewVersion, updateObject, draftState, unpublishedDate, saveOnPageVersion);
                else
                    return Compose("Save changes", Authorize(Permission.Write), validate, updateObject, draftState, unpublishedDate, save);
            }
            else
            {
                // parts are saved as a version to their page
                return Compose("Save changes", Authorize(Permission.Write), validate, useNewVersion, updateObject, draftState, unpublishedDate, saveOnPageVersion);
            }
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
            var args = new CommandCreatedEventArgs { Command = new CompositeCommand(title, commands.Where(c => c != null).ToArray()) };
            if (CreatedCommand != null)
                CreatedCommand.Invoke(this, args);
            return args.Command;
        }

        /// <summary>Invoked before returning a command to be executed.</summary>
        public event EventHandler<CommandCreatedEventArgs> CreatedCommand;
    }
   
}
