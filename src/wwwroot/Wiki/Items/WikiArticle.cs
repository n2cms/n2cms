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

namespace N2.Templates.Wiki.Items
{
    [Definition]
    [RestrictParents(typeof(Wiki))]
    public class WikiArticle : AbstractContentPage, IArticle
    {
        protected static Dictionary<string, string> actions = new Dictionary<string, string>();
        static WikiArticle()
        {
            actions["submit"] = "~/Wiki/UI/Submit.aspx";
            actions["modify"] = "~/Wiki/UI/Edit.aspx";
            actions["history"] = "~/Wiki/UI/History.aspx";
        }

        public WikiArticle()
        {
            Visible = false;
        }

        public override string IconUrl
        {
            get { return "~/Wiki/UI/Img/article_wiki.gif"; }
        }

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
                if (actions.ContainsKey(action[0]))
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
                else if (actions.ContainsKey(Action))
                    return actions[Action];
                else
                    throw new N2Exception("Invalid action '" + Action + "'.");
            }
        }

        public virtual IWiki WikiRoot
        {
            get { return this.Parent as IWiki; }
        }
    }
}
