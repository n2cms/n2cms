using System.Collections.Generic;
using N2.Details;
using N2.Integrity;
using N2.Web;

namespace N2.TemplateWeb.Domain
{
	[Definition("Start page", "Startpage", "", "Click to use this type.", -20)]//, MayBeRoot = true, MayBeStartPage = true)]
	[RestrictParents(typeof (StartPage))]
	public class StartPage : MyPageData, ISitesSource
	{
		[EditableCheckBox("A checkbox", 20, ContainerName="default")]
		public virtual bool BoolProperty
		{
			get { return (bool) (GetDetail("BoolProperty") ?? false); }
			set { SetDetail("BoolProperty", value); }
		}

		public IEnumerable<Site> GetSites()
		{
			return new Site[]
				{
					new Site(Parent != null ? Parent.ID : ID, ID, Host),
					new Site(Parent != null ? Parent.ID : ID, ID, "www." + Host)
				};
		}

		[EditableTextBox("Host", 300, ContainerName="special")]
		public virtual string Host
		{
			get { return (string) (GetDetail("Host") ?? ""); }
			set { SetDetail("Host", value); }
		}


		[N2.Details.EditableImage("ImageUrl", 100, ContainerName = "default")]
		public virtual string ImageUrl
		{
			get { return (string)(GetDetail("ImageUrl") ?? string.Empty); }
			set { SetDetail("ImageUrl", value, string.Empty); }
		}

	}
}