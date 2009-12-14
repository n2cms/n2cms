using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Integrity;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// A control that displays items in it's zone and supports rearranging of the items via drag and drop. 
    /// </summary>
	public class DroppableZone : Zone
	{
        /// <summary>Set to true if parts that have been added to another page (and are displayed elsewhere) may be moved on other pages.</summary>
        public bool AllowExternalManipulation { get; set; }
		ControlPanelState state = ControlPanelState.Hidden;

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
            state = ControlPanel.GetState(Page);
            if (state == ControlPanelState.DragDrop && (AllowExternalManipulation || CurrentItem == CurrentPage))
			{
				if (ZoneName.IndexOfAny(new[] {'.', ',', ' ', '\'', '"', '\t', '\r', '\n'}) >= 0) throw new N2Exception("Zone '" + ZoneName + "' contains illegal characters.");

                Panel zoneContainer = AddPanel(this, ZoneName + " dropZone");
                zoneContainer.Attributes["item"] = CurrentItem.Path;
                zoneContainer.Attributes["zone"] = ZoneName;
                zoneContainer.Attributes["allowed"] = GetAllowedNames(ZoneName, PartsAdapter.GetAllowedDefinitions(CurrentItem, ZoneName, Page.User));
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
            if (state == ControlPanelState.DragDrop && IsMovableOnThisPage(item))
			{
				ItemDefinition definition = GetDefinition(item);
				Panel itemContainer = AddPanel(container, "zoneItem " + definition.Discriminator);
                itemContainer.Attributes["item"] = item.Path;
                itemContainer.Attributes["type"] = definition.Discriminator;
				Control toolbar = AddToolbar(itemContainer, item, definition);
				base.AddChildItem(itemContainer, item);
			}
			else if (state == ControlPanelState.Previewing && item.ID.ToString() == Page.Request["original"])
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
			return N2.Context.Definitions.GetDefinition(item.GetType());
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

		public static string GetAllowedNames(string zoneName, IEnumerable<ItemDefinition> definitions)
		{
			List<string> allowedDefinitions = new List<string>();
			foreach (ItemDefinition potentialChild in definitions)
			{
				allowedDefinitions.Add(potentialChild.Discriminator);
			}
			return string.Join(",", allowedDefinitions.ToArray());
		}
	}
}
