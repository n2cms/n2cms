using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Tests.Content
{
	public class AnItem : N2.ContentItem
	{
		public virtual int IntProperty
		{
			get { return (int)(GetDetail("IntProperty") ?? 0); }
			set { SetDetail("IntProperty", value, 0); }
		}
		public virtual string StringProperty
		{
			get { return (string)(GetDetail("StringProperty") ?? string.Empty); }
			set { SetDetail("StringProperty", value, string.Empty); }
		}
	}
	public class AnItemWithProtectedDefaultConstructor : N2.ContentItem
	{
		protected AnItemWithProtectedDefaultConstructor()
		{
		}

		public virtual int IntProperty
		{
			get { return (int)(GetDetail("IntProperty") ?? 0); }
			set { SetDetail("IntProperty", value, 0); }
		}

		public virtual string StringProperty
		{
			get { return (string)(GetDetail("StringProperty") ?? string.Empty); }
			set { SetDetail("StringProperty", value, string.Empty); }
		}
	}
}
