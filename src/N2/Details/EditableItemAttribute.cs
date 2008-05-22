using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Defines an editable item. The edited item is referenced by the 
	/// property decorated with this attribute. If the property is null a new
	/// item is created and added to the parent items child collection.
	/// </summary>
	/// <example>
	///		public class ParentItem : N2.ContentItem
	///		{
	/// 		[N2.Details.EditableItem]
	/// 		public virtual ChildItem News
	/// 		{
	/// 			get { return (ChildItem)(GetDetail("ChildItem")); }
	/// 			set { SetDetail("ChildItem", value); }
	/// 		}
	///		}
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableItemAttribute : AbstractEditableAttribute, IDisplayable
	{
		#region Fields

		private string defaultChildZoneName;
		private string defaultChildName;

		#endregion

		#region Constructors

		public EditableItemAttribute()
		{
		}

		public EditableItemAttribute(int sortOrder)
			: this(null, null, sortOrder)
		{
		}

		public EditableItemAttribute(string defaultChildName, int sortOrder)
			: this(defaultChildName, null, sortOrder)
		{
		}

		public EditableItemAttribute(string defaultChildName, string defaultChildZoneName, int sortOrder)
		{
			DefaultChildName = defaultChildName;
			DefaultChildZoneName = defaultChildZoneName;
			SortOrder = sortOrder;
		}

		#endregion

		#region Properties

		/// <summary>The name that will be assigned to new child items.</summary>
		public string DefaultChildName
		{
			get { return defaultChildName; }
			set { defaultChildName = value; }
		}

		/// <summary>The zone name that will be assigned to new child items.</summary>
		public string DefaultChildZoneName
		{
			get { return defaultChildZoneName; }
			set { defaultChildZoneName = value; }
		}

		protected virtual IDefinitionManager Definitions
		{
			get { return Context.Definitions; }
		}

		#endregion

		#region Methods

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			ItemEditor itemEditor = editor as ItemEditor;
			itemEditor.Update();
			ItemUtility.FindInParents<IItemEditor>(editor.Parent).Saved += delegate
			                                                  	{
			                                                  		itemEditor.Save();
			                                                  	};
			return true;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
		}

		public override Control AddTo(Control container)
		{
			Control panel = AddPanel(container);

			return AddEditor(panel);
		}

		protected override Control AddEditor(Control panel)
		{
			ItemEditor editor = new ItemEditor();
			editor.ID = Name;
			editor.ZoneName = DefaultChildZoneName;
			editor.Init += OnChildEditorInit;
			panel.Controls.Add(editor);
			return editor;
		}

		protected virtual void OnChildEditorInit(object sender, EventArgs e)
		{
			ItemEditor itemEditor = sender as ItemEditor;
			IItemEditor parentEditor = ItemUtility.FindInParents<IItemEditor>(itemEditor.Parent);
			itemEditor.CurrentItem = GetChild(parentEditor.CurrentItem);
		}

		protected virtual ContentItem GetChild(ContentItem item)
		{
			ContentItem childItem = Utility.GetProperty(item, Name) as ContentItem;
			if (childItem == null)
			{
				PropertyInfo pi = item.GetType().GetProperty(Name);

				if (pi == null)
					throw new N2Exception("The item should have had a property named '{0}'", Name);
				childItem = CreateChild(item, pi.PropertyType);

				pi.SetValue(item, childItem, null);
			}
			return childItem;
		}

		protected virtual ContentItem CreateChild(ContentItem item, Type childItemType)
		{
			ContentItem child;
			try
			{
				child = Definitions.CreateInstance(childItemType, item);
			}
			catch (KeyNotFoundException ex)
			{
				Trace.TraceWarning(
					"EditableItemAttribute.CreateChild: No item of the type {0} was found among the item definitions.", childItemType);
				throw new N2Exception(
					string.Format(
						"No item of the type {0} was found among item definitions. This could happen if the referenced item type an abstract class or a class that doesn't inherit from N2.ContentItem.",
						childItemType),
					ex);
			}
			child.Name = DefaultChildName;
			child.ZoneName = DefaultChildZoneName;
			child.AddTo(item);
			return child;
		}

		#endregion

		#region IDisplayable Members

		Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
		{
			ContentItem linkedItem = item[detailName] as ContentItem;
			if (linkedItem != null)
			{
				if (linkedItem.IsPage)
					return DisplayableAnchorAttribute.AddAnchor(container, linkedItem);
				else
					return ItemUtility.AddUserControl(container, linkedItem, linkedItem.TemplateUrl);
			}
			return null;
		}

		#endregion
	}
}