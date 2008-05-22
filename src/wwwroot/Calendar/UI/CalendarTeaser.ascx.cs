using System;
using N2.Resources;
using N2.Templates.Items;
using N2.Templates.Web.UI;

namespace N2.Templates.Calendar.UI
{
	public partial class CalendarTeaser : TemplateUserControl<AbstractContentPage, Items.CalendarTeaser>
	{
		protected System.Web.UI.WebControls.Calendar cEvents;
		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			Register.StyleSheet(Page, "/Calendar/UI/Css/Calendar.css", Media.All);

			cEvents.SelectionChanged += new System.EventHandler(cEvents_SelectionChanged);
			if(CurrentItem.Container != null)
			{
				foreach(Items.Event calendarEvent in CurrentItem.Container.GetEvents())
				{
					if (calendarEvent.EventDate.HasValue)
						cEvents.SelectedDates.Add(calendarEvent.EventDate.Value);
				}
			}
		}

		void cEvents_SelectionChanged(object sender, System.EventArgs e)
		{
			DateTime date = cEvents.SelectedDate.Date;
			if(CurrentItem.Container != null)
			{
				foreach (Items.Event calendarEvent in CurrentItem.Container.GetEvents())
				{
					if(calendarEvent.EventDate.HasValue && calendarEvent.EventDate.Value.Date == date)
						Response.Redirect(calendarEvent.Url);
				}
			}
			Response.Redirect(CurrentItem.Container.Url + "?date=" + date);
		}
	}
}