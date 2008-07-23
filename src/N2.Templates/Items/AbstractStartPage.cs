using System;
using System.Collections.Generic;
using System.Text;
using N2.Web;
using N2.Details;

namespace N2.Templates.Items
{
    [N2.Web.UI.FieldSet("layoutArea", "Layout", 75, ContainerName = AbstractStartPage.SiteArea)]
    [N2.Web.UI.FieldSet("miscArea", "Miscellaneous", 80, ContainerName = AbstractStartPage.SiteArea)]
    public abstract class AbstractStartPage : AbstractContentPage, IStructuralPage, ISitesSource
	{
        public const string SiteArea = "siteArea";
        public const string LayoutArea = "layoutArea";
        public const string MiscArea = "miscArea";

        [EditableTextBox("Host Name", 72, ContainerName = MiscArea)]
		public virtual string HostName
		{
			get { return (string)(GetDetail("HostName") ?? string.Empty); }
			set { SetDetail("HostName", value); }
		}

		public abstract string Theme { get; set; }
		public abstract string Layout { get; set; }

        [EditableLink("Not Found Page (404)", 77, ContainerName = MiscArea, HelpText = "Display this page when the requested URL isn't found")]
		public virtual ContentItem NotFoundPage
		{
			get { return (ContentItem)GetDetail("NotFoundPage"); }
			set { SetDetail("NotFoundPage", value); }
        }

        [EditableLink("Error Page (500)", 78, ContainerName = MiscArea, HelpText = "Display this page when an unhandled exception occurs.")]
        public virtual ContentItem ErrorPage
        {
            get { return (ContentItem)GetDetail("ErrorPage"); }
            set { SetDetail("ErrorPage", value); }
        }

        [EditableLink("Login Page", 79, ContainerName = MiscArea, HelpText = "Page to display when authorization to a page fails.")]
        public virtual ContentItem LoginPage
        {
            get { return (ContentItem)GetDetail("LoginPage"); }
            set { SetDetail("LoginPage", value); }
        }

		[EditableCheckBox("Show Breadcrumb", 110, ContainerName = LayoutArea)]
		public virtual bool ShowBreadcrumb
		{
			get { return (bool)(GetDetail("ShowBreadcrumb") ?? true); }
			set { SetDetail("ShowBreadcrumb", value, true); }
		}

		public IEnumerable<Site> GetSites()
		{
            Site s = new Site((Parent ?? this).ID, ID, HostName);
            s.Wildcards = true;

            return new Site[] { s };
		}
	}
}
