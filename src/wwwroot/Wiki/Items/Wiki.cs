using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Templates.Items;
using N2.Integrity;
using N2.Details;
using N2.Security.Details;
using System.Collections.Generic;

namespace N2.Templates.Wiki.Items
{
    [Definition(SortOrder = 460)]
    [RestrictParents(typeof(IStructuralPage))]
    [N2.Web.UI.TabPanel(Wiki.WikiTab, "Wiki", 110)]
    [AllowedChildren(typeof(Rss.Items.Subscribe))]
    public class Wiki : WikiArticle, IWiki
    {
        public const string WikiTab = "wiki";
        static Wiki()
        {
            ActionTemplates["search"] = "~/Wiki/UI/Search.aspx";
            ActionTemplates["nohits"] = "~/Wiki/UI/NoHits.aspx";
        }
        public Wiki()
        {
            Visible = true;
        }

        public static string DefaultSubmitText = "<p>Please write some content for the article '{{actionparameter}}'.</p>";
        [WikiText("Submit Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string SubmitText
        {
            get { return (string)(GetDetail("SubmitText") ?? DefaultSubmitText); }
            set { SetDetail("SubmitText", value, DefaultHistoryText); }
        }

        public static string DefaultModifyText = "<p>Please write the updated content for '{{actionparameter}}'.</p>";
        [WikiText("Modify Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string ModifyText
        {
            get { return (string)(GetDetail("ModifyText") ?? DefaultModifyText); }
            set { SetDetail("ModifyText", value, DefaultHistoryText); }
        }

        public static string DefaultHistoryText = "<p>These are the current and past revisions of '{{linkfromparameter}}'.</p>";
        [WikiText("History Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string HistoryText
        {
            get { return (string)(GetDetail("HistoryText") ?? DefaultHistoryText); }
            set { SetDetail("HistoryText", value, DefaultHistoryText); }
        }

        public static string DefaultSearchText = "<p>Your search for '{{linkfromparameter}}' yelded the following results:</p>";
        [WikiText("Search Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string SearchText
        {
            get { return (string)(GetDetail("SearchText") ?? DefaultSearchText); }
            set { SetDetail("SearchText", value, DefaultSearchText); }
        }

        public static string DefaultNoHitsText = "<p>Your search for '{{linkfromparameter}}' yelded no results. You may submit new article with the name '{{linkfromparameter}}'.</p>";
        [WikiText("Search No Hits Text", 111, ContainerName = Wiki.WikiTab)]
        public virtual string NoHitsText
        {
            get { return (string)(GetDetail("NoHitsText") ?? DefaultNoHitsText); }
            set { SetDetail("NoHitsText", value, DefaultNoHitsText); }
        }

        [EditableRoles(Title = "Role required for write access", ContainerName = Wiki.WikiTab)]
        public virtual IEnumerable<string> ModifyRoles
        {
            get { return GetDetailCollection("ModifyRoles", true).Enumerate<string>(); }
        }

        public override string IconUrl
        {
            get { return "~/Wiki/UI/Img/page_wiki.gif"; }
        }

        public override IWiki WikiRoot
        {
            get { return this; }
        }

        public override ContentItem GetChild(string childName)
        {
            ContentItem article = base.GetChild(childName) ?? base.GetChild(childName.Replace(' ', '-'));
            if (article == null)
            {
                Action = "submit";
                ActionParameter = Utility.CapitalizeFirstLetter(childName);
                return this;
            }
            return article;
        }
    }
}
