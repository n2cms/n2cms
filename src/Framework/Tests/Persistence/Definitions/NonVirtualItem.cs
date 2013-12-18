using System;
using N2.Definitions;

namespace N2.Tests.Persistence.Definitions
{
    [PageDefinition]
    [SiblingInsertion(SortBy.Unordered)]
    [SortChildren(SortBy.Unordered)]
    public class NonVirtualItem : ContentItem
    {
        public bool BoolProperty
        {
            get { return (bool)(GetDetail("BoolProperty") ?? true); }
            set { SetDetail<bool>("BoolProperty", value); }
        }

        public int IntProperty
        {
            get { return (int)(GetDetail("IntProperty") ?? 0); }
            set { SetDetail<int>("IntProperty", value); }
        }

        public DateTime DateTimeProperty
        {
            get { return (DateTime)(GetDetail("DateTimeProperty") ?? DateTime.MinValue); }
            set { SetDetail<DateTime>("DateTimeProperty", value); }
        }
        public double DoubleProperty
        {
            get { return GetDetail("DoubleProperty", 0.0); }
            set { SetDetail<double>("DoubleProperty", value, 0.0); }
        }
        public string StringProperty
        {
            get { return (string)(GetDetail("StringProperty") ?? string.Empty); }
            set { SetDetail<string>("StringProperty", value); }
        }
        public ContentItem LinkProperty
        {
            get { return (ContentItem)GetDetail("LinkProperty"); }
            set { SetDetail<ContentItem>("LinkProperty", value); }
        }
        public object ObjectProperty
        {
            get { return (object)GetDetail("ObjectProperty"); }
            set { SetDetail<object>("ObjectProperty", value); }
        }
    }
}
