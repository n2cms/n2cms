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
using N2.Web.UI;
using N2.Web;

namespace Dinamico.Models
{
	[PageDefinition(
		IconUrl = "{IconsUrl}/page_world.png",
		InstallerVisibility = N2.Installation.InstallerHint.PreferredStartPage)]
	[RestrictParents(typeof(IRootPage), typeof(LanguageIntersection))]
	[TabContainer(Defaults.Containers.Site, "Site", 1000)]
	public class StartPage : ContentPage, IStartPage, IStructuralPage, IThemeable, ILanguage, ISitesSource
	{
		#region IThemeable Members

		[EditableThemeAttribute(ContainerName = Defaults.Containers.Site)]
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

		[EditableLanguagesDropDown("Language", 100, ContainerName = Defaults.Containers.Site)]
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
		
		[EditableFreeTextArea("Footer text", 200, ContainerName = Defaults.Containers.Site)]
		public virtual string FooterText { get; set; }

		[EditableFileUpload(ContainerName = Defaults.Containers.Site)]
		public virtual string Logotype { get; set; }

		#region ISitesSource Members

		[EditableText(Title = "Site host name (DNS)", ContainerName = Defaults.Containers.Site)]
		public virtual string HostName { get; set; }

		public IEnumerable<Site> GetSites()
		{
			if (!string.IsNullOrEmpty(HostName))
				yield return new Site(Find.EnumerateParents(this, null, true).Last().ID, ID, HostName) { Wildcards = true };
		}

		#endregion
	}
}