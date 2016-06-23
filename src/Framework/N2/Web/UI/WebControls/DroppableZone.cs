using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Integrity;
using N2.Edit.Workflow;
using N2.Persistence;
using N2.Edit.Versioning;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// A control that displays items in it's zone and supports rearranging of the items via drag and drop. 
    /// </summary>
    public class DroppableZone : Zone
    {
        /// <summary>Set to true if parts that have been added to another page (and are displayed elsewhere) may be moved on other pages.</summary>
        public bool AllowExternalManipulation { get; set; }
        ControlPanelState? state;

        protected ControlPanelState State
        {
            get { return state ?? (state = ControlPanel.GetState(Page)) ?? ControlPanelState.Hidden; }
            set { state = value; }
        }

        public string DropPointBackImageUrl
        {
            get { return (string)(ViewState["DropPointBackImageUrl"] ?? string.Empty); }
            set { ViewState["DropPointBackImageUrl"] = value; }
        }

        public string GripperImageUrl
        {
            get { return (string)(ViewState["GripperImageUrl"] ?? string.Empty); }
            set { ViewState["GripperImageUrl"] = value; }
        }

        protected override void CreateItems(Control container)
        {
            if (State.IsFlagSet(ControlPanelState.DragDrop) && (AllowExternalManipulation || CurrentItem == CurrentPage || Find.ClosestPage(CurrentItem) == CurrentPage))
            {
                if (ZoneName.IndexOfAny(new[] {'.', ',', ' ', '\'', '"', '\t', '\r', '\n'}) >= 0) throw new N2Exception("Zone '" + ZoneName + "' contains illegal characters.");

                Panel zoneContainer = AddPanel(this, ZoneName + " dropZone");
				if (CurrentItem.ID != 0 && !CurrentItem.VersionOf.HasValue)
				{
					zoneContainer.Attributes["data-id"] = CurrentItem.ID.ToString();
					zoneContainer.Attributes[PartUtilities.PathAttribute] = CurrentItem.Path;
				}
				else
				{
					zoneContainer.Attributes[PartUtilities.PathAttribute] = Find.ClosestPage(CurrentItem).Path;
					zoneContainer.Attributes["data-versionKey"] = CurrentItem.GetVersionKey();
					zoneContainer.Attributes["data-versionIndex"] = CurrentItem.VersionIndex.ToString();
				}
                zoneContainer.Attributes[PartUtilities.ZoneAttribute] = ZoneName;
                zoneContainer.Attributes[PartUtilities.AllowedAttribute] = PartUtilities.GetAllowedNames(ZoneName, PartsAdapter.GetAllowedDefinitions(CurrentItem, ZoneName, Page.User));
                
                zoneContainer.ToolTip = GetToolTip(GetDefinition(CurrentItem), ZoneName);
                base.CreateItems(zoneContainer);
            }
            else
            {
                base.CreateItems(this);
            }
        }

        protected override void AddChildItem(Control container, ContentItem item)
        {
            if (State.IsFlagSet(ControlPanelState.DragDrop) && IsMovableOnThisPage(item))
            {
                string preview = Page.Request.QueryString["preview"];
                if (!string.IsNullOrEmpty(preview))
                {
                    int previewIndex;
                    int publishedItemID;
                    if (int.TryParse(preview, out previewIndex) && int.TryParse(Page.Request.QueryString[PathData.ItemQueryKey], out publishedItemID))
                        if (publishedItemID == item.ID)
                            item = Engine.Resolve<IVersionManager>().GetVersion(item, previewIndex);
                }

                ItemDefinition definition = GetDefinition(item);
                Panel itemContainer = AddPanel(container, "zoneItem " + definition.Discriminator);
				if (CurrentItem.ID != 0 && !CurrentItem.VersionOf.HasValue)
				{
					itemContainer.Attributes["data-id"] = item.ID.ToString();
					itemContainer.Attributes[PartUtilities.PathAttribute] = item.Path;
				}
				else
				{
					itemContainer.Attributes[PartUtilities.PathAttribute] = Find.ClosestPage(item).Path;
					itemContainer.Attributes["data-versionKey"] = item.GetVersionKey();
					itemContainer.Attributes["data-versionIndex"] = item.VersionIndex.ToString();
				}
                itemContainer.Attributes[PartUtilities.TypeAttribute] = definition.Discriminator;
                itemContainer.Attributes["data-sortOrder"] = item.SortOrder.ToString();
                
                Control toolbar = AddToolbar(itemContainer, item, definition);
                base.AddChildItem(itemContainer, item);
            }
            else if (State.IsFlagSet(ControlPanelState.Previewing) && item.ID.ToString() == Page.Request["original"])
            {
                item = Engine.Persister.Get(int.Parse(Page.Request["preview"]));
                base.AddChildItem(this, item);
            }
            else
            {
                base.AddChildItem(this, item);
            }
        }

        private bool IsMovableOnThisPage(ContentItem item)
        {
            return AllowExternalManipulation || item.Parent == CurrentPage;
        }

        protected virtual Control AddToolbar(Panel itemContainer, ContentItem item, ItemDefinition definition)
        {
            DraggableToolbar toolbar = new DraggableToolbar(item, definition);
            itemContainer.Controls.Add(toolbar);
            return toolbar;
        }

        private ItemDefinition GetDefinition(ContentItem item)
        {
            return N2.Context.Definitions.GetDefinition(item);
        }

        public static string GetToolTip(ItemDefinition definition, string zoneName)
        {
            foreach (AvailableZoneAttribute za in definition.AvailableZones)
            {
                if(za.ZoneName == zoneName)
                {
                    return za.Title;
                }
            }
            return zoneName;
        }

        private Panel AddPanel(Control container, string className)
        {
            Panel p = new Panel();
            p.CssClass = className;
            container.Controls.Add(p);
            return p;
        }

        protected override string GetInterface()
        {
            return State.GetInterface();
        }
    }
}
