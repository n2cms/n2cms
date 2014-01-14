using System;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// Decorates the content item with a date range editable that will update two date fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class WithEditableDateRangeAttribute : AbstractEditableAttribute, IWritingDisplayable, IDisplayable
    {
        private string betweenText = " - ";
        private string nameEndRange;

        public WithEditableDateRangeAttribute()
            : this("Dates", 20, "From", "To")
        {
        }

        public WithEditableDateRangeAttribute(string title, int sortOrder, string name, string nameEndRange)
            : base(title, sortOrder)
        {
            Name = name;
            NameEndRange = nameEndRange;
        }

        /// <summary>End of range detail (property) on the content item's object</summary>
        public string NameEndRange
        {
            get { return nameEndRange; }
            set { nameEndRange = value; }
        }

        /// <summary>Gets or sets a text displayed between the date fields.</summary>
        public string BetweenText
        {
            get { return betweenText; }
            set { betweenText = value; }
        }

        /// <summary>From date placehodlder text. This is equivalent to <see cref="Placeholder"/>.</summary>
        public string FromDatePlaceholder
        {
            get { return Placeholder; }
            set { Placeholder = value; }
        }
        
        /// <summary>From time placeholder text.</summary>
        public string FromTimePlaceholder { get; set; }
        
        /// <summary>To date placeholder text.</summary>
        public string ToDatePlaceholder { get; set; }
        
        /// <summary>To time placehodler text.</summary>
        public string ToTimePlaceholder { get; set; }

        protected override Control AddEditor(Control container)
        {
            DateRange range = new DateRange();
            range.ID = Name + NameEndRange;
            range.FromDatePicker.DatePickerBox.Placeholder(GetLocalizedText("FromDatePlaceholder") ?? GetLocalizedText("Placeholder") ?? FromDatePlaceholder);
            range.FromDatePicker.TimePickerBox.Placeholder(GetLocalizedText("FromTimePlaceholder") ?? FromTimePlaceholder);
            range.ToDatePicker.DatePickerBox.Placeholder(GetLocalizedText("ToDatePlaceholder") ?? ToDatePlaceholder);
            range.ToDatePicker.TimePickerBox.Placeholder(GetLocalizedText("ToTimePlaceholder") ?? ToTimePlaceholder);
            container.Controls.Add(range);
            range.BetweenText = GetLocalizedText("BetweenText") ?? BetweenText;
            return range;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            DateRange range = (DateRange)editor;
            range.From = (DateTime?)item[this.Name];
            range.To = (DateTime?)item[this.NameEndRange];
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            DateRange range = editor as DateRange;
            if ((DateTime?)item[this.Name] != range.From || (DateTime?)item[this.NameEndRange] != range.To)
            {
                item[this.Name] = range.From;
                item[this.NameEndRange] = range.To;
                return true;
            }
            return false;
        }

        #region IWritingDisplayable Members

        public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            writer.Write(item[propertyName] + BetweenText + item[NameEndRange]);
        }

        #endregion
    }
}
