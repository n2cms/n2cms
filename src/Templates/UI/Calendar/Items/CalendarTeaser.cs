using N2.Collections;
using N2.Details;
using N2.Integrity;

namespace N2.Templates.Calendar.Items
{
	[Definition("Calendar Teaser", "CalendarTeaser")]
	[AllowedZones(Zones.RecursiveRight, Zones.Right)]
	public class CalendarTeaser : Templates.Items.SidebarItem
	{
		[EditableLink("Calendar container", 100)]
		public virtual Calendar Container
		{
			get { return (Calendar)GetDetail("Container"); }
			set { SetDetail("Container", value); }
		}
		
		public override string TemplateUrl
		{
			get
			{
				return "~/Calendar/UI/CalendarTeaser.ascx";
			}
		}

		public override string IconUrl
		{
			get
			{
				return "~/Calendar/UI/Img/calendar_view_month.png";
			}
		}
	}
}
