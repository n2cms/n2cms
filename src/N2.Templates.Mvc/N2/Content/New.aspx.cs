#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Integrity;
using N2.Web.UI.WebControls;
using N2.Security;
using N2.Web;
using N2.Collections;
using System.Web.UI;

namespace N2.Edit
{
	[NavigationLinkPlugin("New", "new", "Content/New.aspx?selected={selected}", Targets.Preview, "{ManagementUrl}/Resources/icons/add.png", 10, GlobalResourceClassName = "Navigation")]
	[ToolbarPlugin("NEW", "new", "Content/New.aspx?selected={selected}", ToolbarArea.Operations, Targets.Preview, "{ManagementUrl}/Resources/icons/add.png", 40, ToolTip = "new", GlobalResourceClassName = "Toolbar")]
    [ControlPanelLink("cpNew", "{ManagementUrl}/Resources/icons/add.png", "Content/New.aspx?selected={Selected.Path}", "New item one level down from this page", 40, ControlPanelState.Visible)]
	public partial class New : Web.EditPage
    {
		ItemDefinition ParentItemDefinition = null;
		protected string ZoneName = null;
		protected IDefinitionManager Definitions;
		protected IContentTemplateRepository Templates;

		public ContentItem ActualItem
		{
			get
			{
				if(rblPosition.SelectedIndex == 1)
                    return Selection.SelectedItem;
				else
                    return Selection.SelectedItem.Parent;
			}
		}

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			Definitions = Engine.Resolve<IDefinitionManager>();
			Templates = Engine.Resolve<IContentTemplateRepository>();
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (Selection.SelectedItem.Parent == null)
			{
				rblPosition.Enabled = false;
			}
			else
			{
				rblPosition.Items[0].Text = "Create new item before: " + BuildHierarchy(Selection.SelectedItem, CreationPosition.Before);
				rblPosition.Items[1].Text = "Create new item below: " + BuildHierarchy(Selection.SelectedItem, CreationPosition.Below);
			}
        }

        protected void rptTypes_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex == 0)
			{
				Control hlNew = e.Item.FindControl("hlNew");
				if(hlNew != null)
					hlNew.Focus();
			}
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			ParentItemDefinition = Definitions.GetDefinition(ActualItem.GetContentType());
			if (!IsPostBack)
			{
				LoadZones();
			}
			ZoneName = rblZone.SelectedValue;
        }

		protected void rblPosition_OnSelectedIndexChanged(object sender, EventArgs args)
		{
			ParentItemDefinition = Definitions.GetDefinition(ActualItem.GetContentType());
			LoadZones();
			ZoneName = rblZone.SelectedValue;
		}

		protected void rblZone_OnSelectedIndexChanged(object sender, EventArgs args)
		{
			ZoneName = rblZone.SelectedValue;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			
			LoadAllowedTypes();
		}

		private string BuildHierarchy(ContentItem selected, CreationPosition position)
		{
			var filter = Engine.EditManager.GetEditorFilter(User);
			var siblings = Find.EnumerateSiblings(selected, 1, 1);
			HierarchyNode<string> root = new HierarchyNode<string>(null);
			foreach (var sibling in filter.Pipe(siblings))
			{
				if (sibling == selected)
				{
					var node = new HierarchyNode<string>(GetNodeText(sibling, true));
					if (position == CreationPosition.Before)
						root.Children.Add(new HierarchyNode<string>("<a href='#'><img src='../Resources/icons/add.png' alt='add'/></a> New item"));

					ContentItem first = First(sibling.Children, filter);
					if (first != null)
					{
						node.Children.Add(new HierarchyNode<string>(GetNodeText(first, false)));
						ContentItem last = Last(sibling.Children, filter);
						if (last != null)
						{
							node.Children.Add(new HierarchyNode<string>("..."));
							node.Children.Add(new HierarchyNode<string>(GetNodeText(last, false)));
						}
					}
					if (position == CreationPosition.Below)
						node.Children.Add(new HierarchyNode<string>("<a href='#'><img src='../Resources/icons/add.png' alt='add'/></a> New item"));

					root.Children.Add(node);
					if (position == CreationPosition.After)
						root.Children.Add(new HierarchyNode<string>("<a href='#'><img src='../Resources/icons/add.png' alt='add'/></a> New item"));
				}
				else
					root.Children.Add(new HierarchyNode<string>(GetNodeText(sibling, false)));
			}
			return root.ToString((c) => c == null ? "" : "<span>" + c, (p) => "<span class='indent'>", (p) => "</span>", (c) => c == null ? "" : "</span>");
		}

		private string GetNodeText(ContentItem item, bool isCurrent)
		{
			string format = "<a href='{2}' title='{1}'><img src='{0}' alt='{3}'/></a> {1} ";
			if (isCurrent)
				format = "<strong>" + format + "</strong>";

			return string.Format(format, ResolveUrl(item.IconUrl), item.Title, item.Url, "icon", "current");
		}

		private static ContentItem Last(IList<ContentItem> children, ItemFilter filter)
		{
			for (int i = children.Count - 1; i >= 0; i--)
			{
				if (!filter.Match(children[i]))
					continue;

				return children[i];
			}
			return null;
		}

		private static ContentItem First(IEnumerable<ContentItem> children, ItemFilter filter)
		{
			foreach (var child in filter.Pipe(children))
			{
				return child;
			}
			return null;
		}

		private void LoadAllowedTypes()
		{
			int allowedChildrenCount = ParentItemDefinition.AllowedChildren.Count;
			IList<ItemDefinition> allowedChildren = Definitions.GetAllowedChildren(ParentItemDefinition, ZoneName, this.User);

			if(!IsAuthorized(Permission.Write))
			{
				cvPermission.IsValid = false;
				return;
			}

			if (allowedChildrenCount == 0)
			{
				Title = string.Format(GetLocalResourceString("NewPage.Title.NoneAllowed"), ParentItemDefinition.Title);
			}
			else if (allowedChildrenCount == 1 && allowedChildren.Count == 1)
			{
                Response.Redirect(GetEditUrl(allowedChildren[0]));
			}
			else
			{
				Title = string.Format(GetLocalResourceString("NewPage.Title.Select"), ActualItem.Title);

				var mostPopular = allowedChildren.Where(d => d.NumberOfItems > 0).OrderByDescending(d => d.NumberOfItems).Take(2).ToList();
				allowedChildren = mostPopular.Union(allowedChildren).ToList();

				rptTypes.DataSource = allowedChildren;
				rptTypes.DataBind();
			}
		}

		private void LoadZones()
		{
			string selectedZone = rblZone.SelectedValue;
			ListItem initialItem = rblZone.Items[0];

			rblZone.Items.Clear();
			rblZone.Items.Insert(0, initialItem);
			foreach (AvailableZoneAttribute zone in ParentItemDefinition.AvailableZones)
			{
				string title = GetZoneString(zone.ZoneName) ?? zone.Title;
				rblZone.Items.Add(new ListItem(title, zone.ZoneName));
			}

			string z = IsPostBack ? selectedZone : Request.QueryString["zoneName"];
			if(rblZone.Items.FindByValue(z) != null)
				rblZone.SelectedValue = z;
		}

		protected CreationPosition GetCreationPosition()
		{
			if (rblPosition.SelectedIndex == 0)
				return CreationPosition.Before;
			else if (rblPosition.SelectedIndex == 2)
				return CreationPosition.After;
			else
				return CreationPosition.Below;
				
		}

        protected string GetEditUrl(ItemDefinition definition)
        {
            Url newUrl = Engine.ManagementPaths.GetEditNewPageUrl(Selection.SelectedItem, definition, ZoneName, GetCreationPosition());
            return newUrl.AppendQuery("returnUrl", Request["returnUrl"]);
        }

		protected string GetDefinitionString(ItemDefinition definition, string key)
		{
			return Utility.GetGlobalResourceString("Definitions", definition.Discriminator + "." + key);
		}

		protected string GetZoneString(string key)
		{
			return Utility.GetGlobalResourceString("Zones", key);
		}
    }
    
}