using System;
using System.Reflection;
using System.Web.UI;
using N2.Edit.Workflow;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Edit.Versioning;
using N2.Persistence.Proxying;
using System.Collections;
using System.Linq;
using N2.Persistence;

namespace N2.Details
{
    /// <summary>
    /// Defines a deletedChild item editor. Renders a drop down list where you can 
    /// select what item to add and edit forms of added items.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableChildrenAttribute : AbstractEditableAttribute, IValueAccessor
    {
        private string zoneName;

        public EditableChildrenAttribute()
            : this(null, null, 200)
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
            PersistAs = Persistence.PropertyPersistenceLocation.ValueAccessor;
        }

        public string ZoneName
        {
            get { return zoneName; }
            set { zoneName = value; }
        }

		public string MinimumTypeName { get; set; }

		public string[] AllowedTemplateKeys { get; set; }

		public override bool UpdateItem(ContentItem item, Control editor)
        {
            bool updated = false;
            ItemEditorList listEditor = (ItemEditorList)editor;
            for (int i = 0; i < listEditor.ItemEditors.Count; i++)
            {
                ItemEditor childEditor = listEditor.ItemEditors[i];
                ItemEditor parentEditor = ItemUtility.FindInParents<ItemEditor>(editor.Parent);

                var childItem = childEditor.CurrentItem;
                if (childItem.ID != 0 && item.ID == 0 && !childItem.IsPage)
                    // we may have initialized the editor with the published version but we want to use the draft here
                    childItem = item.FindPartVersion(childItem);

                if (listEditor.DeletedIndexes.Contains(i))
                {
                    if (childItem.ID == 0)
                        childItem.AddTo(null);
                    else
                        Engine.Persister.Delete(childItem);
                }
                else
                {
                    if (parentEditor != null)
                    {
                        var subContext = parentEditor.BinderContext.CreateNestedContext(childEditor, childItem, childEditor.GetDefinition());
                        if (subContext.Binder.UpdateObject(subContext))
                        {
                            childItem.AddTo(item); // make sure it's on parent's child collection
                            parentEditor.BinderContext.RegisterItemToSave(childItem);
                            updated = true;
                        }
                    }
                    else
                    {
                        IItemEditor fallbackEditor = ItemUtility.FindInParents<IItemEditor>(editor.Parent);
                        if (fallbackEditor != null)
                        {
                            fallbackEditor.Saved += delegate
                            {
                                var cc = childEditor.CreateCommandContext();
                                Engine.Resolve<CommandDispatcher>().Publish(cc);
                            };
                        }
                    }
                }
            }
            return updated || listEditor.DeletedIndexes.Count > 0 || listEditor.AddedDefinitions.Count > 0;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            ItemEditorList listEditor = (ItemEditorList)editor;
            listEditor.ParentItem = item;
            listEditor.ZoneName = ZoneName ?? Name;

            // filtering of children by property generic type
            PropertyInfo info = item.GetContentType().GetProperty(Name);
            if (info != null)
            {
                foreach (Type argument in info.PropertyType.GetGenericArguments())
                {
                    if (typeof(ContentItem).IsAssignableFrom(argument) || argument.IsInterface)
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
			if (!string.IsNullOrEmpty(MinimumTypeName))
				listEditor.MinimumType = Type.GetType(MinimumTypeName);
			listEditor.AllowedTemplateKeys = AllowedTemplateKeys;
            listEditor.ParentItem = ItemUtility.FindInParents<IItemEditor>(container).CurrentItem;
            listEditor.Label = Title;
            container.Controls.Add(listEditor);
            return listEditor;
        }

        public object GetValue(ValueAccessorContext context, string propertyName)
        {
            return context.Instance.GetChildren(ZoneName ?? Name ?? propertyName)
                .ConvertTo(context.Property.PropertyType, propertyName);
        }

        public bool SetValue(ValueAccessorContext context, string propertyName, object value)
        {
            context.Instance.SetChildren(ZoneName ?? Name ?? propertyName, value as IEnumerable);
            return value != null;
        }
    }
}
