using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using LanguageSwitcher;

/// <summary>
/// An item that can be placed in the right column that swtiches languages.
/// </summary>
[N2.Item("Language switcher", "LanguageSwitcher")]
[N2.Integrity.AllowedZones("Right")]
public class LanguageSwitcherItem : LanguageAwareItemBase
{

	[N2.Details.EditableTextBox("Languages", 100, ContainerName = "translated")]	
	[N2.Web.UI.EditorModifier("TextMode", TextBoxMode.MultiLine)]
	public virtual string Languages
	{
		get { return (string)(GetDetail("Languages") ?? "ENG"); }
		set { SetDetail("Languages", value); }
	}

	public override string TemplateUrl
	{
		get
		{
			return "~/Languaging/LanguageSwitcher.ascx";
		}
	}

	public override bool IsPage
	{
		get
		{
			return false;
		}
	}
}
