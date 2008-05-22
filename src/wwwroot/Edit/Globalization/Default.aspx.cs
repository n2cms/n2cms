using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Globalization;
using System.Collections.Generic;
using N2.Collections;

namespace N2.Edit.Globalization
{
	[ToolbarPlugin("", "globalization", "~/Edit/Globalization/Default.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/world.gif", 150, ToolTip = "view translations", GlobalResourceClassName = "Toolbar")]
	public partial class _Default : EditPage
	{
		protected ILanguageGateway gateway;
		protected IEnumerable<ILanguage> languages;

		protected override void OnInit(EventArgs e)
		{
			gateway = Engine.Resolve<ILanguageGateway>();
			languages = gateway.GetAvailableLanguages();

			hlCancel.NavigateUrl = SelectedNode.PreviewUrl;

			DataBind();

			base.OnInit(e);
		}

		protected IEnumerable<ContentItem> GetChildren()
		{
			foreach (ContentItem item in SelectedItem.GetChildren(Engine.EditManager.GetEditorFilter(User)))
			{
				if (!(item is ILanguage))
					yield return item;
			}
		}

		protected IEnumerable<TranslateSpecification> GetTranslations(ContentItem item)
		{
			foreach (TranslateSpecification translate in gateway.GetEditTranslations(item, true))
			{
				translate.EditUrl += "&returnUrl=" + Server.UrlEncode(Request.RawUrl);
				yield return translate;
			}
		}
	}
}
