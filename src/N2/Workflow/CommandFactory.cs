using System;
using N2.Edit;
using N2.Engine;
using N2.Persistence;
using N2.Security;
using N2.Workflow.Commands;

namespace N2.Workflow
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
        IncrementVersionIndexCommand setVersionIndex;
        UpdateContentStateCommand makeDraft;
        UpdateContentStateCommand makePublished;
        ActiveContentSaveCommand saveActiveContent;
		MoveToPositionCommand moveToPosition;
        ISecurityManager security;

        public CommandFactory(IPersister persister, ISecurityManager security, IVersionManager versionMaker, IEditManager editManager, StateChanger changer)
        {
            this.persister = persister;
            makeVersionOfMaster = On.Master(new MakeVersionCommand(versionMaker));
            replaceMaster = new ReplaceMasterCommand(versionMaker);
            makeVersion = new MakeVersionCommand(versionMaker);
            useNewVersion = new UseNewVersionCommand(versionMaker);
            updateObject = new UpdateObjectCommand();
            delete = new DeleteCommand(persister.Repository);
            showPreview = new RedirectToPreviewCommand(editManager);
            showEdit = new RedirectToEditCommand(editManager);
            useMaster = new UseMasterCommand();
            clone = new CloneCommand();
            validate = new ValidateCommand();
            this.security = security;
            save = new SaveCommand(persister);
            setVersionIndex = new IncrementVersionIndexCommand(versionMaker);
            makeDraft = new UpdateContentStateCommand(changer, ContentState.Draft);
            makePublished = new UpdateContentStateCommand(changer, ContentState.Published);
            saveActiveContent = new ActiveContentSaveCommand();
			moveToPosition = new MoveToPositionCommand();
        }

        /// <summary>Gets the command used to publish an item.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will publish an item.</returns>
        public virtual CommandBase<CommandContext> GetPublishCommand(CommandContext context)
        {
            if (context.Interface == Interfaces.Editing)
            {
                if (context.Content is IActiveContent)
					return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, moveToPosition, saveActiveContent, ReturnTo(context.RedirectTo) ?? showPreview);
                
                // Editing
				if (context.Content.VersionOf == null)
				{
					if(context.Content.ID == 0)
						return Compose("Publish", Authorize(Permission.Publish), validate, updateObject, setVersionIndex, makePublished, moveToPosition, save, ReturnTo(context.RedirectTo) ?? showPreview);

					return Compose("Publish", Authorize(Permission.Publish), validate, makeVersion, updateObject, setVersionIndex, makePublished, moveToPosition, save, ReturnTo(context.RedirectTo) ?? showPreview);
				}

				// has been published before
				if (context.Content.State == ContentState.Unpublished)
					return Compose("Publish", Authorize(Permission.Publish), validate, updateObject,/* makeVersionOfMaster,*/ replaceMaster, useMaster, setVersionIndex, makePublished, moveToPosition, save, ReturnTo(context.RedirectTo) ?? showPreview);
                
                // has never been published before (remove old version)
				return Compose("Publish", Authorize(Permission.Publish), validate, updateObject,/* makeVersionOfMaster,*/ replaceMaster, delete, useMaster, makePublished, moveToPosition, save, ReturnTo(context.RedirectTo) ?? showPreview);
            }
            else if (context.Interface == Interfaces.Viewing && context.Content.VersionOf != null)
            {
                // Viewing
                if (context.Content.State == ContentState.Unpublished)
					return Compose("Re-Publish", Authorize(Permission.Publish),/* makeVersionOfMaster,*/ replaceMaster, useMaster, setVersionIndex, makePublished, moveToPosition, save, ReturnTo(context.RedirectTo) ?? showPreview);

				return Compose("Publish", Authorize(Permission.Publish),/* makeVersionOfMaster,*/ replaceMaster, delete, useMaster, makePublished, moveToPosition, save, ReturnTo(context.RedirectTo) ?? showPreview);
            }

            throw new NotSupportedException();
        }

        private CommandBase<CommandContext> ReturnTo(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            return new RedirectCommand(url);
        }

        /// <summary>Gets the command to save and preview changes.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will publish an item.</returns>
        public virtual CommandBase<CommandContext> GetPreviewCommand(CommandContext context)
        {
            if (context.Interface != Interfaces.Editing)
                throw new NotSupportedException("Preview is not supported while " + context.Interface);

            if (context.Content is IActiveContent)
                // handles it's own persistence
                return Compose("Save and Preview", Authorize(Permission.Write), validate, saveActiveContent, ReturnTo(context.RedirectTo) ?? showPreview);
            
            if (context.Content.ID != 0 && context.Content.VersionOf == null)
                // update a master version
                return Compose("Save and preview", Authorize(Permission.Write), validate, useNewVersion, updateObject, setVersionIndex, makeDraft, save, ReturnTo(context.RedirectTo) ?? showPreview);
            
            if (context.Content.State == ContentState.Unpublished)
                // has been published before
                return Compose("Save and preview", Authorize(Permission.Write), validate, clone,         updateObject, setVersionIndex, makeDraft, save, ReturnTo(context.RedirectTo) ?? showPreview);
            
            // has never been published before
            return Compose("Save and preview", Authorize(Permission.Write), validate,                updateObject, setVersionIndex, makeDraft, save, ReturnTo(context.RedirectTo) ?? showPreview);
        }

        /// <summary>Gets the command to save changes to an item without leaving the editing interface.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will save an item.</returns>
        public virtual CommandBase<CommandContext> GetSaveCommand(CommandContext context)
        {
            if (context.Interface != Interfaces.Editing)
                throw new NotSupportedException("Save is not supported while " + context.Interface);

            if (context.Content is IActiveContent)
                // handles it's own persistence
                return Compose("Save changes", Authorize(Permission.Write), validate, saveActiveContent, showEdit);
            
            if (context.Content.ID != 0 && context.Content.VersionOf == null)
                // update a master version
                return Compose("Save changes", Authorize(Permission.Write), validate, useNewVersion,        updateObject, setVersionIndex, makeDraft, save, showEdit);
            
            if (context.Content.State == ContentState.Unpublished)
                // previously published
                return Compose("Save changes", Authorize(Permission.Write), validate,                clone, updateObject, setVersionIndex, makeDraft, save, showEdit);
            
            // has never been published before
            return Compose("Save changes", Authorize(Permission.Write), validate,                       updateObject,                  makeDraft, save, showEdit);
        }

        private CommandBase<CommandContext> Authorize(Permission permission)
        {
            return new AuthorizeCommand(security, permission);
        }

        protected virtual CommandBase<CommandContext> Compose(string title, params CommandBase<CommandContext>[] commands)
        {
            return new CompositeCommand(persister.Repository, title, commands);
        }
    }
   
}
