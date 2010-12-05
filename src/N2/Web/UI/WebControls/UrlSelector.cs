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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Engine;

namespace N2.Web.UI.WebControls
{
	/// <summary>An input box that can be updated with the url from the file selector popup in edit mode.</summary>
	public class UrlSelector : TextBox
	{
		public UrlSelector()
		{
			CssClass = "urlSelector";
		}

		protected IEngine Engine
		{
			get { return N2.Context.Current; }
		}

		/// <summary>Text on the button used to open the popup.</summary>
		public string ButtonText
		{
			get { return (string)ViewState["ButtonText"] ?? "..."; }
			set { ViewState["ButtonText"] = value; }
		}

		/// <summary>Url to the page responsible for selecting urls.</summary>
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
			get { return "openUrlSelectorPopup('{0}', '{1}', '{2}', '{3}', '{4}');"; }
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

		#region Script
		private string script =
			@"
function openUrlSelectorPopup(popupUrl,tbId,popupOptions,defaultMode,availableModes){{
	var tb = document.getElementById(tbId);
	window.open(popupUrl
				+ '&tbid=' + tbId 
				+ '&defaultMode=' + defaultMode 
				+ '&availableModes=' + availableModes
				+ '&selectedUrl=' + encodeURIComponent(tb.value),
		null, 
		popupOptions);
}}
";
		#endregion

		/// <summary>Initializes the UrlSelector control.</summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			EnsureChildControls();
			Page.ClientScript.RegisterClientScriptBlock(
				typeof(UrlSelector),
				"N2.Web.UI.WebControls.FileSelector.script",
				script,
				true);
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
			HtmlInputButton hib = new HtmlInputButton();
			hib.ID = ID + "-button";
			hib.Value = ButtonText;
			hib.Attributes["class"] = "popupButton selectorButton";
			Controls.Add(hib);
			hib.Attributes["onclick"] = string.Format(OpenPopupFormat,
													  N2.Web.Url.ToAbsolute(BrowserUrl),
													  ClientID,
													  PopupOptions,
													  DefaultMode,
													  AvailableModes);
			hib.RenderControl(writer);
		}
	}
}