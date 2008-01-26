using N2.Details;

/// <summary>
/// This is an abstract class that we can derive from on in all 
/// situations when we want edit the item's title and name.
/// </summary>
[WithEditable("Name", typeof(N2.Web.UI.WebControls.NameEditor), "Text", 20, "Name")]
public abstract class MyItemBase : N2.ContentItem
{
	[DisplayableLiteral()]
	[EditableTextBox("Title", 10, Required=true)]
	public override string Title
	{
		get { return base.Title; }
		set { base.Title = value; }
	}
}
