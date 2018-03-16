using System;
using System.Data.SqlTypes;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// Defines an editable date/time picker control for a content item.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableSqlDateAttribute : EditableDateAttribute
    {
        public EditableSqlDateAttribute()
            : base(null, 20)
        {
        }

        public EditableSqlDateAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        private DateTime? ConvertToSqlSafeDateTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;
            
            if (dateTime.Value == DateTime.MinValue)
            {
                dateTime = (DateTime?)SqlDateTime.MinValue.Value;
            }
            else if (dateTime.Value.Year == DateTime.MinValue.Year 
                && dateTime.Value.Month == DateTime.MinValue.Month 
                && dateTime.Value.Day == DateTime.MinValue.Day)
            {
                //We're only using time. Use SqlDateTime.MinValue for the date portion.
                dateTime = (DateTime?)new DateTime(SqlDateTime.MinValue.Value.Year, SqlDateTime.MinValue.Value.Month, SqlDateTime.MinValue.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute, dateTime.Value.Second, dateTime.Value.Millisecond);
            }

            return dateTime;
        }
        
        public override bool UpdateItem(ContentItem item, Control editor)
        {
            DatePicker picker = (DatePicker)editor;
            picker.SelectedDate = ConvertToSqlSafeDateTime(picker.SelectedDate);

            return base.UpdateItem(item, picker);
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            base.UpdateEditor(item, editor);

            DatePicker picker = (DatePicker)editor;
            picker.SelectedDate = ConvertToSqlSafeDateTime(picker.SelectedDate);
        }
    }
}
