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
using N2.Security;

namespace Dinamico.Models
{
	/// <summary>
	/// This is the start page on a site. Separate start pages can respond to 
	/// a domain name and/or form the root of translation.
	/// </summary>
	[PageDefinition(
		IconUrl = "{IconsUrl}/page_world.png",
		InstallerVisibility = N2.Installation.InstallerHint.PreferredStartPage)]
	[RestrictParents(typeof(IRootPage), typeof(LanguageIntersection))]
	[RecursiveContainer("SiteContainer", 1000,
        RequiredPermission = Permission.Administer)]
    [TabContainer(Defaults.Containers.Site, "Site", 0,
        ContainerName = "SiteContainer")]
	[WithEditableTemplateSelection(ContainerName = Defaults.Containers.Metadata)]
	public class StartPage : ContentPage, IStartPage, IStructuralPage, IThemeable, ILanguage, ISitesSource
	{
		#region IThemeable Members

		[EditableThemeSelection(EnablePreview = true, ContainerName = Defaults.Containers.Site)]
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
		[DisplayableTokens]
		public virtual string FooterText { get; set; }

		[EditableImageUpload(ContainerName = Defaults.Containers.Site)]
		public virtual string Logotype { get; set; }

		#region ISitesSource Members

		[EditableText(Title = "Site host name (DNS)", 
			ContainerName = Defaults.Containers.Site,
			HelpTitle = "Sets a host name for this site/language. The web server must be configured to accept this host name for this to work.")]
		public virtual string HostName { get; set; }

		public IEnumerable<Site> GetSites()
		{
			if (!string.IsNullOrEmpty(HostName))
				yield return new Site(Find.EnumerateParents(this, null, true).Last().ID, ID, HostName) { Wildcards = true };
		}

		#endregion
	}
}