using N2;
using N2.Details;
using N2.Integrity;

/// <summary>
/// 
/// </summary>
[Definition("List")]
[WithEditableTitle("Title", 10)]
[AllowedZones("Content", "Left", "Right")] // This tells N2 that the List part may be in any of the zones "Content", "Left" or "Right"
public class List : AbstractItem
{
	[EditableLink("ListSubpagesOf", 100)]
	public virtual ContentItem ListSubpagesOf
	{
		get { return (ContentItem)GetDetail("ListSubpagesOf"); }
		set { SetDetail("ListSubpagesOf", value); }
	}
}
