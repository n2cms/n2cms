#region License
/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */
#endregion

using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using N2.Web;
using N2.Edit.Settings;

namespace N2.Edit.Web
{
    public class EditSiteMapProvider : N2.Web.PublicSiteMapProvider
    {
		public EditSiteMapProvider()
		{
			this.Settings = Engine.Resolve<NavigationSettings>();
		}

		private NavigationSettings settings;

		protected NavigationSettings Settings
		{
			get { return settings; }
			set { settings = value; }
		}

        public bool DisplayDataItems
        {
			get { return Settings.DisplayDataItems; }
		}

		protected static Engine.IEngine Engine
		{
			get { return N2.Context.Current; }
		}

        protected override SiteMapNode Convert(ContentItem item)
        {
            if(item!=null)
                return new EditSiteMapNode(this, (ContentItem)item);
            return null;
        }

        protected override bool IsDisplayable(ContentItem item)
        {
            return DisplayDataItems || item.IsPage;
        }

		public override SiteMapNode FindSiteMapNode(string rawUrl)
		{
			string[] pageQueryPair = rawUrl.Split('?', '#');
			if (pageQueryPair.Length > 1)
			{
				NameValueCollection queryString = HttpUtility.ParseQueryString(pageQueryPair[1]);
				if (!string.IsNullOrEmpty(queryString["selected"]))
					return base.FindSiteMapNode(queryString["selected"]);
			}
			return base.FindSiteMapNode(rawUrl);
		}

        protected override System.Web.SiteMapNode GetRootNodeCore()
        {
			return Convert((ContentItem)Engine.Persister.Get(Engine.Resolve<Site>().RootItemID));
        }
    }
}
