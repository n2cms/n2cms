using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Calendar Teaser",
        IconUrl = "~/Content/Img/calendar_view_month.png")]
    public class CalendarTeaser : SidebarItem
    {
        [EditableLink("Calendar container", 100)]
        public virtual Calendar Container
        {
            get { return (Calendar) GetDetail("Container"); }
            set { SetDetail("Container", value); }
        }
    }
}
