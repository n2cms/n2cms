using N2.Templates;
using N2.Templates.Items;
using N2.Integrity;
using N2.Templates.Services;
using N2.Web;

namespace N2.Addons.Wiki.Items
{
    [Definition]
    [RestrictParents(typeof(Wiki))]
	[Template("~/Addons/Wiki/UI/Views/Article.aspx")]
    [Template("index", "~/Addons/Wiki/UI/Views/Article.aspx")]
    [Template("submit", "~/Addons/Wiki/UI/Views/Submit.aspx")]
    [Template("modify", "~/Addons/Wiki/UI/Views/Edit.aspx")]
    [Template("history", "~/Addons/Wiki/UI/Views/History.aspx")]
    [Template("upload", "~/Addons/Wiki/UI/Views/Upload.aspx")]
    [Template("version", "~/Addons/Wiki/UI/Views/Version.aspx")]
    public class WikiArticle : AbstractContentPage, IArticle, ISyndicatable
    {

        public WikiArticle()
        {
            Visible = false;
        }

        public override string IconUrl
        {
            get { return "~/Addons/Wiki/UI/Img/article_wiki.gif"; }
        }

        [WikiText("Wiki Text", 100, ContainerName = Tabs.Content)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

		public override ContentItem GetChild(string childName)
		{
			return base.GetChild(childName) ?? base.GetChild(childName.Replace(' ', '-'));
		}

        public string AppendUrl(string action)
        {
            return N2.Web.Url.Parse(Url).AppendSegment(action);
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
                return new[] { string.Empty, string.Empty };

            int slashIndex = path.IndexOf('/');
            if (slashIndex < 0)
                return new[] { path.ToLower(), string.Empty };
        	
			return new[] { path.Substring(0, slashIndex).ToLower(), path.Substring(slashIndex + 1) };
        }

        public virtual IWiki WikiRoot
        {
            get { return Parent as IWiki; }
        }

        #region ISyndicatable Members

        public string Summary
        {
            get { return ""; }
        }

        #endregion
    }
}
