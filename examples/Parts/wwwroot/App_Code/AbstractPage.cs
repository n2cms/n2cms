using N2;
using N2.Details;

/// <summary>
/// This is an abstract class that we can derive from on in all 
/// situations when we want edit the item's title and name.
/// </summary>
[WithEditableTitle("Title", 10)]
[WithEditableName("Name", 20)]
public abstract class AbstractPage : ContentItem
{
}

