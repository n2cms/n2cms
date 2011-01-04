using System;
using System.Web.UI;
using N2.Web;
using N2.Web.UI.WebControls;
using System.Web.UI.WebControls;

namespace N2.Edit
{
	/// <summary>
	/// Registers a link plugin in the control panel plugin area.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ControlPanelLinkAttribute : AdministrativePluginAttribute, IControlPanelPlugin
	{
		public ControlPanelLinkAttribute(string name, string iconUrl, string url, string toolTip, int sortOrder, ControlPanelState showDuring)
		{
			IconUrl = iconUrl;
			NavigateUrl = url;
			ToolTip = toolTip;
			ShowDuring = showDuring;
			SortOrder = sortOrder;
			Name = name;
			GlobalResourceClassName = "ControlPanel";
		}

		public bool RequireCurrentItem { get; set;}

		/// <summary>Url encode the NavigateUrl and NavigateQuery (for usage in query string).</summary>
		public bool UrlEncode { get; set;}

		/// <summary>The anchor text.</summary>
		public string Title { get; set; }

		/// <summary>The anchor target frame.</summary>
		public string Target { get; set; }

		/// <summary>The anchor tool tip.</summary>
		public string ToolTip { get; set;}

		/// <summary>The anchor's url.</summary>
		public string NavigateUrl { get; set; }

		/// <summary>The anchor's url query.</summary>
		public string NavigateQuery { get; set; }

		/// <summary>The control panel state that displays this link.</summary>
		public ControlPanelState ShowDuring { get; set; }

		/// <summary>Used for translating the plugin's texts from a global resource.</summary>
		public string GlobalResourceClassName { get; set; }

		public override Control AddTo(Control container, PluginContext context)
		{
			if(RequireCurrentItem && context.Selected == null)
				return null;
			if(!ActiveFor(container, context.State))
				return null;

			HyperLink link = new HyperLink();
			string tooltip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? ToolTip;
			string title = Utility.GetResourceString(GlobalResourceClassName, Name + ".Title") ?? Title;
			link.Text = GetInnerHtml(context, IconUrl, tooltip, title);
			Url url = context.Rebase(context.Format(NavigateUrl, UrlEncode));
			if (!string.IsNullOrEmpty(NavigateQuery))
				url = url.AppendQuery(context.Format(NavigateQuery, UrlEncode));
			link.NavigateUrl = url;
			link.ToolTip = context.Format(tooltip, false);
            link.CssClass = Name;
			
			AddTargetAttribute(link);

			container.Controls.Add(link);

			return link;
		}

		private void AddTargetAttribute(HyperLink link)
		{
			if (String.IsNullOrEmpty(Target))
				return;

			switch (Target.ToLowerInvariant())
			{
				case "_blank":
					link.Attributes.Add("onclick", "window.open(this.href);return false;");
					break;

				case "_top":
					link.Attributes.Add("onclick", "top.location.href=this.href;return false;");
					break;
			}
		}

		protected virtual bool ActiveFor(Control container, ControlPanelState state)
		{
			return (ShowDuring & state) == state;
		}
	}
}