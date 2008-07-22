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
using System.Collections.Generic;
using N2.Engine.Globalization;

namespace N2.Templates.UI.Layouts.Parts
{
	public partial class TopMenu : N2.Templates.Web.UI.TemplateUserControl<ContentItem>
	{
		public int MaxLevels
		{
			get { return tm.MaxLevels; }
			set { tm.MaxLevels = value; }
		}

		protected ILanguageGateway languages;
		protected override void OnInit(EventArgs e)
		{
			languages = Engine.Resolve<ILanguageGateway>();

            if (languages.Enabled)
            {
                rptLanguages.DataSource = GetTranslations();
                DataBind();

                rptLanguages.Visible = rptLanguages.Items.Count > 0;
            }
            else
            {
                rptLanguages.Visible = false;
            }
			base.OnInit(e);
		}

		private IEnumerable<Translation> GetTranslations()
		{
			foreach (ContentItem translation in languages.FindTranslations(CurrentPage))
			{
				yield return new Translation(translation, languages.GetLanguage(translation));
			}
		}
	}
}