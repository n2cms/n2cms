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

		[Details.ThemeSelector("Theme", 74, ContainerName = SiteArea)]
		public virtual string Theme
		{
			get { return (string)(GetDetail("Theme") ?? string.Empty); }
			set { SetDetail("Theme", value); }
		}

		[Details.LayoutSelector("Layout", 76, ContainerName = SiteArea)]
		public virtual string Layout
		{
			get { return (string)(GetDetail("Layout") ?? "~/Layouts/DefaultLayout.Master"); }
			set { SetDetail("Layout", value); }
		}

		[EditableLink("Not found page", 78, ContainerName = SiteArea, HelpText = "Display this page when the requested URL isn't found")]
		public virtual ContentItem NotFoundPage
		{
			get { return (ContentItem)GetDetail("NotFoundPage"); }
			set { SetDetail("NotFoundPage", value); }
		}


		public IEnumerable<Site> GetSites()
		{
			yield return new Site(Parent.ID, ID, HostName);
			yield return new Site(Parent.ID, ID, "www." + HostName);
		}
	}
}
