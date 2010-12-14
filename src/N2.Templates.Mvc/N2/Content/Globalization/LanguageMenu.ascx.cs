using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Edit.Web;
using N2.Engine.Globalization;
using System.Collections.Generic;

namespace N2.Edit.Globalization
{
	[EditToolbarPlugin("{ManagementUrl}/Content/Globalization/LanguageMenu.ascx")]
	public partial class LanguageMenu : EditPageUserControl
	{
		protected Repeater rptLanguages;

		protected bool CreatingNew
		{
			get { return Request["discriminator"] != null; }
		}
		protected ILanguageGateway Gateway
		{
			get { return Engine.Resolve<ILanguageGateway>(); }
		}
		protected ILanguage CurrentLanguage
		{
			get { return Gateway.GetLanguage(Selection.SelectedItem); }
		}

		protected override void OnPreRender(EventArgs e)
		{
			List<TranslateSpecification> translations = new List<TranslateSpecification>(Gateway.GetEditTranslations(Selection.SelectedItem, false));
			if (Gateway.Enabled && translations.Count > 0)
			{
				if (!CreatingNew)
				{
					rptLanguages.DataSource = translations;
				}
				DataBind();
			}
			else
			{
				Visible = false;
			}
			base.OnPreRender(e);
		}
	}
}