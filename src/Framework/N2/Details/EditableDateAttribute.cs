using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// Defines an editable date/time picker control for a content item.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableDateAttribute : AbstractEditableAttribute, IDisplayable, IWritingDisplayable
    {
        bool showDate = true;
        bool showTime = true;
        bool useTodayAsDefault = false;

        public EditableDateAttribute()
            : base(null, 20)
        {
        }

        public EditableDateAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        /// <summary>Set to false to hide time box.</summary>
        public bool ShowTime
        {
            get { return showTime; }
            set { showTime = value; }
        }

        /// <summary>Set to show the current date and time as default value.</summary>
        public bool UseTodayAsDefault
        {
            get { return useTodayAsDefault; }
            set { useTodayAsDefault = value; }
        }

        /// <summary>Set to false to hide date box.</summary>
        public bool ShowDate
        {
            get { return showDate; }
            set { showDate = value; }
        }

        /// <summary>Gets or sets the placeholder text for the time box.</summary>
        public string TimePlaceholder { get; set; }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            DatePicker picker = (DatePicker)editor;
            var value = item[Name];
            if (value == null)
            {
                item[Name] = picker.SelectedDate;
                return true;
            }
            if (!(value is DateTime) || (DateTime?)value != picker.SelectedDate)
            {
                item[Name] = picker.SelectedDate;
                return true;
            }
            return false;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            DatePicker picker = (DatePicker) editor;
            object value = item[Name];
            if (value == null)
            {
                if (useTodayAsDefault == true)
                    picker.SelectedDate = N2.Utility.CurrentTime();
                else
                    picker.SelectedDate = null;
            }
            else if ((DateTime)value == DateTime.MinValue)
            {
                if (useTodayAsDefault == true)
                    picker.SelectedDate = N2.Utility.CurrentTime();
                else
                    picker.SelectedDate = (DateTime?)value;
            }
            else if (value is DateTime?)
                picker.SelectedDate = (DateTime?)value;
            else if (value is DateTime)
                picker.SelectedDate = (DateTime?)value;
            else
                picker.SelectedDate = Utility.Convert<DateTime>(value);
        }

        protected override Control AddEditor(Control container)
        {
            DatePicker picker = new DatePicker();
            picker.TimePickerBox.Visible = ShowTime;
            picker.DatePickerBox.Visible = ShowDate;
            picker.ID = Name;
            picker.DatePickerBox.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
            picker.TimePickerBox.Placeholder(GetLocalizedText("TimePlaceholder") ?? TimePlaceholder);
            
            container.Controls.Add(picker);
            
            return picker;
        }

        #region IDisplayable Members

        Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
        {
            DateTime? date = item[Name] as DateTime?;
            if(date.HasValue)
            {
                Literal l = new Literal();
                l.Text = date.Value.ToString();
                container.Controls.Add(l);
                return l;
            }
            return null;
        }

        #endregion

        #region IWritingDisplayable Members

        public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            writer.Write(item[propertyName]);
        }

        #endregion
    }
}
