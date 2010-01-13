using System;
using System.Reflection;
using System.Web.UI;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Workflow;

namespace N2.Details
{
	/// <summary>
	/// Defines a child item editor. Renders a drop down list where you can 
	/// select what item to add and edit forms of added items.
	/// </summary>
	public class EditableChildrenAttribute : AbstractEditableAttribute
	{
		private string zoneName;

		public EditableChildrenAttribute()
			: base(null, 200)
		{
		}

		public EditableChildrenAttribute(string title, string zoneName, int sortOrder)
			: this(title, zoneName, zoneName, sortOrder)
		{
		}

		public EditableChildrenAttribute(string title, string zoneName, string name, int sortOrder)
			: base(title, name, sortOrder)
		{
			this.zoneName = zoneName;
		}

		public string ZoneName
		{
			get { return zoneName; }
			set { zoneName = value; }
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			ItemEditorList listEditor = (ItemEditorList)editor;
			for (int i = 0; i < listEditor.ItemEditors.Count; i++)
			{
				if (listEditor.DeletedIndexes.Contains(i))
				{
					Context.Persister.Delete(listEditor.ItemEditors[i].CurrentItem);
				}
				else
				{
					ItemEditor childEditor = listEditor.ItemEditors[i];
					ItemEditor parentEditor = ItemUtility.FindInParents<ItemEditor>(editor.Parent);
					if (parentEditor != null)
					{
						var subContext = parentEditor.BinderContext.CreateNestedContext(childEditor, childEditor.CurrentItem);
						return subContext.Binder.UpdateObject(subContext);
					}
					else
					{
						IItemEditor fallbackEditor = ItemUtility.FindInParents<IItemEditor>(editor.Parent);
						if (fallbackEditor != null)
						{
							fallbackEditor.Saved += delegate { childEditor.Save(); };
						}
					}
				}
			}
			return listEditor.DeletedIndexes.Count > 0 || listEditor.AddedTypes.Count > 0;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			ItemEditorList listEditor = (ItemEditorList)editor;
			listEditor.ParentItem = item;
			listEditor.ZoneName = ZoneName;

            // filtering of children by property generic type
            PropertyInfo info = item.GetType().GetProperty(Name);
            if (info != null)
            {
                foreach (Type argument in info.PropertyType.GetGenericArguments())
                {
                    if (typeof(ContentItem).IsAssignableFrom(argument))
                    {
                        listEditor.MinimumType = argument;
                        break;
                    }
                }
            }
		}

		public override Control AddTo(Control container)
		{
			Control panel = AddPanel(container);
			return AddEditor(panel);
		}

		protected override Control AddEditor(Control container)
		{
			ItemEditorList listEditor = new ItemEditorList();
			listEditor.ID = Name;
			listEditor.ParentItem = ItemUtility.FindInParents<IItemEditor>(container).CurrentItem;
			container.Controls.Add(listEditor);
			return listEditor;
		}
	}
}