using N2;
using N2.Details;
using N2.Integrity;

[Definition("List")]
[WithEditableTitle("Title", 10)]
[AllowedZones("Content", "Left", "Right")]
public class List : AbstractItem
{
	[EditableLink("ListSubpagesOf", 100)]
	public virtual ContentItem ListSubpagesOf
	{
		get { return (ContentItem)GetDetail("ListSubpagesOf"); }
		set { SetDetail("ListSubpagesOf", value); }
	}
}
