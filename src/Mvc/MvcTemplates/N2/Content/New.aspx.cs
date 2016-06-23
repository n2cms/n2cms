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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Collections;
using N2.Definitions;
using N2.Integrity;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    [NavigationLinkPlugin("New", "new", "{ManagementUrl}/Content/New.aspx?{Selection.SelectedQueryKey}={selected}", Targets.Preview, "{ManagementUrl}/Resources/icons/add.png", 10,
        GlobalResourceClassName = "Navigation",
        RequiredPermission = Permission.Write,
        IconClass = "fa fa-plus-circle",
        Legacy = true)]
    [ToolbarPlugin("", "new_tool", "{ManagementUrl}/Content/New.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Operations, Targets.Preview, "{ManagementUrl}/Resources/icons/add.png", 40, ToolTip = "new",
        GlobalResourceClassName = "Toolbar",
        RequiredPermission = Permission.Write,
        Legacy = true)]
    [ControlPanelLink("cpNew", "{ManagementUrl}/Resources/icons/add.png", "{ManagementUrl}/Content/New.aspx?{Selection.SelectedQueryKey}={Selected.Path}", "New item one level down from this page", 40, ControlPanelState.Visible,
        CssClass = "complementary",
        RequiredPermission = Permission.Write,
        IconClass = "fa fa-plus-circle",
		Legacy = true)]
    public partial class New : Web.EditPage
    {
        ItemDefinition ParentItemDefinition = null;
        protected string ZoneName = null;
        protected IDefinitionManager Definitions;
        protected IList<ItemDefinition> AvailableDefinitions = new List<ItemDefinition>();

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
            Definitions = Engine.Definitions;
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ParentItemDefinition = Definitions.GetDefinition(ActualItem);
            if (!IsPostBack)
            {
                LoadZones();
            }
            ZoneName = GetSelectedZone();
        }

        protected void rblPosition_OnSelectedIndexChanged(object sender, EventArgs args)
        {
            ParentItemDefinition = Definitions.GetDefinition(ActualItem);
            LoadZones();
            ZoneName = GetSelectedZone();
        }

        protected void rblZone_OnSelectedIndexChanged(object sender, EventArgs args)
        {
            ZoneName = GetSelectedZone();
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
            string format = string.IsNullOrEmpty(item.IconClass)
                ? "<a href='{2}' title='{1}'><img src='{0}' alt='{3}'/></a> {1} "
                : "<a href='{2}' title='{1}'><b class='{5}'></b></a> {1} ";
            if (isCurrent)
                format = "<strong>" + format + "</strong>";

            return string.Format(format, ResolveUrl(item.IconUrl), HtmlSanitizer.Current.Clean(item.Title), item.Url, "icon", "current", item.IconClass);
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
            int allowedChildrenCount = ParentItemDefinition.GetAllowedChildren(Definitions, Selection.SelectedItem).Count();
            IList<ItemDefinition> allowedChildren = Definitions.GetAllowedChildren(Selection.SelectedItem, ZoneName)
                .WhereAuthorized(Engine.SecurityManager, User, Selection.SelectedItem)
                .ToList();

            if(!IsAuthorized(Permission.Write))
            {
                cvPermission.IsValid = false;
                return;
            }

            if (allowedChildrenCount == 0)
            {
                Title = string.Format(GetLocalResourceString("NewPage.Title.NoneAllowed", "No item is allowed below an item of type \"{0}\""), ParentItemDefinition.Title);
            }
            else if (allowedChildrenCount == 1 && allowedChildren.Count == 1)
            {
                Response.Redirect(GetEditUrl(allowedChildren[0]));
            }
            else
            {
                Title = string.Format(GetLocalResourceString("NewPage.Title.Select", "Select type of item below \"{0}\""), ActualItem.Title);

                var top = allowedChildren.OrderByDescending(d => d.NumberOfItems).ThenBy(d => d.SortOrder).Take(1).ToList();
                var rest = allowedChildren.Except(top).ToList();

                AvailableDefinitions = top.Union(rest).ToList();
            }
        }

        public IEnumerable<TemplateDefinition> GetTemplates(ItemDefinition definition)
        {
			return Engine.Resolve<ITemplateAggregator>().GetTemplates(definition.ItemType)
				.WhereAllowed(Selection.SelectedItem, ZoneName, User, Engine.Definitions, Engine.SecurityManager);
        }

        private void LoadZones()
        {
            string selectedZone = rblZone.SelectedValue;
            ListItem initialItem = rblZone.Items[0];

            rblZone.Items.Clear();
            rblZone.Items.Insert(0, initialItem);
            foreach (AvailableZoneAttribute zone in ParentItemDefinition.AvailableZones)
            {
                string title = GetLocalizedString("Zones", zone.ZoneName) ?? zone.Title;
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

        protected string GetLocalizedString(string classKey, string discriminator, string key)
        {
            return Utility.GetGlobalResourceString(classKey, discriminator + "." + key);
        }

        protected string GetLocalizedString(string classKey, string key)
        {
            return Utility.GetGlobalResourceString(classKey, key);
        }

        private string GetSelectedZone()
        {
            return string.IsNullOrEmpty(rblZone.SelectedValue) ? null : rblZone.SelectedValue;
        }
    }
    
}
