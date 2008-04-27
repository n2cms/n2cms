using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2;
using N2.Globalization;
using N2.Details;
using N2.Web.UI;
using N2DevelopmentWeb.Domain;

namespace N2DevelopmentWeb.Domain
{
	[Definition]
	[TabPanel("globalization", "Globalization", 200)]
	public class LanguageRoot : MyPageData, ILanguageRoot
	{
		[EditableTextBox("FlagUrl", 100, ContainerName = "globalization")]
		public virtual string FlagUrl
		{
			get { return (string)(GetDetail("FlagUrl") ?? DeduceFlagUrl()); }
			set { SetDetail("FlagUrl", value, DeduceFlagUrl()); }
		}

		private string DeduceFlagUrl()
		{
			if (string.IsNullOrEmpty(LanguageCode))
				return string.Empty;
			return "~/Edit/Globalization/flags/" + LanguageCode + ".png";
		}

		[EditableTextBox("LanguageTitle", 110, ContainerName = "globalization")]
		public virtual string LanguageTitle
		{
			get { return (string)(GetDetail("LanguageTitle") ?? Title); }
			set { SetDetail("LanguageTitle", value, Title); }
		}

		[EditableLanguagesDropDown("LanguageCode", 120, ContainerName = "globalization")]
		public virtual string LanguageCode
		{
			get { return (string)(GetDetail("LanguageCode") ?? string.Empty); }
			set { SetDetail("LanguageCode", value, string.Empty); }
		}
	}
}
