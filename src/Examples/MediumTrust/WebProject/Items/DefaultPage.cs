using N2;
using N2.Details;
using N2.Integrity;

namespace MediumTrustTest.Items
{
	[WithEditableTitle("Title", 10)]
	[WithEditableName("Name", 20)]
	[WithEditablePublishedRange("Published between", 30)]
	[AvailableZone("Content", "Content")]
	public class DefaultPage : ContentItem
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}
	}
}
