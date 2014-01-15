using System;
using System.Collections.Generic;
using N2.Templates.Mvc.Models.Pages;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
    public class CalendarModel : IItemContainer<Calendar>
    {
        public CalendarModel(Calendar currentItem, ICollection<Event> results)
        {
            CurrentItem = currentItem;
            Events = results;
        }

        /// <summary>Gets the item associated with the item container.</summary>
        ContentItem IItemContainer.CurrentItem
        {
            get { return CurrentItem; }
        }

        public Calendar CurrentItem { get; private set; }
        public ICollection<Event> Events { get; private set; }
    }
}
