using N2.Details;
namespace N2.Tests.Edit.Items
{
	[PageDefinition]
	public class NormalPage : ContentItem
	{
		[EditableEnum(typeof(WidthType))]
		public virtual WidthType WidthType { get; set; }
		
		[EditableNumber]
		public virtual int Width { get; set; }
	}
}
