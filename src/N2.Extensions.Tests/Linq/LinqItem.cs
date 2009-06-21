using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Extensions.Tests.Linq
{
	[PageDefinition]
	public class LinqItem : ContentItem
	{
		[N2.Details.EditableTextBox("StringProperty", 100)]
		public virtual string StringProperty
		{
			get { return (string)(GetDetail("StringProperty") ?? string.Empty); }
			set { SetDetail("StringProperty", value, string.Empty); }
		}
	}
}
