using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.Items
{
    [Definition("News List", "NewsList", "A news list box that can be displayed in a column.", "", 160)]
    [WithEditableTitle("Title", 10, Required = false)]
    [AllowedZones(Zones.RecursiveRight, Zones.RecursiveLeft, Zones.Right, Zones.Left, Zones.Content, Zones.ColumnLeft, Zones.ColumnRight)]
    public class NewsList : SidebarItem
    {
        [EditableLink("News container", 100)]
        public virtual NewsContainer Container
        {
            get { return (NewsContainer) GetDetail("Container"); }
            set { SetDetail("Container", value); }
        }

        [EditableTextBox("Max news", 120)]
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

        protected override string IconName
        {
            get { return "newspaper_go"; }
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