using System;
using System.Collections.Generic;
using System.Text;
using N2.Web;
using N2.Details;

namespace N2.Templates.Items
{
	public abstract class AbstractStartPage : AbstractContentPage, IStructuralPage, ISitesSource
	{
		public const string SiteArea = "siteArea";

		[EditableTextBox("Host Name", 72, ContainerName = SiteArea)]
		public virtual string HostName
		{
			get { return (string)(GetDetail("HostName") ?? string.Empty); }
			set { SetDetail("HostName", value); }
		}

		public abstract string Theme { get; set; }
		public abstract string Layout { get; set; }

		[EditableLink("Not found page", 78, ContainerName = SiteArea, HelpText = "Display this page when the requested URL isn't found")]
		public virtual ContentItem NotFoundPage
		{
			get { return (ContentItem)GetDetail("NotFoundPage"); }
			set { SetDetail("NotFoundPage", value); }
		}

		[EditableCheckBox("Show Breadcrumb", 110, ContainerName = SiteArea)]
		public virtual bool ShowBreadcrumb
		{
			get { return (bool)(GetDetail("ShowBreadcrumb") ?? true); }
			set { SetDetail("ShowBreadcrumb", value, true); }
		}

		public IEnumerable<Site> GetSites()
		{
            Site s = new Site(Parent.ID, ID, HostName);
            s.Wildcards = true;

            return new Site[] { s };
		}
	}
}
