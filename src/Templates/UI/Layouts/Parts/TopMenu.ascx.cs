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
using N2.Globalization;
using System.Collections.Generic;

namespace N2.Templates.UI.Layouts.Parts
{
	public partial class TopMenu : N2.Templates.Web.UI.TemplateUserControl<ContentItem>
	{
		protected ILanguageGateway languages;
		protected override void OnInit(EventArgs e)
		{
			languages = Engine.Resolve<ILanguageGateway>();

			rptLanguages.DataSource = GetTranslations();
			DataBind();

			rptLanguages.Visible = rptLanguages.Items.Count > 0;

			base.OnInit(e);
		}

		private IEnumerable<Translation> GetTranslations()
		{
			foreach (ContentItem translation in languages.FindTranslations(CurrentPage))
			{
				yield return new Translation(translation, languages.GetLanguage(translation));
			}
		}

		public class Translation
		{
			private ContentItem page;
			private ILanguage language;

			public Translation(ContentItem page, ILanguage language)
			{
				this.page = page;
				this.language = language;
			}

			public ContentItem Page
			{
				get { return page; }
				set { page = value; }
			}

			public ILanguage Language
			{
				get { return language; }
				set { language = value; }
			}
		}
	}
}