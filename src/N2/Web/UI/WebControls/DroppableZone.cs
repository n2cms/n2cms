using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Integrity;
using N2.Web.UI.WebControls;

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
			state = ControlPanel.GetState();
            if (state == ControlPanelState.DragDrop && (AllowExternalManipulation || CurrentItem == CurrentPage))
			{
				if (ZoneName.IndexOfAny(new[] {'.', ',', ' ', '\'', '"', '\t', '\r', '\n'}) >= 0) throw new N2Exception("Zone '" + ZoneName + "' contains illegal characters.");

                Panel zoneContainer = AddPanel(this, ZoneName + " dropZone");
				base.CreateItems(zoneContainer);
				AddDropPoint(zoneContainer, CurrentItem, CreationPosition.Below);

				string allowed = GetAllowed(ZoneName);
				RegisterArray("dropZones", "{{selector: '.{0}.dropPoint', accept: '{1}'}}", ZoneName, allowed);
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
				AddDropPoint(container, item, CreationPosition.Before);

				ItemDefinition definition = GetDefinition(item);
				Panel itemContainer = AddPanel(container, "zoneItem " + definition.Discriminator);
				RegisterArray("dragItems", string.Format("{{dragKey:'{0}',item:{1}}}", itemContainer.ClientID, item.ID));

				AddToolbar(itemContainer, item, definition);
				base.AddChildItem(itemContainer, item);
			}
			else if (state == ControlPanelState.Previewing && item.ID.ToString() == Page.Request["original"])
			{
				item = Engine.Persister.Get(int.Parse(Page.Request["preview"]));
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

	    protected virtual void AddToolbar(Panel itemContainer, ContentItem item, ItemDefinition definition)
		{
			itemContainer.Controls.Add(new DraggableToolbar(item, definition, GripperImageUrl));
		}

		private ItemDefinition GetDefinition(ContentItem item)
		{
			return N2.Context.Definitions.GetDefinition(item.GetType());
		}

		private DropPoint AddDropPoint(Control container, ContentItem item, CreationPosition position)
		{
			DropPoint dp = new DropPoint(ZoneName, item, position, DropPointBackImageUrl);
			SetToolTip(dp, position == CreationPosition.Below ? item : item.Parent);
			container.Controls.Add(dp);
			return dp;
		}

		private void SetToolTip(DropPoint dp, ContentItem item)
		{
			ItemDefinition definition = GetDefinition(item);
			foreach (AvailableZoneAttribute za in definition.AvailableZones)
			{
				if(za.ZoneName == ZoneName)
				{
                    dp.ToolTip = ZoneName;
                    dp.Text = za.Title;
				}
			}
		}

		private Panel AddPanel(Control container, string className)
		{
			Panel p = new Panel();
			p.CssClass = className;
			container.Controls.Add(p);
			return p;
		}

		private string GetAllowed(string zone)
		{
			List<string> allowedDefinitions = new List<string>();
			foreach (ItemDefinition potentialChild in N2.Context.Definitions.GetDefinitions())
			{
				if (potentialChild.IsAllowedInZone(zone))
				{
					allowedDefinitions.Add("." + potentialChild.Discriminator);
				}
			}
			return string.Join(",", allowedDefinitions.ToArray());
		}


		private void RegisterArray(string arrayName, string arrayValue)
		{
			Page.ClientScript.RegisterArrayDeclaration(arrayName, arrayValue);
		}

		private void RegisterArray(string arrayName, string arrayValueFormat, params object[] values)
		{
			Page.ClientScript.RegisterArrayDeclaration(arrayName, string.Format(arrayValueFormat, values));
		}
	}
}
