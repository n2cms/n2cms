using N2.Details;
namespace N2.Tests.Edit.Items
{
    [Definition]
    public class NormalItem : ContentItem
    {
        [EditableEnum(typeof(WidthType))]
        public virtual WidthType WidthType { get; set; }

        [EditableNumber]
        public virtual int Width { get; set; }

        [EditableLink]
        public virtual ContentItem EditableLink { get; set; }

        public override bool IsPage
        {
            get
            {
                return false;
            }
        }
    }
}
