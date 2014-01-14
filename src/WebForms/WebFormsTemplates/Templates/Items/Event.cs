using System;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Templates.Services;
using N2.Web;
using N2.Persistence;

namespace N2.Templates.Items
{
    [PageDefinition("Event", 
        Description = "An event in the event calendar.", 
        SortOrder = 110,
        IconUrl = "~/Templates/UI/Img/calendar_view_day.png")]
    [RestrictParents(typeof(Calendar))]
    [ConventionTemplate("CalendarItem")]
    public class Event : AbstractContentPage, ISyndicatable
    {
        public Event()
        {
            Visible = false;
        }

        [EditableDate("Event date", 22, ContainerName = Tabs.Content)]
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

        [EditableText("Introduction", 90, ContainerName = Tabs.Content, TextMode = TextBoxMode.MultiLine, Rows = 4, Columns = 80)]
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

        [Persistable(PersistAs = PropertyPersistenceLocation.Detail)]
        public virtual bool Syndicate { get; set; }
    }
}
