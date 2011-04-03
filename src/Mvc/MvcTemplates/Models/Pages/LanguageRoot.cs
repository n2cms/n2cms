using System;
using System.Globalization;
using System.Web.UI.WebControls;
using N2.Engine.Globalization;
using N2.Details;
using N2.Persistence.Serialization;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.UI;
using N2.Integrity;
using N2.Definitions;
using N2.Security;

namespace N2.Templates.Mvc.Models.Pages
{
	//[Definition("Language root", "LanguageRoot", "A starting point for translations of the start page.", "", 450)]
	[PageDefinition("Language root",
		Description = "A starting point for translations of the start page.",
		SortOrder = 450,
		IconUrl = "~/Content/Img/page_world.png")]
	[TabContainer(LanguageRoot.SiteArea, "Site", 70, 
		RequiredPermission = Permission.Administer)]
	[RestrictParents(typeof (StartPage))]
	[FieldSetContainer(StartPage.MiscArea, "Miscellaneous", 80, ContainerName = LanguageRoot.SiteArea)]
	[FieldSetContainer(StartPage.LayoutArea, "Layout", 75, ContainerName = LanguageRoot.SiteArea)]
	public class LanguageRoot : ContentPageBase, IStructuralPage, ILanguage, IStartPage
	{
		public LanguageRoot()
		{
			Visible = false;
			SortOrder = 10000;
		}

		public const string SiteArea = "siteArea";
		public const string MiscArea = "miscArea";

		#region ILanguage Members

		public string FlagUrl
		{
			get
			{
				if (string.IsNullOrEmpty(LanguageCode))
					return "";

				string[] parts = LanguageCode.Split('-');
				return String.Format("~/N2/Resources/Img/Flags/{0}.png", parts[parts.Length - 1].ToLowerInvariant());
			}
		}

		[EditableLanguagesDropDown("Language", 100, ContainerName = MiscArea)]
		public string LanguageCode
		{
			get { return (string) GetDetail("LanguageCode"); }
			set { SetDetail("LanguageCode", value); }
		}

		public string LanguageTitle
		{
			get
			{
				if (string.IsNullOrEmpty(LanguageCode))
					return "";
				else
					return new CultureInfo(LanguageCode).DisplayName;
			}
		}

		#endregion

		[FileAttachment, EditableFileUploadAttribute("Top Image", 88, ContainerName = Tabs.Content, CssClass = "top")]
		public virtual string TopImage
		{
			get { return (string) (GetDetail("TopImage") ?? string.Empty); }
			set { SetDetail("TopImage", value, string.Empty); }
		}

		[FileAttachment, EditableFileUploadAttribute("Content Image", 90, ContainerName = Tabs.Content, CssClass = "main")]
		public virtual string Image
		{
			get { return (string) (GetDetail("Image") ?? string.Empty); }
			set { SetDetail("Image", value, string.Empty); }
		}

		[EditableText("Footer Text", 80, ContainerName = MiscArea, TextMode = TextBoxMode.MultiLine, Rows = 3)]
		public virtual string FooterText
		{
			get { return (string) (GetDetail("FooterText") ?? string.Empty); }
			set { SetDetail("FooterText", value, string.Empty); }
		}

		[EditableItem("Header", 100, ContainerName = SiteArea)]
		public virtual Top Header
		{
			get { return (Top) GetDetail("Header"); }
			set { SetDetail("Header", value); }
		}
	}
}