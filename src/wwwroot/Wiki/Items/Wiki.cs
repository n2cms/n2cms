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
    public class Wiki : WikiArticle, IWiki
    {
        static Wiki()
        {
            actions["search"] = "~/Wiki/UI/SearchArticle.aspx";
        }
        public Wiki()
        {
            Visible = true;
        }

        [WikiText("Submit Text", 110, ContainerName = Tabs.Content)]
        public virtual string SubmitText
        {
            get { return (string)(GetDetail("SubmitText") ?? string.Empty); }
            set { SetDetail("SubmitText", value, string.Empty); }
        }

        [WikiText("Modify Text", 110, ContainerName = Tabs.Content)]
        public virtual string ModifyText
        {
            get { return (string)(GetDetail("ModifyText") ?? string.Empty); }
            set { SetDetail("ModifyText", value, string.Empty); }
        }

        [WikiText("History Text", 110, ContainerName = Tabs.Content)]
        public virtual string HistoryText
        {
            get { return (string)(GetDetail("HistoryText") ?? string.Empty); }
            set { SetDetail("HistoryText", value, string.Empty); }
        }

        [WikiText("Search Text", 110, ContainerName = Tabs.Content)]
        public virtual string SearchText
        {
            get { return (string)(GetDetail("SearchText") ?? string.Empty); }
            set { SetDetail("SearchText", value, string.Empty); }
        }

        [EditableRoles(Title = "Require these roles to input or change information.")]
        public virtual IEnumerable<string> ModifyRoles
        {
            get { return GetDetailCollection("ModifyRoles", true).Enumerate<string>(); }
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
