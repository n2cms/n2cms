using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit.Workflow;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// Used internally to add the save changes during on page editing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ControlPanelEditingSaveAttribute : ControlPanelLinkAttribute
    {
        public ControlPanelEditingSaveAttribute(string toolTip, int sortOrder)
            : base("cpEditingSave", "{ManagementUrl}/Resources/icons/disk_green.png", null, toolTip, sortOrder, ControlPanelState.Editing)
        {
            IconClass = "icon-save";
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            if(!ActiveFor(container, context.State))
                return null;

            LinkButton btn = new LinkButton();
            btn.Text = GetInnerHtml(context, IconUrl, ToolTip, Title);
            btn.ToolTip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? context.Format(ToolTip, false);
            btn.CssClass = "save";
            container.Controls.Add(btn);
            btn.Command += delegate
                {
                    IList<IItemEditor> itemEditors = GetEditedItems(container.Page);

                    foreach (IItemEditor itemEditor in itemEditors)
                    {
                        var definition = Engine.Definitions.GetDefinition(itemEditor.CurrentItem);
                        Engine.Resolve<CommandDispatcher>().Publish(
                            new CommandContext(
                                definition,
                                itemEditor.CurrentItem,
                                Interfaces.Viewing,
                                container.Page.User,
                                new EditorCollectionBinder(definition, itemEditor.AddedEditors),
                                new NullValidator<CommandContext>()));
                    }

                    RedirectTo(container.Page, context.Selected);
                };
            return btn;
        }

        protected virtual IList<IItemEditor> GetEditedItems(Page page)
        {
            Dictionary<ContentItem, IDictionary<string, Control>> itemsEditors = new Dictionary<ContentItem, IDictionary<string, Control>>();

            IEnumerable<IEditableEditor> editors = ItemUtility.FindInChildren<IEditableEditor>(page);
            foreach (EditableDisplay ed in editors)
            {
                if (!itemsEditors.ContainsKey(ed.CurrentItem))
                {
                    itemsEditors[ed.CurrentItem] = new Dictionary<string, Control>();
                }
                itemsEditors[ed.CurrentItem][ed.PropertyName] = ed.Editor;
            }

            IList<IItemEditor> items = new List<IItemEditor>();
            foreach (ContentItem item in itemsEditors.Keys)
            {
                items.Add(new OnPageItemEditor(ItemEditorVersioningMode.VersionAndSave, item.ZoneName, itemsEditors[item], item));
            }
            return items;
        }

        protected void RedirectTo(Page page, ContentItem item)
        {
            string url = page.Request["returnUrl"];
            if (string.IsNullOrEmpty(url))
                url = Engine.GetContentAdapter<NodeAdapter>(item).GetPreviewUrl(item);

            page.Response.Redirect(url);
        }


        #region class OnPageItemEditor

        private class OnPageItemEditor : IItemEditor
        {
            public OnPageItemEditor(ItemEditorVersioningMode versioningMode, string zoneName,
                                    IDictionary<string, Control> addedEditors, ContentItem currentItem)
            {
                this.versioningMode = versioningMode;
                this.zoneName = zoneName;
                this.addedEditors = addedEditors;
                this.currentItem = currentItem;
            }

            #region IItemEditor Members

            private ItemEditorVersioningMode versioningMode = ItemEditorVersioningMode.VersionAndSave;
            private string zoneName = string.Empty;
            private readonly IDictionary<string, Control> addedEditors = new Dictionary<string, Control>();
            private ContentItem currentItem;

            public ItemEditorVersioningMode VersioningMode
            {
                get { return versioningMode; }
                set { versioningMode = value; }
            }

            public string ZoneName
            {
                get { return zoneName; }
                set { zoneName = value; }
            }

            public IDictionary<string, Control> AddedEditors
            {
                get { return addedEditors; }
            }

            #endregion

            #region IItemContainer Members

            public ContentItem CurrentItem
            {
                get { return currentItem; }
                set { currentItem = value; }
            }

            #endregion

            #region IItemEditor Members

            public event EventHandler<ItemEventArgs> Saved = delegate {};
			public event Action<object, CommandContext> CreatingContext;

			#endregion
		}

        #endregion

        public class EditorCollectionBinder : IContentForm<CommandContext>
        {
            ItemDefinition definition;
            IDictionary<string, Control> editors;

            public EditorCollectionBinder(ItemDefinition definition, IDictionary<string, Control> editors)
            {
                this.definition = definition;
                this.editors = editors;
            }

            #region IBinder<CommandContext> Members

            public bool UpdateObject(CommandContext value)
            {
                bool wasUpdated = false;
                foreach (var kvp in editors)
                {
                    var editable = definition.GetContainable(kvp.Key) as IEditable;
                    if (editable != null)
                    {
                        wasUpdated |= editable.UpdateItem(value.Content, kvp.Value);
                    }
                }
                return wasUpdated;
            }

            public void UpdateInterface(CommandContext value)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
