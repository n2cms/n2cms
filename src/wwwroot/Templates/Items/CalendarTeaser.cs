using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items
{
    [Definition("Calendar Teaser", "CalendarTeaser")]
    [AllowedZones(Zones.RecursiveRight, Zones.RecursiveLeft, Zones.Right, Zones.Left)]
    public class CalendarTeaser : Templates.Items.SidebarItem
    {
        [EditableLink("Calendar container", 100)]
        public virtual Calendar Container
        {
            get { return (Calendar)GetDetail("Container"); }
            set { SetDetail("Container", value); }
        }

        protected override string IconName
        {
            get { return "calendar_view_month"; }
        }

        protected override string TemplateName
        {
            get { return "CalendarTeaser"; }
        }
    }
}