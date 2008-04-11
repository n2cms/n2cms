using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Integrity;
using N2.Resources;
using N2.Web.UI.WebControls;

[assembly: WebResource("N2.Resources.layout_edit.png", "image/png")]
[assembly: WebResource("N2.Resources.page_refresh.png", "image/png")]

namespace N2.Web.UI.WebControls
{
	public class DragDropControlPanel : ControlPanel
	{
		private string doneText = "Done organizing";
		private string organizeText = "Organize parts";
		private HyperLink dragModeLink = new HyperLink();
		private HyperLink doneDraggingLink = new HyperLink();

		ITemplate dragdropHeaderTemplate;
		ITemplate dragdropFooterTemplate;

		public string DragDropScriptUrl
		{
			get { return (string) (ViewState["DragDropScriptUrl"] ?? "~/Edit/Js/parts.js"); }
			set { ViewState["DragDropScriptUrl"] = value; }
		}


		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate DragDropHeaderTemplate
		{
			get { return this.dragdropHeaderTemplate; }
			set { this.dragdropHeaderTemplate = value; }
		}
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate DragDropFooterTemplate
		{
			get { return this.dragdropFooterTemplate; }
			set { this.dragdropFooterTemplate = value; }
		}

		public string DragDropStyleSheetUrl
		{
			get { return (string)(ViewState["DragDropStyleSheetUrl"] ?? "~/Edit/Css/Parts.css"); }
			set { ViewState["DragDropStyleSheetUrl"] = value; }
		}

		protected virtual IEnumerable<ItemDefinition> AvailableDefinitions
		{
			get
			{
				foreach (ItemDefinition definition in PageDefinition.AllowedChildren)
				{
					if (IsAllowedInAZone(definition))
					{
						yield return definition;
					}
				}
			}
		}

		protected virtual ItemDefinition PageDefinition
		{
			get { return N2.Context.Definitions.GetDefinition(CurrentItem.GetType()); }
		}

		public string DoneText
		{
			get { return doneText; }
			set { doneText = value; }
		}

		public string OrganizeText
		{
			get { return organizeText; }
			set { organizeText = value; }
		}

		[NotifyParentProperty(true)]
		public HyperLink DragModeLink
		{
			get { return dragModeLink; }
			set { dragModeLink = value; }
		}

		[NotifyParentProperty(true)]
		public HyperLink DoneDraggingLink
		{
			get { return doneDraggingLink; }
			set { doneDraggingLink = value; }
		}

		protected override void AddControlPanelControls(ControlPanelState state)
		{
			if (state == ControlPanelState.DragDrop)
			{
				CheckAndAppendTemplate(DragDropHeaderTemplate, this);

				AddDoneButtons(this);
				AddDefinitions(this);

				CheckAndAppendTemplate(DragDropFooterTemplate, this);
			}
			else
			{
				base.AddControlPanelControls(state);
			}
		}

		protected virtual void AddDefinitions(Control container)
		{
			HtmlGenericControl definitions = new HtmlGenericControl("div");
			definitions.Attributes["class"] = "definitions";
			container.Controls.Add(definitions);

			foreach (ItemDefinition definition in AvailableDefinitions)
			{
				HtmlGenericControl div = new HtmlGenericControl("div");
				div.Attributes["title"] = definition.ToolTip;
				div.Attributes["id"] = definition.Discriminator;
				div.Attributes["class"] = "definition " + definition.Discriminator;
				div.InnerHtml = FormatImageAndText(Utility.ToAbsolute(definition.IconUrl), definition.Title);
				definitions.Controls.Add(div);
			}
		}

		private bool IsAllowedInAZone(ItemDefinition definition)
		{
			foreach (AvailableZoneAttribute zone in PageDefinition.AvailableZones)
			{
				if (definition.IsAllowedInZone(zone.ZoneName))
					return true;
			}
			return false;
		}

		protected override void AddEditButtons()
		{
			AddDragButton();
			base.AddEditButtons();
		}

		protected virtual void AddDragButton()
		{
			DragModeLink.NavigateUrl = GetQuickEditUrl("drag");
			DragModeLink.Text = FormatImageAndText(Page.ClientScript.GetWebResourceUrl(typeof(DragDropControlPanel), "N2.Resources.layout_edit.png"), OrganizeText); ;
			DragModeLink.CssClass = "dragEdit";
			Controls.Add(DragModeLink);
		}

		protected virtual void AddDoneButtons(Control container)
		{
			DoneDraggingLink.NavigateUrl = CurrentItem.Url;
			DoneDraggingLink.Text = FormatImageAndText(Page.ClientScript.GetWebResourceUrl(typeof(DragDropControlPanel), "N2.Resources.page_refresh.png"), DoneText);
			DoneDraggingLink.CssClass = "editMode";
			container.Controls.Add(DoneDraggingLink);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (GetState() == ControlPanelState.DragDrop)
			{
				RegisterStyles();
				RegisterScripts();
			}
		}

		private void RegisterStyles()
		{
			Register.StyleSheet(Page, DragDropStyleSheetUrl, Media.All);
		}

		private void RegisterScripts()
		{
			Register.JQuery(Page);
			Register.JavaScript(Page, "~/edit/js/jquery.ui.ashx");
			Register.JavaScript(Page, DragDropScriptUrl);

			Register.JavaScript(Page, @"if(typeof dragItems != 'undefined')
	window.n2ddcp = new DragDrop(dropZones, dropPoints, dragItems);
else
	window.n2ddcp = new DragDrop(dropZones, dropPoints, []);", ScriptOptions.DocumentReady);
		}
	}
}