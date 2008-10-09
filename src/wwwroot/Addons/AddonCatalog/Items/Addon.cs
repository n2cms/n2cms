using System;
using System.Web.UI;
using N2.Details;
using N2.Integrity;
using N2.Templates;
using N2.Templates.Items;
using N2.Definitions;
using N2.Web.UI;

namespace N2.Addons.AddonCatalog.Items
{
    /// <summary>
    /// To spice things up there is a scheduled action that finds all MyParts and 
    /// updates their properties on a regular basis. Far-fetched? Yes. 
    /// </summary>
    [Definition("Add-on")]
    [RestrictParents(typeof(AddonCatalog))]
    public class Addon : AbstractContentPage, IContainable
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

        [EditableFreeTextArea("Summary", 100, ContainerName = Tabs.Content)]
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

        [EditableTextBox("DownloadUrl", 100, ContainerName = Tabs.Content)]
        public virtual string DownloadUrl
        {
            get { return GetDetail("DownloadUrl", ""); }
            set { SetDetail("DownloadUrl", value, ""); }
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
            return base.GetChild(childName);
        }

        public override string TemplateUrl
        {
            get { return Get.UI + ((Action == "edit") ? "EditAddon.aspx" : "AddonPage.aspx"); }
        }

        public override string IconUrl
        {
            get { return "~/Addons/AddonCatalog/UI/plugin_link.png"; }
        }
    }
}