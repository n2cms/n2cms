using N2.Details;

namespace N2.Tests.Details.Models
{
    public enum enumDays { Mon = 1, Tue = 2, Wed = 3, Thu = 4, Fri = 5, Sat = 6, Sun = 7 };

    public class EnumableItem : ContentItem
    {
        [EditableEnum("Days", 30, typeof(enumDays))]
        public virtual string DaysString
        {
            get { return (string)GetDetail("DaysString"); }
            set { SetDetail("DaysString", value); }
        }

        [EditableEnum("Days", 30, typeof(enumDays))]
        public virtual enumDays DaysEnum
        {
            get { return (enumDays)GetDetail("DaysEnum"); }
            set { SetDetail("DaysEnum", (int)value); }
        }

        [EditableEnum("Days", 30, typeof(enumDays))]
        public virtual int DaysInteger
        {
            get { return (int)GetDetail("DaysInteger"); }
            set { SetDetail("DaysInteger", value); }
        }
    }
}
