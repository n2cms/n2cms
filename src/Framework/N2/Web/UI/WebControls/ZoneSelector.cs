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

namespace N2.Web.UI.WebControls
{
    /// <summary>A drop down list that allows selection between available zones on the parent item.</summary>
    public class ZoneSelector : System.Web.UI.WebControls.DropDownList
    {
        protected virtual Definitions.IDefinitionManager Definitions
        {
            get { return N2.Context.Definitions; }
        }

        /// <summary>Initializes the zone selector control.</summary>
        protected override void OnInit(EventArgs e)
        {
            this.CssClass = "ZoneSelector";
            base.OnInit(e);

            ContentItem item = ItemUtility.FindCurrentItem(this.Parent);
            if (item != null && item.Parent != null)
            {
                N2.Definitions.ItemDefinition definition = N2.Context.Definitions.GetDefinition(item.Parent);
                this.DataSource = definition.AvailableZones;
                this.DataTextField = "Title";
                this.DataValueField = "ZoneName";
                this.DataBind();
                this.Items.Insert(0, "");

                string defaultValue = this.Page.Request.QueryString["zoneName"];
                if (!string.IsNullOrEmpty(defaultValue) && this.Items.FindByValue(defaultValue) != null)
                    this.SelectedValue = defaultValue;
            }
        }
    }
}
