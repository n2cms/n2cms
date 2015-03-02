using System;
using System.IO;
using System.Text;
using System.Web.UI;
using N2.Details;
using N2.Engine;

namespace N2.Web
{
	[PartDefinition(Title = ResponsiveMenuPart.S_RM, IconClass = "fa fa-list-ul", RequiredPermission = N2.Security.Permission.Administer)]
	public class ResponsiveMenuPart : MenuPart
	{
		public const string S_RM = "Responive Menu";

		[EditableEnum("Collapsed Title Display Mode", 200, typeof(TitleDisplayOptions), HelpText = "The title to show when the menu is collapsed")]
		public TitleDisplayOptions MenuCollapsedTitleDisplayMode
		{
			get { return GetDetail("CollapsedTitleDisplay", TitleDisplayOptions.None); }
			set { SetDetail("CollapsedTitleDisplay", value); }
		}
	}

	[Adapts(typeof(ResponsiveMenuPart))]
	public class ResponsiveMenuPartMvcAdapter : MenuPartMvcAdapter
	{
		public override void RenderPart(System.Web.Mvc.HtmlHelper html, ContentItem part, TextWriter writer = null)
		{
			var menuPart = part as ResponsiveMenuPart;
			if (part == null)
				throw new ArgumentException("part");

			var htmlTextWriter = html.ViewContext.Writer as HtmlTextWriter;
			if (htmlTextWriter != null)
			{
				RenderMenu(htmlTextWriter, menuPart);
			}
			else
			{
				var sb = new StringBuilder();
				using (var sw = new StringWriter(sb))
				using (var xml = new HtmlTextWriter(sw))
					RenderMenu(xml, menuPart);
				html.ViewContext.Writer.Write(sb.ToString());
			}
		}

		private void RenderMenu(HtmlTextWriter htmlTextWriter, ResponsiveMenuPart menuPart)
		{
			WriteBefore(htmlTextWriter, menuPart);
			new MenuPartRenderer(menuPart).WriteHtml(htmlTextWriter);
			WriteAfter(htmlTextWriter, menuPart);
		}

		private void WriteBefore(HtmlTextWriter xml, ResponsiveMenuPart menuPart)
		{
			xml.AddAttribute("type", "button");
			xml.AddAttribute("class", "navbar-toggle menuToggle");
			xml.AddAttribute("data-toggle", "collapse");
			xml.AddAttribute("data-target", string.Format("#{0}-collapse", String.IsNullOrEmpty(menuPart.Name) ? "menu" + menuPart.ID : menuPart.Name));
			xml.RenderBeginTag("button"); // <button>

			// add the screen reader info
			xml.AddAttribute("class", "sr-only");
			xml.RenderBeginTag("span"); // <span> 
			xml.Write("Toggle navigation");
			xml.RenderEndTag(); // <span>

			xml.AddAttribute("class", "barContainer");
			xml.RenderBeginTag("div"); // <div class="barContainer">
			for (int i = 0; i < 3; i++)
			{
				xml.AddAttribute("class", "icon-bar");
				xml.RenderBeginTag("span"); // <span class="icon-bar">
				xml.RenderEndTag(); // </span>
			}

			xml.RenderEndTag(); //</div>

			if (menuPart.MenuCollapsedTitleDisplayMode != MenuPartBase.TitleDisplayOptions.None)
			{
				xml.AddAttribute("class", "buttonText");
				xml.RenderBeginTag("span");

				switch (menuPart.MenuCollapsedTitleDisplayMode)
				{
					case MenuPartBase.TitleDisplayOptions.CustomTitle:
						xml.Write(menuPart.Title);
						break;
					case MenuPartBase.TitleDisplayOptions.CurrentPageTitle:
						xml.Write(Content.Current.Page.Title);
						break;
					case MenuPartBase.TitleDisplayOptions.None:
						break;
				}

				xml.RenderEndTag(); // </span>
			}


			xml.RenderEndTag(); // </button>

			xml.AddAttribute("id", string.Format("{0}-collapse", String.IsNullOrEmpty(menuPart.Name) ? "menu" + menuPart.ID : menuPart.Name));
			xml.AddAttribute("class", "collapse navbar-collapse");
			xml.RenderBeginTag("div"); //<div class="collapse navbar-collapse">
		}

		private void WriteAfter(HtmlTextWriter xml, MenuPart menuPart)
		{
			xml.RenderEndTag(); // </div>
		}
	}
}
