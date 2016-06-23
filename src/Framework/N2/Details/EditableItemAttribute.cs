using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Web.UI;
using N2.Definitions;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Web;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Edit.Versioning;
using N2.Persistence.Proxying;
using N2.Web.Parts;

namespace N2.Details
{
    /// <summary>
    /// Defines an editable item. The edited item is referenced by the 
    /// property decorated with this attribute. If the property is null a new
    /// item is created and added to the parent items child collection.
    /// </summary>
    /// <example>
    ///     public class ParentItem : N2.ContentItem
    ///     {
    ///         [N2.Details.EditableItem]
    ///         public virtual ChildItem News { get; set; }
    ///     }
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableItemAttribute : AbstractEditableAttribute, IDisplayable, IWritingDisplayable, IValueAccessor
    {
        #region Fields

        private string defaultChildZoneName;
        private string defaultChildName;

        #endregion

        #region Constructors

        public EditableItemAttribute()
            : this(null, null, 200)
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
            PersistAs = PropertyPersistenceLocation.ValueAccessor;
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

        protected virtual ContentActivator Activator
        {
            get { return Context.Current.Resolve<ContentActivator>(); }
        }

        #endregion

        #region Methods

        public override bool UpdateItem(ContentItem parentItem, Control editor)
        {
            ItemEditor childEditor = editor as ItemEditor;
            ItemEditor parentEditor = ItemUtility.FindInParents<ItemEditor>(editor.Parent);

            var childItem = childEditor.CurrentItem;
            if (childItem.ID != 0 && parentItem.ID == 0)
                // we may have initialized the editor with the published version but we want to use the draft here
                childItem = parentItem.FindPartVersion(childItem);

            if (childEditor.UpdateObject(parentEditor.BinderContext.CreateNestedContext(childEditor, childItem, childEditor.GetDefinition())))
            {
                parentItem[Name] = childItem;
                return true;
            }
            
            return false;
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
			parentEditor.CreatingContext += (s, cc) => cc.RegisterItemToSave(itemEditor.CurrentItem);
        }


		protected virtual ContentItem GetChild(ContentItem item)
        {
            ContentItem childItem = Utility.GetProperty(item, Name) as ContentItem;
            if (childItem == null)
            {
                PropertyInfo pi = item.GetContentType().GetProperty(Name);

                if (pi == null)
                    throw new N2Exception("The item should have had a property named '{0}'", Name);
                if (!typeof(ContentItem).IsAssignableFrom(pi.PropertyType))
                    throw new N2Exception("The property type '" + pi.PropertyType + "' is not assignable to N2.ContentItem which is required by the [EditableItem] attribute.");
                if (pi.PropertyType.IsAbstract)
                    throw new N2Exception("The property type '" + pi.PropertyType + "' is abstract");

                childItem = CreateChild(item, pi.PropertyType);

                pi.SetValue(item, childItem, null);
            }
            return childItem;
        }

        protected virtual ContentItem CreateChild(ContentItem item, Type childItemType)
        {
            try
            {
				ContentItem child = Activator.CreateInstance(childItemType, item);
				child.Name = DefaultChildName;
				child.ZoneName = DefaultChildZoneName;
				return child;
			}
            catch (KeyNotFoundException ex)
            {
                N2.Engine.Logger.WarnFormat("EditableItemAttribute.CreateChild: No item of the type {0} was found among the item definitions: {0}", childItemType);
                throw new N2Exception(
                    string.Format(
                        "No item of the type {0} was found among item definitions. This could happen if the referenced item type an abstract class or a class that doesn't inherit from N2.ContentItem.",
                        childItemType),
                    ex);
            }
        }

        #endregion

        #region IDisplayable Members

        Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
        {
            ContentItem linkedItem = item[detailName] as ContentItem;
            if (linkedItem != null)
            {
                if (linkedItem.IsPage)
                    return DisplayableAnchorAttribute.GetLinkBuilder(item, linkedItem, detailName, null, null).AddTo(container);

                return Engine.GetContentAdapter<PartsAdapter>(item).AddChildPart(linkedItem, container);
                //return ItemUtility.AddUserControl(container, linkedItem);
            }
            return null;
        }

        #endregion

        #region IWritingDisplayable Members

        public void Write(ContentItem item, string detailName, System.IO.TextWriter writer)
        {
            ContentItem linkedItem = item[detailName] as ContentItem;
            if (linkedItem != null && linkedItem.IsPage)
            {
                DisplayableAnchorAttribute.GetLinkBuilder(item, linkedItem, detailName, null, null).WriteTo(writer);
            }
        }

        #endregion

        public object GetValue(ValueAccessorContext context, string propertyName)
        {
            return context.Instance.GetChild(DefaultChildName ?? propertyName);
        }

        public bool SetValue(ValueAccessorContext context, string propertyName, object value)
        {
            var item = context.Instance;
            var existing = item.GetChild(DefaultChildName ?? propertyName);
            if (existing == null && value != null)
            {
                item.SetChild(DefaultChildName ?? propertyName, value);
                if (!string.IsNullOrEmpty(DefaultChildZoneName))
                    ((ContentItem)value).ZoneName = DefaultChildZoneName;
                return true;
            }
            return false;
        }
    }
}
