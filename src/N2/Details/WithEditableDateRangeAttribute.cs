using System;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// Decorates the content item with a date range editable that will update two date fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WithEditableDateRangeAttribute : AbstractEditableAttribute
    {
        protected string betweenText = " - ";
        private string nameEndRange;

        public WithEditableDateRangeAttribute(string title, int sortOrder, string name, string nameEndRange)
            : base(title, sortOrder)
        {
            this.Name = name;
            this.NameEndRange = nameEndRange;
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

        protected override Control AddEditor(Control container)
        {
            DateRange range = new DateRange();
            range.ID = Name + NameEndRange;
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
    }
}
