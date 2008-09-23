using N2.Integrity;

namespace N2.Templates.Wiki.Items
{
    [Definition]
    [AllowedZones(AllowedZones.All)]
    [RestrictParents(typeof(WikiArticle))]
    public class WikiText : Templates.Items.AbstractItem, IArticle
    {
        public override string TemplateUrl
        {
            get { return "~/Templates/Wiki/UI/Parts/WikiPart.ascx"; }
        }

        public override string IconUrl
        {
            get { return "~/Templates/Wiki/UI/Img/part_wiki.gif"; }
        }

        #region IArticle Members

        [WikiText("Wiki Text", 100)]
        public virtual string Text
        {
            get { return (string)(GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        public IWiki WikiRoot
        {
            get { return Parent as IWiki; }
        } 

        public string Action
        {
            get { return string.Empty; }
        }

        public string ActionParameter
        {
            get { return string.Empty; }
        }

        #endregion
    }
}
