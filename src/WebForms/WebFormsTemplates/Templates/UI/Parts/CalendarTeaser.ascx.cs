using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Resources;
using N2.Templates.Items;
using N2.Templates.Web.UI;
using N2.Web;

namespace N2.Templates.UI.Parts
{
    public partial class CalendarTeaser : TemplateUserControl<ContentItem, Templates.Items.CalendarTeaser>
    {
        protected System.Web.UI.WebControls.Calendar cEvents;
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            Resources.Register.StyleSheet(Page, "~/Templates/UI/Css/Calendar.css", Media.All);

            cEvents.SelectionChanged += new System.EventHandler(cEvents_SelectionChanged);
            if(CurrentItem.Container != null)
            {
                foreach(Event calendarEvent in CurrentItem.Container.GetEvents())
                {
                    if (calendarEvent.EventDate.HasValue)
                        cEvents.SelectedDates.Add(calendarEvent.EventDate.Value);
                }
            }
        }

        void cEvents_SelectionChanged(object sender, EventArgs e)
        {
            DateTime date = cEvents.SelectedDate.Date;
            if(CurrentItem.Container != null)
            {
                IList<Event> events = CurrentItem.Container.GetEvents(date);
                if(events.Count == 1) Response.Redirect(events[0].Url);

                Url url = CurrentItem.Container.Url;
                url = url.AppendQuery("date", date);
                url = url.AppendQuery("filter", "true");
                Response.Redirect(url);
            }
        }
    }
}
