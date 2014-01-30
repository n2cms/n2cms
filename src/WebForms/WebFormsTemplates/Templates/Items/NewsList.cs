using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [PartDefinition("News List", 
        Description = "A news list box that can be displayed in a column.", 
        SortOrder = 160,
        IconUrl = "~/Templates/UI/Img/newspaper_go.png")]
    [WithEditableTitle("Title", 10, Required = false)]
    [AllowedZones(Zones.RecursiveRight, Zones.RecursiveLeft, Zones.Right, Zones.Left, Zones.Content, Zones.ColumnLeft, Zones.ColumnRight)]
    public class NewsList : SidebarItem
    {
        public enum HeadingLevel
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }

        [EditableEnum("Title heading level", 90, typeof(HeadingLevel))]
        public virtual int TitleLevel
        {
            get { return (int)(GetDetail("TitleLevel") ?? 3); }
            set { SetDetail("TitleLevel", value, 3); }
        }

        [EditableLink("News container", 100)]
        public virtual NewsContainer Container
        {
            get { return (NewsContainer) GetDetail("Container"); }
            set { SetDetail("Container", value); }
        }

        [EditableNumber("Max news", 120)]
        public virtual int MaxNews
        {
            get { return (int) (GetDetail("MaxNews") ?? 3); }
            set { SetDetail("MaxNews", value, 3); }
        }

        public virtual void Filter(ItemList items)
        {
            TypeFilter.Filter(items, typeof (News));
            CountFilter.Filter(items, 0, MaxNews);
        }

        protected override string TemplateName
        {
            get { return InTheMiddle() ? "NewsList" : "NewsBox"; }
        }

        private bool InTheMiddle()
        {
            return ZoneName == Zones.Content || ZoneName == Zones.ColumnLeft || ZoneName == Zones.ColumnRight;
        }
    }
}
