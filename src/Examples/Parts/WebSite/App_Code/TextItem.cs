using N2;
using N2.Details;
using N2.Integrity;

[Definition("Text item")]
[WithEditableTitle("Title", 10, Required = false)]
[AllowedZones("Content", "Secondary")]
public class TextItem : AbstractItem
{
	[EditableFreeTextArea("Text", 100)]
	public virtual string Text
	{
		get { return (string)GetDetail("Text"); }
		set { SetDetail("Text", value); }
	}
}
