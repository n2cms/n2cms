using N2.Details;
namespace N2.Tests.Edit.Items
{
    [PageDefinition]
    public class NormalPage : ContentItem
    {
        [EditableEnum(typeof(WidthType))]
        public virtual WidthType WidthType { get; set; }
        
        [EditableNumber(DefaultValue = 2)]
        public virtual int Width { get; set; }

        [EditableLink]
        public virtual ContentItem EditableLink { get; set; }

        [EditableItem]
        public virtual NormalItem EditableItem { get; set; }
    }
}
