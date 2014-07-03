using System;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Services;
using N2.Web.Mvc;
using N2.Persistence;

namespace N2.Templates.Mvc.Models.Pages
{
    [PageDefinition("Event",
        Description = "An event in the event calendar.",
        SortOrder = 110,
        IconClass = "fa fa-calendar-empty")]
    [RestrictParents(typeof (Calendar))]
    public class Event : ContentPageBase, ISyndicatable
    {
        [EditableDate("Event date", 22, ContainerName = Tabs.Content)]
        [Persistable]
        public virtual DateTime? EventDate { get; set; }

        [DefaultValue(false)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        [DisplayableLiteral]
        public virtual string EventDateString
        {
            get
            {
                if (!EventDate.HasValue) return null;
                if (EventDate.Value.TimeOfDay.TotalSeconds == 0) return EventDate.Value.ToShortDateString();

                return EventDate.Value.ToString();
            }
        }

        [Obsolete("Use Summary")]
        [DisplayableLiteral]
        public virtual string Introduction { get { return Summary; } }

        [Persistable(PersistAs = PropertyPersistenceLocation.Detail)]
        public virtual bool Syndicate { get; set; }
    }
}
