using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using N2;
using N2.Definitions;
using N2.Details;
using N2.Engine.Globalization;
using N2.Integrity;
using N2.Security;
using N2.Web;
using N2.Web.UI;

namespace Dinamico.Models
{
    /// <summary>
    /// This is the start page on a site. Separate start pages can respond to 
    /// a domain name and/or form the root of translation. The registration of
    /// this model is performed by <see cref="Registrations.StartPageRegistration"/>.
    /// </summary>
	[PageDefinition("Start Page",
		Description = "The topmost node of a site. This can be placed below a language intersection to also represent a language",
		IconClass = "fa fa-home",
		InstallerVisibility = N2.Installation.InstallerHint.PreferredStartPage)]
	[WithEditableTranslations(ContainerName = "SiteContainer")]
    public class StartPage : ContentPage, IStartPage, IStructuralPage, IThemeable, ILanguage, ISitesSource, ITranslator
    {
        #region IThemeable Members

        [EditableThemeSelection(EnablePreview = true)]
        public virtual string Theme { get; set; }

        #endregion

        #region ILanguage Members

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

        public virtual string FooterText { get; set; }

        public virtual string Logotype { get; set; }

		public virtual string Author { get; set; }

		public virtual string Keywords { get; set; }

		public virtual string Description { get; set; }

		public virtual string LoginPage { get; set; }

        #region ISitesSource Members

        public virtual string HostName { get; set; }

        public IEnumerable<Site> GetSites()
        {
            if (!string.IsNullOrEmpty(HostName))
                yield return new Site(Find.EnumerateParents(this, null, true).Last().ID, ID, HostName) { Wildcards = true };
        }

		public string Translate(string key, string fallback = null)
		{
			return DetailCollections.GetTranslation(key) ?? fallback;
		}

		public IDictionary<string, string> GetTranslations()
		{
			return DetailCollections.GetTranslations();
		}

		#endregion
	}
}
