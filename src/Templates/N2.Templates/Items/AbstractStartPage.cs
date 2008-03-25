using System;
using System.Collections.Generic;
using System.Text;
using N2.Web;
using N2.Details;

namespace N2.Templates.Items
{
	public abstract class AbstractStartPage : AbstractContentPage, IStructuralPage, ISitesSource
	{

		[EditableTextBox("Host Name", 72, ContainerName = "siteArea")]
		public virtual string HostName
		{
			get { return (string)(GetDetail("HostName") ?? string.Empty); }
			set { SetDetail("HostName", value); }
		}

		[Details.ThemeSelector("Theme", 74, ContainerName = "siteArea")]
		public virtual string Theme
		{
			get { return (string)(GetDetail("Theme") ?? string.Empty); }
			set { SetDetail("Theme", value); }
		}

		[Details.LayoutSelector("Layout", 76, ContainerName = "siteArea")]
		public virtual string Layout
		{
			get { return (string)(GetDetail("Layout") ?? "~/Layouts/DefaultLayout.Master"); }
			set { SetDetail("Layout", value); }
		}

		public IEnumerable<Site> GetSites()
		{
			yield return new Site(Parent.ID, ID, HostName);
			yield return new Site(Parent.ID, ID, "www." + HostName);
		}
	}
}
