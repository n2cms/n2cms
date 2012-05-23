#region License

/* Copyright (C) 2007-2009 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */

#endregion

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Engine;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
	/// <summary>An input box that can be updated with the url from the file selector popup in edit mode.</summary>
	public class UrlSelector : TextBox
	{
		public UrlSelector()
		{
			CssClass = "urlSelector selector";
		}

		/// <summary>Content item types that may be selected using this selector.</summary>
		public string SelectableTypes
		{
			get { return ViewState["SelectableTypes"] as string; }
			set { ViewState["SelectableTypes"] = value; }
		}

		/// <summary>File extensions that may be selected using this selector.</summary>
		public string SelectableExtensions
		{
			get { return ViewState["SelectableExtensions"] as string; }
			set { ViewState["SelectableExtensions"] = value; }
		}

		private IEngine engine;
		protected IEngine Engine
		{
			get { return engine ?? N2.Context.Current; }
			set { engine = value; }
		}

		/// <summary>Text on the button used to open the popup.</summary>
		public string ButtonText
		{
			get { return (string)ViewState["ButtonText"] ?? "..."; }
			set { ViewState["ButtonText"] = value; }
		}

		/// <summary>Url to the page responsible for selecting managementUrls.</summary>
		public string BrowserUrl
		{
			get { return (string)ViewState["BrowserUrl"] ?? N2.Web.Url.Parse("{ManagementUrl}/Content/Navigation/Tree.aspx").ResolveTokens().AppendQuery("location=selection"); }
			set { ViewState["BrowserUrl"] = value; }
		}

		/// <summary>The selected url.</summary>
		public virtual string Url
		{
			get { return Text; }
			set { Text = value; }
		}

		/// <summary>Size and features of the popup window.</summary>
		public virtual string PopupOptions
		{
			get { return (string)ViewState["PopupOptions"] ?? "height=600,width=400,resizable=yes,status=yes,scrollbars=yes"; }
			set { ViewState["PopupOptions"] = value; }
		}

		/// <summary>Format for the javascript invoked when the open popup button is clicked.</summary>
		protected virtual string OpenPopupFormat
		{
			get { return "openUrlSelectorPopup('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');"; }
		}

		public virtual UrlSelectorMode DefaultMode
		{
			get { return ViewState["DefaultMode"] != null ? (UrlSelectorMode)ViewState["DefaultMode"] : UrlSelectorMode.Items; }
			set { ViewState["DefaultMode"] = value; }
		}

		public virtual UrlSelectorMode AvailableModes
		{
			get { return ViewState["AvailableModes"] != null ? (UrlSelectorMode)ViewState["AvailableModes"] : UrlSelectorMode.All; }
			set { ViewState["AvailableModes"] = value; }
		}

		/// <summary>Initializes the UrlSelector control.</summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			EnsureChildControls();

			Page.JavaScript("{ManagementUrl}/Resources/Js/UrlSelection.js");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Page.JQueryUi();
			Page.JQueryPlugins();

			RegisterClientScripts();
		}

		protected virtual void RegisterClientScripts()
		{
			Page.JavaScript("$('#" + ClientID + "').n2autocomplete({ filter: 'any', selectableTypes:'" + SelectableTypes + "' });", ScriptPosition.Bottom, ScriptOptions.DocumentReady | ScriptOptions.ScriptTags);
		}

		/// <summary>Renders and tag and the open popup window button.</summary>
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);

			RenderButton(writer);
		}

		/// <summary>Renders the open popup button.</summary>
		private void RenderButton(HtmlTextWriter writer)
		{
			HtmlGenericControl span = new HtmlGenericControl("span");
			Controls.Add(span);
			HtmlInputButton cb = new HtmlInputButton();
            span.Controls.Add(cb);
            HtmlInputButton pb = new HtmlInputButton();
            span.Controls.Add(pb);

			span.Attributes["class"] = "selectorButtons";

			pb.Value = ButtonText;
			pb.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Select") ?? "Select";
			pb.Attributes["class"] = "popupButton selectorButton";
			pb.Attributes["onclick"] = string.Format(OpenPopupFormat,
													  N2.Web.Url.ResolveTokens(BrowserUrl),
													  ClientID,
													  PopupOptions,
													  DefaultMode,
													  AvailableModes,
													  SelectableTypes,
													  SelectableExtensions
													  );
			cb.Value = "x";
			cb.Attributes["title"] = Utility.GetGlobalResourceString("UrlSelector", "Clear") ?? "Clear";
			cb.Attributes["class"] = "clearButton";
			cb.Attributes["onclick"] = "document.getElementById('" + ClientID + "').value = '';";

			span.RenderControl(writer);
		}
	}
}