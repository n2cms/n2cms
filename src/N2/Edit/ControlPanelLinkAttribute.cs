using System;
using System.Web.UI;
using N2.Web.UI.WebControls;
using System.Web.UI.WebControls;

namespace N2.Edit
{
	/// <summary>
	/// Registers a plugin in the control panel plugin area.
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
		}

		public bool RequireCurrentItem { get; set;}
		public bool UrlEncode { get; set;}
		public string Title { get; set; }
		public string Target { get; set; }
		public string ToolTip { get; set;}
		public string NavigateUrl { get; set;}
		public string IconUrl { get; set;}
		public ControlPanelState ShowDuring { get; set;}

		public override Control AddTo(Control container, PluginContext context)
		{
			if(RequireCurrentItem && context.Selected == null)
				return null;
			if(!ActiveFor(container, context.State))
				return null;

			HyperLink link = new HyperLink();
			link.Text = GetInnerHtml(IconUrl, ToolTip, Title);
			link.NavigateUrl = context.Format(NavigateUrl, UrlEncode);
			link.ToolTip = context.Format(ToolTip, false);
			link.Target = Target;
			container.Controls.Add(link);

			return link;
		}

		protected virtual bool ActiveFor(Control container, ControlPanelState state)
		{
			return (ShowDuring & state) == state;
		}
	}
}
