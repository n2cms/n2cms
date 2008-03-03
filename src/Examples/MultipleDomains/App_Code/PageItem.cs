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
public class PageItem : MyItemBase
{
	[N2.Details.EditableFreeTextArea("Text", 100)]
    public virtual string Text
    {
        get { return (string)GetDetail("Text"); }
        set { SetDetail("Text", value); }
    }

	[N2.Details.EditableUrl("Url", 120)]	
	public virtual string NextSite
	{
	    get { return (string)(GetDetail("NextSite") ?? string.Empty); }
	    set { SetDetail("NextSite", value, string.Empty); }
	}

}
