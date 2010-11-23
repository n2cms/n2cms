using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using System.Web.UI;
using N2.Collections;
using N2.Configuration;
using N2.Definitions;
using N2.Edit.Settings;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence;
using N2.Plugin;
using N2.Security;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	/// <summary>
	/// Class responsible for plugins in edit mode, knowling links to edit
	/// pages and saving interaction.
	/// </summary>
	[Service(typeof (IEditManager))]
	public class EditManager : IEditManager
	{
		protected static readonly object addedEditorKey = new object();
		protected static readonly object savingVersionKey = new object();

		private readonly IDefinitionManager definitions;
		private readonly IPersister persister;
		private readonly IPluginFinder pluginFinder;
		private readonly ISecurityManager securityManager;
		private readonly NavigationSettings settings;
		private readonly StateChanger stateChanger;
		private readonly IList<string> uploadFolders = new List<string>();
		private readonly IVersionManager versioner;
		protected EventHandlerList Events = new EventHandlerList();

		public EditManager(IDefinitionManager definitions, IPersister persister, IVersionManager versioner,
		                   ISecurityManager securityManager, IPluginFinder pluginFinder, NavigationSettings settings,
		                   StateChanger changer, EditSection config)
		{
			this.definitions = definitions;
			this.persister = persister;
			this.versioner = versioner;
			this.securityManager = securityManager;
			this.pluginFinder = pluginFinder;
			stateChanger = changer;
			this.settings = settings;

			EditTheme = config.EditTheme;
			EnableVersioning = config.Versions.Enabled;
			MaximumNumberOfVersions = config.Versions.MaximumPerItem;
			uploadFolders = new List<string>(config.UploadFolders.Folders);
		}

		/// <summary>Number of item versions to keep.</summary>
		protected virtual int MaximumNumberOfVersions { get; set; }

		#region IEditManager Members

		public virtual string EditTheme { get; set; }

		/// <summary>Gets or sets wether a version is saved when updating items.</summary>
		public virtual bool EnableVersioning { get; set; }

		/// <summary>Gets folders paths on the server where users are allowed to upload content through the interface.</summary>
		public virtual IList<string> UploadFolders
		{
			get { return uploadFolders; }
		}

		/// <summary>Adds defined editors and containers to a control.</summary>
		/// <param name="itemType">The type of content item whose editors to add.</param>
		/// <param name="editorContainer">The container onto which add the editors.</param>
		/// <param name="user">The user whose credentials will be queried.</param>
		public virtual IDictionary<string, Control> AddEditors(Type itemType, Control editorContainer, IPrincipal user)
		{
			ItemDefinition definition = definitions.GetDefinition(itemType);
			IEditableContainer rootContainer = definition.RootContainer;
			IDictionary<string, Control> addedEditors = new Dictionary<string, Control>();
			AddEditorsRecursive(rootContainer, editorContainer, user, addedEditors);
			return addedEditors;
		}

		/// <summary>Sets initial editor values.</summary>
		/// <param name="addedEditors">Previously added editor controls.</param>
		/// <param name="item">The content item to use for update.</param>
		/// <param name="user">The current user.</param>
		public virtual void UpdateEditors(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (addedEditors == null) throw new ArgumentNullException("addedEditors");

			ItemDefinition definition = definitions.GetDefinition(item.GetContentType());
			ApplyModifications(definition, addedEditors);
			foreach (IEditable e in definition.GetEditables(user))
			{
				if (addedEditors.ContainsKey(e.Name) && addedEditors[e.Name] != null)
					e.UpdateEditor(item, addedEditors[e.Name]);
			}
		}

		/// <summary>Updates the item by way of letting the defined editable attributes interpret the added editors.</summary>
		/// <param name="item">The item to update.</param>
		/// <param name="addedEditors">The previously added editors.</param>
		/// <param name="user">The user for filtering updatable editors.</param>
		/// <returns>Whether any property on the item was updated.</returns>
		public virtual string[] UpdateItem(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (addedEditors == null) throw new ArgumentNullException("addedEditors");

			var updatedDetails = new List<string>();

			ItemDefinition definition = definitions.GetDefinition(item.GetContentType());
			foreach (IEditable e in definition.GetEditables(user))
			{
				if (addedEditors.ContainsKey(e.Name))
				{
					bool wasUpdated = e.UpdateItem(item, addedEditors[e.Name]);
					if (wasUpdated)
						updatedDetails.Add(e.Name);
				}
			}

			if (updatedDetails.Count > 0)
			{
				item.Updated = Utility.CurrentTime();
				if (user != null)
					item.SavedBy = user.Identity.Name;
			}

			return updatedDetails.ToArray();
		}

		/// <summary>Saves an item using values from the supplied item editor.</summary>
		/// <param name="item">The item to update.</param>
		/// <param name="addedEditors">The editors to update the item with.</param>
		/// <param name="versioningMode">How to treat the item beeing saved in respect to versioning.</param>
		/// <param name="user">The user that is performing the saving.</param>
		public virtual ContentItem Save(ContentItem item, IDictionary<string, Control> addedEditors,
		                                ItemEditorVersioningMode versioningMode, IPrincipal user)
		{
			// when an unpublished version is saved and published
			if (versioningMode == ItemEditorVersioningMode.SaveAsMaster)
			{
				return SaveAsMaster(item, addedEditors, user);
			}

			// when an item is saved without any new version
			if (versioningMode == ItemEditorVersioningMode.SaveOnly)
			{
				return SaveOnly(item, addedEditors, user);
			}

			// when an item is saved but a version is stored before the item is updated
			if (versioningMode == ItemEditorVersioningMode.VersionAndSave)
			{
				return VersionAndSave(item, addedEditors, user);
			}

			// when making a version without publishing the item
			if (versioningMode == ItemEditorVersioningMode.VersionOnly)
			{
				return VersionOnly(item, addedEditors, user);
			}

			throw new ArgumentException("Unexpected versioning mode.", "versioningMode");
		}

		public virtual IEnumerable<T> GetPlugins<T>(IPrincipal user)
			where T : AdministrativePluginAttribute
		{
			return pluginFinder.GetPlugins<T>(user);
		}

		public virtual ItemFilter GetEditorFilter(IPrincipal user)
		{
			ItemFilter filter = new AccessFilter(user, securityManager);
			if (!settings.DisplayDataItems)
			{
				filter = new CompositeFilter(new PageFilter(), filter);
			}
			return filter;
		}

		#endregion

		#region Helper

		/// <summary>Applies defined modifications to the editors.</summary>
		public virtual void ApplyModifications(ItemDefinition definition, IDictionary<string, Control> addedEditors)
		{
			foreach (string name in addedEditors.Keys)
			{
				foreach (EditorModifierAttribute em in definition.GetModifiers(name))
				{
					Control editor = addedEditors[em.Name];
					em.Modify(editor);
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>Adds editors and containers to the supplied container.</summary>
		/// <param name="containerControl">The control on which editors and containres will be added.</param>
		/// <param name="contained">The definition that will add a control in the container.</param>
		/// <param name="user"></param>
		/// <param name="addedEditors"></param>
		public virtual void AddEditorsRecursive(IContainable contained, Control containerControl, IPrincipal user,
		                                        IDictionary<string, Control> addedEditors)
		{
			Control added = contained.AddTo(containerControl);

			if (contained is IEditable)
			{
				addedEditors[contained.Name] = added;
				OnAddedEditor(new ControlEventArgs(added));
			}
			if (contained is IEditableContainer)
			{
				foreach (IContainable subContained in ((IEditableContainer) contained).GetContained(user))
				{
					AddEditorsRecursive(subContained, added, user, addedEditors);
				}
			}
		}

		#endregion

		/// <summary>
		/// Event that is triggered when page is saved/published
		/// </summary>
		public event EventHandler<ItemEventArgs> ItemSaved;

		protected virtual void OnItemSaved(ItemEventArgs e)
		{
			if (ItemSaved != null)
				ItemSaved(this, e);
		}

		private ContentItem SaveAsMaster(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			using (ITransaction tx = persister.Repository.BeginTransaction())
			{
				ContentItem itemToUpdate = item.VersionOf;
				if (itemToUpdate == null)
					throw new ArgumentException("Expected the current item to be a version of another item.", "item");

				if (ShouldCreateVersionOf(item))
					SaveVersion(itemToUpdate);

				DateTime? published = itemToUpdate.Published;
				bool wasUpdated = UpdateItem(itemToUpdate, addedEditors, user).Length > 0;
				if (wasUpdated || IsNew(itemToUpdate))
				{
					itemToUpdate.Published = published ?? Utility.CurrentTime();
					stateChanger.ChangeTo(itemToUpdate, ContentState.Published);
					persister.Save(itemToUpdate);
				}

				tx.Commit();

				OnItemSaved(new ItemEventArgs(itemToUpdate));
				return item.VersionOf;
			}
		}

		private ContentItem SaveOnly(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			bool wasUpdated = UpdateItem(item, addedEditors, user).Length > 0;
			if (wasUpdated || IsNew(item))
			{
				if (item.VersionOf == null)
					stateChanger.ChangeTo(item, ContentState.Published);
				else if (item.State == ContentState.Unpublished)
					// TODO: handle changes to previously published
					stateChanger.ChangeTo(item, ContentState.Draft);
				else
					stateChanger.ChangeTo(item, ContentState.Draft);

				persister.Save(item);
			}

			OnItemSaved(new ItemEventArgs(item));
			return item;
		}

		private ContentItem VersionAndSave(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			using (ITransaction tx = persister.Repository.BeginTransaction())
			{
				if (ShouldCreateVersionOf(item))
					SaveVersion(item);

				DateTime? initialPublished = item.Published;
				bool wasUpdated = UpdateItem(item, addedEditors, user).Length > 0;
				DateTime? updatedPublished = item.Published;

				// the item was the only version of an unpublished item - publish it
				if (initialPublished == null && updatedPublished == null)
				{
					item.Published = Utility.CurrentTime();
					stateChanger.ChangeTo(item, ContentState.Published);
					wasUpdated = true;
				}

				if (wasUpdated || IsNew(item))
				{
					if (item.VersionOf == null)
						stateChanger.ChangeTo(item, ContentState.Published);
					item.VersionIndex++;
					persister.Save(item);
				}

				tx.Commit();

				OnItemSaved(new ItemEventArgs(item));
				return item;
			}
		}

		private ContentItem VersionOnly(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			using (ITransaction tx = persister.Repository.BeginTransaction())
			{
				if (ShouldCreateVersionOf(item))
					item = SaveVersion(item);

				bool wasUpdated = UpdateItem(item, addedEditors, user).Length > 0;
				if (wasUpdated || IsNew(item))
				{
					item.Published = null;
					stateChanger.ChangeTo(item, ContentState.Draft);
					item.VersionIndex++;
					persister.Save(item);
				}

				tx.Commit();

				OnItemSaved(new ItemEventArgs(item));
				return item;
			}
		}

		private bool ShouldCreateVersionOf(ContentItem item)
		{
			return EnableVersioning && !IsNew(item) && versioner.IsVersionable(item);
		}

		/// <summary>Occurs when a detail editor (a control that contains an editor) is added.</summary>
		public event EventHandler<ControlEventArgs> AddedEditor
		{
			add { Events.AddHandler(addedEditorKey, value); }
			remove { Events.RemoveHandler(addedEditorKey, value); }
		}

		/// <summary>Occurs when a version is about to be saved.</summary>
		public event EventHandler<CancellableItemEventArgs> SavingVersion
		{
			add { Events.AddHandler(savingVersionKey, value); }
			remove { Events.RemoveHandler(savingVersionKey, value); }
		}

		#region Helper Methods

		private ContentItem SaveVersion(ContentItem current)
		{
			ContentItem savedVersion = null;
			var handler = Events[savingVersionKey] as EventHandler<CancellableItemEventArgs>;
			Utility.InvokeEvent(handler, current, this, delegate(ContentItem item)
			                                            	{
			                                            		savedVersion = versioner.SaveVersion(item);
			                                            		versioner.TrimVersionCountTo(item, MaximumNumberOfVersions);
			                                            	});
			return savedVersion;
		}

		private static bool IsNew(ContentItem current)
		{
			return current.ID == 0;
		}

		/// <summary>
		/// Triggers the AddedEditor event.
		/// </summary>
		private void OnAddedEditor(ControlEventArgs args)
		{
			var handler = Events[addedEditorKey] as EventHandler<ControlEventArgs>;
			if (handler != null)
				handler.Invoke(this, args);
		}

		#endregion
	}
}