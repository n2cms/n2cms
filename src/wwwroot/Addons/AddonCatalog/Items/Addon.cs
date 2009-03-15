using N2.Details;
using N2.Integrity;
using N2.Templates;
using N2.Templates.Items;
using N2.Web;
using System.Web.UI.WebControls;

namespace N2.Addons.AddonCatalog.Items
{
    /// <summary>
    /// To spice things up there is a scheduled action that finds all MyParts and 
    /// updates their properties on a regular basis. Far-fetched? Yes. 
    /// </summary>
    [Definition("Add-on", SortOrder = 1000)]
    [RestrictParents(typeof(AddonCatalog))]
	[Template("~/Addons/AddonCatalog/UI/AddonPage.aspx")]
	[Template("edit", "~/Addons/AddonCatalog/UI/EditAddon.aspx")]
	[Template("download", "~/Addons/AddonCatalog/UI/Download.aspx")]
	public class Addon : AbstractContentPage
    {
        public Addon()
        {
            Visible = false;
        }

		[EditableTextBox("Text", 100, ContainerName = Tabs.Content, TextMode = TextBoxMode.MultiLine)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

        [EditableEnum("Category", 100, typeof(CodeCategory), ContainerName = Tabs.Content)]
        public virtual CodeCategory Category
        {
            get { return GetDetail("Category", CodeCategory.None); }
            set { SetDetail("Category", (int)value, (int)CodeCategory.None); }
        }

        [EditableEnum("Requirements", 100, typeof(Requirement), ContainerName = Tabs.Content)]
        public virtual Requirement Requirements
        {
            get { return GetDetail("Requirements", Requirement.None); }
            set { SetDetail("Requirements", (int)value, (int)Requirement.None); }
        }

        [EditableTextBox("Last Tested Version", 100, ContainerName = Tabs.Content)]
        public virtual string LastTestedVersion
        {
            get { return GetDetail("LastTestedVersion", ""); }
            set { SetDetail("LastTestedVersion", value, ""); }
        }

        [EditableTextBox("Version", 100, ContainerName = Tabs.Content)]
        public virtual string AddonVersion
        {
            get { return GetDetail("AddonVersion", ""); }
            set { SetDetail("AddonVersion", value, ""); }
        }

		[EditableTextBox("Summary", 100, ContainerName = Tabs.Content, TextMode = TextBoxMode.MultiLine)]
        public virtual string Summary
        {
            get { return GetDetail("Summary", ""); }
            set { SetDetail("Summary", value, ""); }
        }

        [EditableTextBox("HomepageUrl", 100, ContainerName = Tabs.Content)]
        public virtual string HomepageUrl
        {
            get { return GetDetail("HomepageUrl", ""); }
            set { SetDetail("HomepageUrl", value, ""); }
        }

        [EditableTextBox("SourceCodeUrl", 100, ContainerName = Tabs.Content)]
        public virtual string SourceCodeUrl
        {
            get { return GetDetail("SourceCodeUrl", ""); }
            set { SetDetail("SourceCodeUrl", value, ""); }
        }

        [EditableTextBox("Votes", 100, ContainerName = Tabs.Content)]
        public virtual int Votes
        {
            get { return GetDetail("Votes", 0); }
            set { SetDetail("Votes", value, 0); }
        }

        [EditableTextBox("Downloads", 100, ContainerName = Tabs.Content)]
        public virtual int Downloads
        {
            get { return GetDetail("Downloads", 0); }
            set { SetDetail("Downloads", value, 0); }
        }

		[EditableTextBox("UploadedFileUrl", 100, ContainerName = Tabs.Content)]
        public virtual string UploadedFileUrl
        {
            get { return GetDetail("UploadedFileUrl", ""); }
            set { SetDetail("UploadedFileUrl", value, ""); }
        }

		[EditableTextBox("ContactEmail", 100, ContainerName = Tabs.Content)]
        public virtual string ContactEmail
        {
            get { return GetDetail("ContactEmail", ""); }
            set { SetDetail("ContactEmail", value, ""); }
		}

		[EditableTextBox("ContactName", 100, ContainerName = Tabs.Content)]
		public virtual string ContactName
		{
			get { return GetDetail("ContactName", ""); }
			set { SetDetail("ContactName", value, ""); }
		}

		[EditableTextBox("AuthorUserName", 100, ContainerName = Tabs.Content)]
		public virtual string AuthorUserName
		{
			get { return GetDetail<string>("AuthorUserName", null); }
			set { SetDetail("AuthorUserName", value, ""); }
		}

        public virtual string DownloadUrl
        {
            get { return Web.Url.Parse(Url).AppendSegment("download"); }
        }

        public override string IconUrl
        {
            get { return "~/Addons/AddonCatalog/UI/plugin_link.png"; }
        }
    }
}