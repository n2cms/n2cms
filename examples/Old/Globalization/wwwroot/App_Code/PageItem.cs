using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Details;
using LanguageSwitcher;

/// <summary>
/// This is my custom content data container class. Since this class derives 
/// from <see cref="N2.ContentItem"/> N2 will pick up this class and make it 
/// available when we create pages in edit mode. 
/// 
/// Another thing to notice is that in addition to Text defined to be editable 
/// in this class it's Title and Name are also editable. This is because of the 
/// abstract base class <see cref="MyItemBase"/> it derives from.
/// </summary>
[N2.Item("Default page", "PageItem")]
[WithEditable("Name", typeof(TextBox), "Text", -10, "Name", Required = true)]
[N2.Integrity.AvailableZone("Right column", "Right")]
public class PageItem : LanguageAwareItemBase
{
	[EditableFreeTextArea("Text", 100, ContainerName = "translated")]
    public virtual string Text
    {
        get { return (string)GetDetail("Text"); }
        set { SetDetail("Text", value); }
    }
}
