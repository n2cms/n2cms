using N2.Templates.Items;
using N2.Integrity;
using System.Collections.Generic;
using N2.Templates.Services;
using N2.Web;

namespace N2.Templates.Wiki.Items
{
    [Definition]
    [RestrictParents(typeof(Wiki))]
	[Template("~/Templates/Wiki/UI/Views/Article.aspx")]
    [Template("index", "~/Templates/Wiki/UI/Views/Article.aspx")]
    [Template("submit", "~/Templates/Wiki/UI/Views/Submit.aspx")]
    [Template("modify", "~/Templates/Wiki/UI/Views/Edit.aspx")]
    [Template("history", "~/Templates/Wiki/UI/Views/History.aspx")]
    [Template("upload", "~/Templates/Wiki/UI/Views/Upload.aspx")]
    [Template("version", "~/Templates/Wiki/UI/Views/Version.aspx")]
    public class WikiArticle : AbstractContentPage, IArticle, ISyndicatable
    {

        public WikiArticle()
        {
            Visible = false;
        }

        public override string IconUrl
        {
            get { return "~/Templates/Wiki/UI/Img/article_wiki.gif"; }
        }

		//protected static Dictionary<string, string> ActionTemplates = new Dictionary<string, string>();
		//public string Action { get; set; }
		//public string ActionParameter { get; set; }

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

		//public override ContentItem GetChild(string childName)
		//{
		//    ContentItem article = base.GetChild(childName);
		//    if (article == null)
		//    {
		//        string[] action = GetSegments(childName);
		//        if (ActionTemplates.ContainsKey(action[0]))
		//        {
		//            Action = action[0];
		//            ActionParameter = action[1];
		//            return this;
		//        }
		//    }
		//    return article;
		//}

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

		//public override string TemplateUrl
		//{
		//    get
		//    {
		//        if (string.IsNullOrEmpty(Action))
		//            return "~/Templates/Wiki/UI/Views/Article.aspx";
		//        else if (ActionTemplates.ContainsKey(Action))
		//            return ActionTemplates[Action];
		//        else
		//            throw new N2Exception("Invalid action '{0}'.", Action);
		//    }
		//}

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
