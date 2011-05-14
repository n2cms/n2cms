using System;

namespace N2.Tests.Persistence.Definitions
{
	[PageDefinition("Default persistable Item", Name = "PersistableItem")]
	public class PersistableItem1 : N2.ContentItem
	{
		public virtual bool BoolProperty
		{
			get { return (bool)(GetDetail("BoolProperty") ?? true); }
			set { SetDetail<bool>("BoolProperty", value); }
		}

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
	}
}
