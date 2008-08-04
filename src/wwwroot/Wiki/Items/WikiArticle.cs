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
using System.Collections.Generic;
using N2.Web;
using N2.Templates.Syndication;

namespace N2.Templates.Wiki.Items
{
    [Definition]
    [RestrictParents(typeof(Wiki))]
    public class WikiArticle : AbstractContentPage, IArticle, ISyndicatable
    {
        static WikiArticle()
        {
            ActionTemplates["index"] = "~/Wiki/UI/Article.aspx";
            ActionTemplates["submit"] = "~/Wiki/UI/Submit.aspx";
            ActionTemplates["modify"] = "~/Wiki/UI/Edit.aspx";
            ActionTemplates["history"] = "~/Wiki/UI/History.aspx";
            ActionTemplates["upload"] = "~/Wiki/UI/Upload.aspx";
            ActionTemplates["version"] = "~/Wiki/UI/Version.aspx";
        }

        public WikiArticle()
        {
            Visible = false;
        }

        public override string IconUrl
        {
            get { return "~/Wiki/UI/Img/article_wiki.gif"; }
        }

        protected static Dictionary<string, string> ActionTemplates = new Dictionary<string, string>();
        public string Action { get; set; }
        public string ActionParameter { get; set; }

        [WikiText("Wiki Text", 100, ContainerName = Tabs.Content)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public string AppendUrl(string action)
        {
            return N2.Web.Url.Parse(Url).AppendSegment(action);
        }

        public override ContentItem GetChild(string childName)
        {
            ContentItem article = base.GetChild(childName);
            if (article == null)
            {
                string[] action = GetSegments(childName);
                if (ActionTemplates.ContainsKey(action[0]))
                {
                    Action = action[0];
                    ActionParameter = action[1];
                    return this;
                }
            }
            return article;
        }

        public override string SavedBy
        {
            get
            {
                string name = base.SavedBy;
                if (string.IsNullOrEmpty(name))
                    name = (string)this["SavedByAddress"] ?? string.Empty;
                return name;
            }
            set { base.SavedBy = value; }
        }

        protected string[] GetSegments(string path)
        {
            if (path == null)
                return new string[] { string.Empty, string.Empty };
            int slashIndex = path.IndexOf('/');
            if (slashIndex < 0)
                return new string[] { path.ToLower(), string.Empty };
            else
                return new string[] { path.Substring(0, slashIndex).ToLower(), path.Substring(slashIndex + 1) };
        }

        public override string TemplateUrl
        {
            get
            {
                if (string.IsNullOrEmpty(Action))
                    return "~/Wiki/UI/Article.aspx";
                else if (ActionTemplates.ContainsKey(Action))
                    return ActionTemplates[Action];
                else
                    throw new N2Exception("Invalid action '{0}'.", Action);
            }
        }

        public virtual IWiki WikiRoot
        {
            get { return this.Parent as IWiki; }
        }

        #region ISyndicatable Members

        public string Summary
        {
            get { return ""; }
        }

        #endregion
    }
}
