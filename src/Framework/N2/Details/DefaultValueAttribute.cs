using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Edit.Workflow;

namespace N2.Details
{
	public class DefaultValueAttribute : Attribute, IContentModifier, IUniquelyNamed
	{
		public DefaultValueAttribute()
		{
		}

		public DefaultValueAttribute(object value)
		{
			Value = value;
		}

		public string Name { get; set; }

		public object Value { get; set; }

		#region IContentModifier Members

		public ContentState ChangingTo
		{
			get { return ContentState.New; }
		}

		public void Modify(ContentItem item)
		{
			if (Name == null)
				return;
			item[Name] = Value;
		}

		#endregion
	}
}
