using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Web;

namespace N2.Details
{
    /// <summary>Attribute used to mark properties as editable. This is used to associate the control used for the editing with the property/detail on the content item whose value we are editing.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableUserControlAttribute : AbstractEditableAttribute
    {
		string editorPropertyName;
		string userControlPath;

		#region Constructors
		/// <summary>Initializes a new instance of the EditableAttribute class set to use a user control.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="userControlPath">The virtual path of a user control used for editing</param>
		/// <param name="editorPropertyName">The property on the edit control that will update the unit's property</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableUserControlAttribute(string title, string userControlPath, string editorPropertyName, int sortOrder)
			: base(title, sortOrder)
		{
			this.UserControlPath = userControlPath;
			this.editorPropertyName = editorPropertyName;
		}
		/// <summary>Initializes a new instance of the EditableAttribute class set to use a user control.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="userControlPath">The virtual path of a user control used for editing</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableUserControlAttribute(string title, string userControlPath, int sortOrder)
			: base(title, sortOrder)
		{
			this.UserControlPath = userControlPath;
			this.editorPropertyName = null;
		}
		/// <summary>Initializes a new instance of the EditableAttribute class set to use a user control.</summary>
		/// <param name="userControlPath">The virtual path of a user control used for editing</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableUserControlAttribute(string userControlPath, int sortOrder)
			: base("", sortOrder)
		{
			this.UserControlPath = userControlPath;
			this.editorPropertyName = null;
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the virtual path of a user control. This property is only considered when ControlType is <see cref="System.Web.UI.UserControl"/>.</summary>
		public string UserControlPath
		{
			get { return userControlPath; }
			set { userControlPath = value; }
		} 
		#endregion


		public override bool UpdateItem(ContentItem item, Control editor)
		{
			if (editor is IContentBinder)
			{
				IContentBinder binder = editor as IContentBinder;
				return binder.UpdateObject(item);
			}
			else
			{
				var current = item[Name];
				var updated = Utility.GetProperty(editor, editorPropertyName);
				if (current == updated)
					return false;

				item[Name] = updated;
				return true;
			}
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			if (editor is IContentBinder)
			{
				IContentBinder binder = editor as IContentBinder;
				binder.UpdateInterface(item);
			}
			else
			{
				Utility.SetProperty(editor, editorPropertyName, item[Name]);
			}
		}

		public override Control AddTo(Control container)
		{
			if (string.IsNullOrEmpty(Title))
			{
				Control editor = AddEditor(container);
				AddHelp(container);
				return editor;
			}

			return base.AddTo(container);
		}

		protected override Control AddEditor(Control container)
		{
			Control c = container.Page.LoadControl(Url.ResolveTokens(this.UserControlPath));
			container.Controls.Add(c);
			return c;
		}
	}
}
