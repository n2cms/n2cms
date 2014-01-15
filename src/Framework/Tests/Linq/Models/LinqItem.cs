using System;

namespace N2.Extensions.Tests.Linq
{
    [PageDefinition]
    public class LinqItem : ContentItem
    {
        public virtual string StringProperty
        {
            get { return GetDetail("StringProperty", string.Empty); }
            set { SetDetail("StringProperty", value, string.Empty); }
        }

        public virtual string StringProperty2
        {
            get { return GetDetail("StringProperty2", string.Empty); }
            set { SetDetail("StringProperty2", value, string.Empty); }
        }

        public virtual int IntProperty
        {
            get { return GetDetail("IntProperty", 0); }
            set { SetDetail("IntProperty", value); }
        }

        public virtual DateTime DateTimeProperty
        {
            get { return GetDetail("DateTimeProperty", DateTime.MinValue); }
            set { SetDetail("DateTimeProperty", value); }
        }

        public virtual double DoubleProperty
        {
            get { return GetDetail("DoubleProperty", 0); }
            set { SetDetail("DoubleProperty", value); }
        }

        public virtual bool BooleanProperty
        {
            get { return GetDetail("BooleanProperty", false); }
            set { SetDetail("BooleanProperty", value); }
        }

        public virtual ContentItem ContentItemProperty
        {
            get { return GetDetail("ContentItemProperty") as ContentItem; }
            set { SetDetail("ContentItemProperty", value); }
        }
    }
}
