using System;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Integrity;
using N2.Templates.Services;

namespace N2.Templates.Items
{
    [Definition("Event", "Event", "An event item", "", 110)]
    [RestrictParents(typeof(Calendar))]
    public class Event : AbstractContentPage, ISyndicatable
    {
        public Event()
        {
            Visible = false;
        }

        [Editable("Event date", typeof(N2.Web.UI.WebControls.DatePicker), "SelectedDate", 22, ContainerName = Tabs.Content)]
        public virtual DateTime? EventDate
        {
            get { return (DateTime?)GetDetail("EventDate"); }
            set { SetDetail("EventDate", value); }
        }

    	public virtual string EventDateString
    	{
    		get
    		{
    			if (!EventDate.HasValue) return null;
    			if (EventDate.Value.TimeOfDay.TotalSeconds == 0) return EventDate.Value.ToShortDateString();
    			
				return EventDate.Value.ToString();
    		}
    	}

        [EditableTextBox("Introduction", 90, ContainerName = Tabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 4, Columns = 80)]
        public virtual string Introduction
        {
            get { return (string)(GetDetail("Introduction") ?? string.Empty); }
            set { SetDetail("Introduction", value, string.Empty); }
        }

        public override void AddTo(ContentItem newParent)
        {
            Utility.Insert(this, newParent, "EventDate");
        }

        string ISyndicatable.Summary
        {
            get { return Introduction; }
        }

        protected override string IconName
        {
            get { return "calendar_view_day"; }
        }

        protected override string TemplateName
        {
            get { return "CalendarItem"; }
        }
    }
}