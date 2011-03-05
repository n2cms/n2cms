using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Integrity;
using N2.Definitions;
using N2.Details;
using N2.Engine.Globalization;
using System.Globalization;

namespace Dinamico.Models
{
	[PageDefinition(
		IconUrl = "{IconsUrl}/page_world.png",
		InstallerVisibility = N2.Installation.InstallerHint.PreferredStartPage)]
	[RestrictParents(typeof(IRootPage), typeof(LanguageIntersection))]
	public class StartPage : TextPage, IStartPage, IStructuralPage, IThemeable, ILanguage
	{
		#region IThemeable Members

		[EditableThemeAttribute]
		public virtual string Theme { get; set; }

		#endregion

		#region ILanguage Members

		public string FlagUrl
		{
			get
			{
				if (string.IsNullOrEmpty(LanguageCode))
					return "";

				string[] parts = LanguageCode.Split('-');
				return N2.Web.Url.ResolveTokens(string.Format("~/N2/Resources/Img/Flags/{0}.png", parts[parts.Length - 1].ToLower()));
			}
		}

		[EditableLanguagesDropDown("Language", 100, ContainerName = Constants.Containers.Site)]
		public virtual string LanguageCode { get; set; }

		public string LanguageTitle
		{
			get
			{
				if (string.IsNullOrEmpty(LanguageCode))
					return "";
				else
					return new CultureInfo(LanguageCode).DisplayName;
			}
		}

		#endregion

		[EditableFreeTextArea("Footer text", 200)]
		public virtual string FooterText { get; set; }
	}
}