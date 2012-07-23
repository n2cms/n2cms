using System;
using System.Collections.Generic;
using N2.Persistence;
using N2.Persistence.Search;
using N2.Details;
using N2.Definitions;

namespace N2.Tests.Persistence.Definitions
{
	[PartDefinition("Default persistable part", Name = "PersistablePart")]
	public class PersistablePart1 : ContentItem, IPart
	{
	}

	[PageDefinition("Default persistable Item", Name = "PersistableItem")]
	public class PersistableItem1 : N2.ContentItem, IPage
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

		public virtual DateTime DateTimeProperty
		{
			get { return (DateTime)(GetDetail("DateTimeProperty") ?? DateTime.MinValue); }
			set { SetDetail<DateTime>("DateTimeProperty", value); }
		}
		public virtual double DoubleProperty
		{
			get { return (double)(GetDetail("DoubleProperty") ?? 0); }
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
	}


	[PageDefinition("NonIndexable persistable Item")]
	[Indexable(IsIndexable = false)]
	public class PersistableItem1b : PersistableItem1
	{
	}
}
