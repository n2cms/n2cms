using System;
using System.Collections.Generic;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Web;
using System.Web.UI;
using System.Security.Principal;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	/// <summary>
	/// Controls aspects related to the editor interface and editing content items.
	/// </summary>
	[Controls(typeof(ContentItem))]
	public class EditableAdapter : AbstractContentAdapter
	{
		/// <summary>Adds the editors defined for the item to the control hierarchy.</summary>
		/// <param name="itemType">The type to add editors for.</param>
		/// <param name="container">The container onto which to add editors.</param>
		/// <param name="user">The user to filter access by.</param>
		/// <returns>A editor name to control map of added editors.</returns>
		public virtual IDictionary<string, Control> AddDefinedEditors(Type itemType, Control container, IPrincipal user)
		{
			return Engine.EditManager.AddEditors(itemType, container, user);
		}

		/// <summary>Updates editors with values from the item.</summary>
		/// <param name="item">The item containing values.</param>
		/// <param name="addedEditors">A map of editors to update (may have been filtered by access).</param>
		/// <param name="user">The user to filter access by.</param>
		public virtual void LoadAddedEditors(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			Engine.EditManager.UpdateEditors(item, addedEditors, user);
		}

		/// <summary>Updates an item with the values from the editor controls without saving it.</summary>
		/// <param name="item">The item to update.</param>
		/// <param name="addedEditors">Editors containing interesting values.</param>
		/// <param name="user">The user to filter access by.</param>
		public virtual void UpdateItem(ContentItem item, IDictionary<string, Control> addedEditors, IPrincipal user)
		{
			Engine.EditManager.UpdateItem(item, addedEditors, user);
		}

		/// <summary>Saves an item using values from the supplied item editor.</summary>
		/// <param name="item">The item to update.</param>
		/// <param name="addedEditors">The editors to update the item with.</param>
		/// <param name="versioningMode">How to treat the item beeing saved in respect to versioning.</param>
		/// <param name="user">The user that is performing the saving.</param>
		/// <returns>The item to continue using.</returns>
		public virtual ContentItem SaveItem(ContentItem item, IDictionary<string, Control> addedEditors, ItemEditorVersioningMode versioningMode, IPrincipal user)
		{
			return Engine.EditManager.Save(item, addedEditors, versioningMode, user);
		}

		/// <summary>Gets the default folder for an item.</summary>
		/// <param name="item">The item whose default folder is requested.</param>
		/// <returns>The default folder.</returns>
		public virtual string GetDefaultFolder(ContentItem item)
		{
			return Engine.Resolve<IDefaultDirectory>().GetDefaultDirectory(item);
		}
	}
}
