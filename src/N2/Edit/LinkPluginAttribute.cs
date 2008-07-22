using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Edit
{
	/// <summary>
	/// Base class for plugins in the navigation screen and toolbar.
	/// </summary>
	public abstract class LinkPluginAttribute : AdministrativePluginAttribute
	{
		private string globalResourceClassName;
		private string iconUrl;
		private string target = "preview";
		private string title;
		private string toolTip;
		private string urlFormat;

		/// <summary>The target frame for the plugn link.</summary>
		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		/// <summary>The plugin's icon.</summary>
		public string IconUrl
		{
			get { return iconUrl; }
			set { iconUrl = value; }
		}

		/// <summary>The plugin's text.</summary>
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		/// <summary>
		/// The plugin's url format. These magic strings are interpreted by the 
		/// client and inserted in the url before the frame is loaded: 
		/// {selected}, {memory}, {action}
		/// </summary>
		public string UrlFormat
		{
			get { return urlFormat; }
			set { urlFormat = value; }
		}

		/// <summary>The plugin's tool tip.</summary>
		public string ToolTip
		{
			get { return toolTip; }
			set { toolTip = value; }
		}

		/// <summary>Used for translating the plugin's texts from a global resource.</summary>
		public string GlobalResourceClassName
		{
			get { return globalResourceClassName; }
			set { globalResourceClassName = value; }
		}

		/// <summary>This string is used by the client to find plugins.</summary>
		protected abstract string ArrayVariableName { get; }

		public override Control AddTo(Control container)
		{
			HtmlAnchor a = AddAnchor(container);

			RegisterToolbarUrl(container, a.ClientID, N2.Web.Url.ToAbsolute(UrlFormat));

			return a;
		}

        protected virtual void RegisterToolbarUrl(Control container, string clientID, string urlFormat)
        {
            string arrayScript = string.Format("{{ linkId: \"{0}\", urlFormat: \"{1}\" }}", clientID, urlFormat);
            container.Page.ClientScript.RegisterArrayDeclaration(ArrayVariableName, arrayScript);
        }

		protected virtual HtmlAnchor AddAnchor(Control container)
		{
			string tooltip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? ToolTip;
			string title = Utility.GetResourceString(GlobalResourceClassName, Name + ".Title") ?? Title;

			HtmlAnchor a = new HtmlAnchor();
			a.ID = "h" + Name;
			a.HRef = UrlFormat
				.Replace("~/", N2.Web.Url.ToAbsolute("~/"))
				.Replace("{selected}", GetSelectedPath(container))
				.Replace("{memory}", "")
				.Replace("{action}", "");

			a.Target = Target;
			a.Attributes["class"] = "command";
			a.Title = tooltip;

			if (string.IsNullOrEmpty(IconUrl))
				a.InnerHtml = title;
			else
				a.InnerHtml = string.Format("<img src='{0}' alt='{1}'/>{2}", N2.Web.Url.ToAbsolute(IconUrl), tooltip, title);

			container.Controls.Add(a);
			return a;
		}
	}
}
