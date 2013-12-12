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
    public class StartPage : ContentPage, IStartPage, IStructuralPage, IThemeable, ILanguage, ISitesSource
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

        [EditableUrl("Login Page", 79, HelpText = "Page to display when authorization to a page fails.")]
        public virtual string LoginPage
        {
            get { return (string)GetDetail("LoginPage"); }
            set { SetDetail("LoginPage", value); }
        }


        #region ISitesSource Members

        public virtual string HostName { get; set; }

        public IEnumerable<Site> GetSites()
        {
            if (!string.IsNullOrEmpty(HostName))
                yield return new Site(Find.EnumerateParents(this, null, true).Last().ID, ID, HostName) { Wildcards = true };
        }

        #endregion
    }
}