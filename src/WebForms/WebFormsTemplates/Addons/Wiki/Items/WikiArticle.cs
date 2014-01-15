using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Web;
using N2.Web.UI;
using N2.Persistence;

namespace N2.Addons.Wiki.Items
{
    [WithEditableTitle("Title", 10, Focus = true, ContainerName = Tabs.Content)]
    [WithEditableName("Name", 20, ContainerName = Tabs.Content)]
    [WithEditablePublishedRange("Published Between", 30, ContainerName = Tabs.Advanced, BetweenText = " and ")]
    [TabContainer(Tabs.Content, "Content", Tabs.ContentIndex)]
    [TabContainer(Tabs.Advanced, "Advanced", Tabs.AdvancedIndex)]
    public abstract class WikiBase : ContentItem
    {
        [EditableFreeTextArea("Text", 100, ContainerName = Tabs.Content)]
        public virtual string Text
        {
            get { return (string)(GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        [EditableCheckBox("Visible", 40, ContainerName = Tabs.Advanced)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }
    }

    [PageDefinition("Wiki Article", IconUrl = "~/Addons/Wiki/UI/Img/article_wiki.gif")]
    [RestrictParents(typeof(Wiki))]
    [Template("~/Addons/Wiki/UI/Views/Article.aspx")]
    [Template("index", "~/Addons/Wiki/UI/Views/Article.aspx")]
    [Template("submit", "~/Addons/Wiki/UI/Views/Submit.aspx")]
    [Template("modify", "~/Addons/Wiki/UI/Views/Edit.aspx")]
    [Template("history", "~/Addons/Wiki/UI/Views/History.aspx")]
    [Template("upload", "~/Addons/Wiki/UI/Views/Upload.aspx")]
    [Template("version", "~/Addons/Wiki/UI/Views/Version.aspx")]
    public class WikiArticle : WikiBase, IArticle, ISyndicatable, IContentPage
    {
        public WikiArticle()
        {
            Visible = false;
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

        [Persistable(PersistAs = PropertyPersistenceLocation.Detail)]
        public virtual bool Syndicate { get; set; }

        #endregion
    }
}
