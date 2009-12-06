using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Workflow.Commands;
using N2.Edit;
using N2.Security;
using N2.Persistence;
using System.Diagnostics;

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
        AuthorizeCommand authorize;
        SaveCommand save;
        IncrementVersionIndexCommand setVersionIndex;
        UpdateContentStateCommand makeDraft;
        UpdateContentStateCommand makePublished;

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
            authorize = new AuthorizeCommand(security);
            save = new SaveCommand(persister);
            setVersionIndex = new IncrementVersionIndexCommand(versionMaker);
            makeDraft = new UpdateContentStateCommand(changer, ContentState.Draft);
            makePublished = new UpdateContentStateCommand(changer, ContentState.Published);
        }

        //public virtual IEnumerable<Command<CommandState>> AvailableCommands(CommandState context)
        //{
        //    if (context.Interface == Interfaces.Editing)
        //        return EditCommands(context);
        //    else if (context.Interface == Interfaces.Viewing)
        //        return ViewCommands(context);

        //    throw new InvalidOperationException("No commands for interface: " + context.Interface);
        //}

        //private IEnumerable<Command<CommandState>> EditCommands(CommandState context)
        //{
        //    yield return PublishCommand(context);
        //    yield return PreviewCommand(context);
        //    yield return SaveCommand(context);
        //    yield return Redirect("Cancel", context.Data);
        //}

        //private IEnumerable<Command<CommandState>> ViewCommands(CommandState context)
        //{
        //    if (context.Data.VersionOf == null)
        //    {
        //        // master version
        //    }
        //    else
        //    {
        //        yield return PublishCommand(context);
        //        yield return Composite("Delete", authorize, delete, useMaster, showPreview);
        //        yield return new RedirectCommand<CommandState> { Title = "Edit", Url = "#edit" + context.Data.Url }; // TODO: fix
        //    }
        //}

        /// <summary>Gets the command used to publish an item.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will publish an item.</returns>
        public virtual CommandBase<CommandContext> GetPublishCommand(CommandContext context)
        {
            if (context.Interface == Interfaces.Editing)
            {
                // Editing
                if(context.Data.VersionOf == null)
                    return Compose("Publish", Intent(Permission.Publish), authorize, validate, makeVersion, updateObject, setVersionIndex, makePublished, save, showPreview);
                else if (context.Data.State == ContentState.Unpublished)
                    // has been published before
                    return Compose("Publish", Intent(Permission.Publish), authorize, validate, updateObject,/* makeVersionOfMaster,*/ replaceMaster, useMaster, setVersionIndex, makePublished, save, showPreview);
                else
                    // has never been published before (remove old version)
                    return Compose("Publish", Intent(Permission.Publish), authorize, validate, updateObject,/* makeVersionOfMaster,*/ replaceMaster, delete, useMaster, makePublished, save, showPreview);
            }
            else if (context.Interface == Interfaces.Viewing && context.Data.VersionOf != null)
            {
                // Viewing
                if (context.Data.State == ContentState.Unpublished)
                    return Compose("Re-Publish", Intent(Permission.Publish), authorize,/* makeVersionOfMaster,*/ replaceMaster, useMaster, setVersionIndex, makePublished, save, showPreview);
                else
                    return Compose("Publish", Intent(Permission.Publish), authorize,/* makeVersionOfMaster,*/ replaceMaster, delete, useMaster, makePublished, save, showPreview);
            }

            throw new NotSupportedException();
        }

        private CommandBase<CommandContext> Intent(Permission permission)
        {
 	        return new IntentCommand(permission);
        }

        /// <summary>Gets the command to save and preview changes.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will publish an item.</returns>
        public virtual CommandBase<CommandContext> GetPreviewCommand(CommandContext context)
        {
            if (context.Interface != Interfaces.Editing)
                throw new NotSupportedException("Preview is not supported while " + context.Interface);

            if (context.Data.VersionOf == null)
                // is master version
                return Compose("Save and preview", Intent(Permission.Write), authorize, validate, useNewVersion, updateObject, setVersionIndex, makeDraft, save, showPreview);
            else if (context.Data.State == ContentState.Unpublished)
                // has been published before
                return Compose("Save and preview", Intent(Permission.Write), authorize, validate, clone,         updateObject, setVersionIndex, makeDraft, save, showPreview);
            else
                // has never been published before
                return Compose("Save and preview", Intent(Permission.Write), authorize, validate,                updateObject, setVersionIndex, makeDraft, save, showPreview);
        }

        /// <summary>Gets the command to save changes to an item without leaving the editing interface.</summary>
        /// <param name="context">The command context used to determine which command to return.</param>
        /// <returns>A command that when executed will save an item.</returns>
        public virtual CommandBase<CommandContext> GetSaveCommand(CommandContext context)
        {
            if (context.Interface != Interfaces.Editing)
                throw new NotSupportedException("Save is not supported while " + context.Interface);

            if (context.Data.VersionOf == null)
                // is master version
                return Compose("Save changes", Intent(Permission.Write), authorize, validate, useNewVersion, updateObject, setVersionIndex, makeDraft, save, showEdit);
            else if (context.Data.State == ContentState.Unpublished)
                // previously published
                return Compose("Save changes", Intent(Permission.Write), authorize, validate, clone,         updateObject, setVersionIndex, makeDraft, save, showEdit);
            else
                // has never been published before
                return Compose("Save changes", Intent(Permission.Write), authorize, validate,                updateObject,                  makeDraft, save, showEdit);
        }

        protected virtual CommandBase<CommandContext> Compose(string title, params CommandBase<CommandContext>[] commands)
        {
            return new CompositeCommand(persister.Repository, title, commands);
        }
    }
   
}
