using System;
using System.Collections.Generic;
using N2.Persistence;
using N2.Persistence.Search;
using N2.Details;
using N2.Definitions;

namespace N2.Tests.Persistence.Definitions
{
    [PartDefinition("Default persistable part", Name = "PersistablePart")]
    public class PersistablePart : ContentItem, IPart
    {
        [Persistable]
        public virtual string PersistableProperty { get; set; }

        [EditableDummy]
        public virtual List<string> StringList
        {
            get { return (List<string>)GetDetail("StringList"); }
            set { SetDetail("StringList", value); }
        }

        [EditableLink]
        public virtual ContentItem EditableLink { get; set; }
    }

    [PageDefinition("Default persistable Item", Name = "PersistableItem")]
    public class PersistableItem : N2.ContentItem, IPage
    {
        public virtual bool BoolProperty
        {
            get { return (bool)(GetDetail("BoolProperty") ?? true); }
            set { SetDetail<bool>("BoolProperty", value); }
        }

        [EditableNumber(DefaultValue = 666)]
        public virtual int IntProperty
        {
            get { return (int)(GetDetail("IntProperty") ?? 0); }
            set { SetDetail<int>("IntProperty", value); }
        }

        [EditableDate]
        public virtual DateTime DateTimeProperty
        {
            get { return (DateTime)(GetDetail("DateTimeProperty") ?? DateTime.MinValue); }
            set { SetDetail<DateTime>("DateTimeProperty", value); }
        }
        public virtual double DoubleProperty
        {
            get { return Convert.ToDouble(GetDetail("DoubleProperty") ?? 0); }
            set { SetDetail<double>("DoubleProperty", value); }
        }
        [Indexable]
        public virtual string StringProperty
        {
            get { return (string)(GetDetail("StringProperty") ?? string.Empty); }
            set { SetDetail<string>("StringProperty", value); }
        }

        public virtual ContentItem LinkProperty
        {
            get { return (ContentItem)GetDetail("LinkProperty"); }
            set { SetDetail<ContentItem>("LinkProperty", value); }
        }

        [Editable(Name = "ContentLinks", PersistAs = PropertyPersistenceLocation.DetailCollection)]
        public virtual IEnumerable<ContentItem> ContentLinks
        {
            get
            {
                var dc = GetDetailCollection("ContentLinks", false);
                if (dc == null)
                    return new ContentItem[0];

                return dc.Enumerate<ContentItem>();
            }
            set
            {
                var dc = GetDetailCollection("ContentLinks", true);
                dc.Replace(value);
            }
        }

        public virtual object ObjectProperty
        {
            get { return (object)GetDetail("ObjectProperty"); }
            set { SetDetail<object>("ObjectProperty", value); }
        }

        [EditableEnum]
        public virtual AppDomainManagerInitializationOptions EnumProperty
        {
            get { return GetDetail("EnumProperty", AppDomainManagerInitializationOptions.None); }
            set { SetDetail("EnumProperty", value, AppDomainManagerInitializationOptions.None); }
        }

        public virtual Guid GuidProperty
        {
            get
            {
                string value = GetDetail<string>("GuidProperty", null);
                return string.IsNullOrEmpty(value) ? Guid.Empty : new Guid(value);
            }
            set
            {
                SetDetail("GuidProperty", value.ToString());
            }
        }

        public virtual string WritableGuid
        {
            get { return (string)(GetDetail("WritableRSSString") ?? Guid.NewGuid().ToString()); }
            set { SetDetail("WritableRSSString", value, Guid.NewGuid().ToString()); }
        }

        public virtual string ReadOnlyGuid
        {
            get
            {
                string result = (string)GetDetail("ReadOnlyRSSString");
                if (string.IsNullOrEmpty(result))
                {
                    result = Guid.NewGuid().ToString();
                    SetDetail("ReadOnlyRSSString", result);
                }
                return result;
            }
        }

        [Indexable]
        public virtual string NonDetailProperty { get; set; }

        [Indexable]
        public virtual string NonDetailOnlyGetterProperty { get { return "Lorem ipsum"; } }

        [EditableTags]
        public virtual IEnumerable<string> Tags
        {
            get { return GetDetailCollection("Tags", true).OfType<string>(); }
            set { GetDetailCollection("Tags", true).Replace(value); }
        }

        [Persistable]
        public virtual string PersistableProperty { get; set; }

        [EditableDummy] 
        public virtual List<string> StringList
        {
            get { return (List<string>)GetDetail("StringList"); }
            set { SetDetail("StringList", value); }
        }

        [EditableLink]
        public virtual ContentItem EditableLink { get; set; }
    }

    public class EditableDummyAttribute : AbstractEditableAttribute
    {
        public override bool UpdateItem(ContentItem item, System.Web.UI.Control editor)
        {
            return true;
        }

        public override void UpdateEditor(ContentItem item, System.Web.UI.Control editor)
        {
        }

        protected override System.Web.UI.Control AddEditor(System.Web.UI.Control container)
        {
            return null;
        }
    }


    [PageDefinition("NonIndexable persistable Item")]
    [Indexable(IsIndexable = false)]
    public class PersistableItem1b : PersistableItem
    {
        [EditableNumber(DefaultValue = 666)]
        public virtual int CompilerGeneratedIntProperty { get; set; }
    }
}
