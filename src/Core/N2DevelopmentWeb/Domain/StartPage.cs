using System.Collections.Generic;
using N2.Details;
using N2.Integrity;
using N2.Web;
using N2.Globalization;
using N2.Web.UI;
using N2;

namespace N2DevelopmentWeb.Domain
{
	[Definition("Start page", "Startpage", "", "Click to use this type.", -20)]//, MayBeRoot = true, MayBeStartPage = true)]
	[RestrictParents(typeof (StartPage), typeof(RootPage))]
	[TabPanel("globalization", "Globalization", 1020)]
	public class StartPage : MyPageData, ISitesSource, ILanguage
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


		#region ILanguageRoot Members

		public virtual string FlagUrl
		{
			get 
			{
				if (string.IsNullOrEmpty(LanguageCode))
					return null;
				else
				{
					string[] parts = LanguageCode.Split('-');
					return "/edit/globalization/flags/" + parts[parts.Length-1] + ".png";
				}
			}
		}

		[EditableTextBox("LanguageTitle", 200, ContainerName = "globalization")]
		public virtual string LanguageTitle
		{
			get { return (string) (GetDetail("LanguageTitle") ?? ""); }
			set { SetDetail("LanguageTitle", value); }
		}

		[EditableLanguagesDropDown("LanguageCode", 300, ContainerName = "globalization")]
		public virtual string LanguageCode
		{
			get { return (string)(GetDetail("LanguageCode") ?? ""); }
			set { SetDetail("LanguageCode", value); }
		}

		#endregion
	}
}