using System;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates;
using N2.Templates.Items;
using N2.Definitions;

namespace N2.Addons.AddonCatalog.Items
{
    /// <summary>
    /// To spice things up there is a scheduled action that finds all MyParts and 
    /// updates their properties on a regular basis. Far-fetched? Yes. 
    /// </summary>
    [Definition("Add-On")]
    [RestrictParents(typeof(AddonCatalog))]
    public class Addon : AbstractContentPage
    {
        public Addon()
        {
            Visible = false;
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

        [N2.Details.EditableTextBox("Version", 100, ContainerName = Tabs.Content)]
        public virtual string AddonVersion
        {
            get { return GetDetail("AddonVersion", ""); }
            set { SetDetail("AddonVersion", value, ""); }
        }

        [EditableTextBox("Summary", 100, TextMode = TextBoxMode.MultiLine, ContainerName = Tabs.Content)]
        public virtual string Summary
        {
            get { return GetDetail("Summary", ""); }
            set { SetDetail("Summary", value, ""); }
        }

        [EditableTextBox("Description", 100, TextMode = TextBoxMode.MultiLine, ContainerName = Tabs.Content)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        [EditableTextBox("HomepageUrl", 100, ContainerName = Tabs.Content)]
        public virtual string HomepageUrl
        {
            get { return GetDetail("HomepageUrl", ""); }
            set { SetDetail("HomepageUrl", value, ""); }
        }

        [EditableTextBox("ContactName", 100, ContainerName = Tabs.Content)]
        public virtual string ContactName
        {
            get { return GetDetail("ContactName", ""); }
            set { SetDetail("ContactName", value, ""); }
        }

        [EditableTextBox("ContactEmail", 100, ContainerName = Tabs.Content)]
        public virtual string ContactEmail
        {
            get { return GetDetail("ContactEmail", ""); }
            set { SetDetail("ContactEmail", value, ""); }
        }

        [EditableTextBox("UploadedFileUrl", 100, ContainerName = Tabs.Content)]
        public virtual string UploadedFileUrl
        {
            get { return GetDetail("UploadedFileUrl", ""); }
            set { SetDetail("UploadedFileUrl", value, ""); }
        }

        public virtual string DownloadUrl
        {
            get { return Web.Url.Parse(Url).AppendSegment("download"); }
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


        public string Action { get; set; }

        public override ContentItem GetChild(string childName)
        {
            if (childName.Equals("edit", StringComparison.InvariantCultureIgnoreCase))
            {
                Action = "edit";
                return this;
            }
            if(childName.Equals("download", StringComparison.InvariantCultureIgnoreCase))
            {
                Action = "download";
                return this;
            }
            return base.GetChild(childName);
        }

        public override string TemplateUrl
        {
            get 
            {
                switch (Action)
                {
                    case "edit":
                        return Paths.UI + "EditAddon.aspx";
                    case "download":
                        return Paths.UI + "Download.aspx";
                    default:
                        return Paths.UI + "AddonPage.aspx";
                }
            }
        }

        public override string IconUrl
        {
            get { return Paths.UI + "plugin_link.png"; }
        }
    }
}