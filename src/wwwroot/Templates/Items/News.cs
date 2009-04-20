using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Services;
using N2.Web;

namespace N2.Templates.Items
{
    [Definition("News", "News", "A news page.", "", 155)]
    [RestrictParents(typeof (NewsContainer))]
	[ConventionTemplate("NewsItem")]
    public class News : AbstractContentPage, ISyndicatable
    {
        public News()
        {
            Visible = false;
        }

        public override void AddTo(ContentItem newParent)
        {
            Utility.Insert(this, newParent, "Published DESC");
        }

        [EditableTextBox("Introduction", 90, ContainerName = Tabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 4,
            Columns = 80)]
        public virtual string Introduction
        {
            get { return (string) (GetDetail("Introduction") ?? string.Empty); }
            set { SetDetail("Introduction", value, string.Empty); }
        }

        string ISyndicatable.Summary
        {
            get { return Introduction; }
        }

        protected override string IconName
        {
            get { return "newspaper"; }
        }
    }
}