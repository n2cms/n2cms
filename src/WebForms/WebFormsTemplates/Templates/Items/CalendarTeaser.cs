using N2.Details;
using N2.Integrity;

namespace N2.Templates.Items
{
    [PartDefinition("Calendar Teaser",
        IconUrl = "~/Templates/UI/Img/calendar_view_month.png")]
    [AllowedZones(Zones.RecursiveRight, Zones.RecursiveLeft, Zones.Right, Zones.Left)]
    public class CalendarTeaser : SidebarItem
    {
        [EditableLink("Calendar container", 100)]
        public virtual Calendar Container
        {
            get { return (Calendar)GetDetail("Container"); }
            set { SetDetail("Container", value); }
        }

        protected override string TemplateName
        {
            get { return "CalendarTeaser"; }
        }
    }
}
