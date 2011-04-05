using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2.Edit.Workflow;

namespace N2.Details
{
	public class DefaultValueAttribute : Attribute, IContentTransformer, IUniquelyNamed
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

		public bool Transform(ContentItem item)
		{
			if (Name == null)
				return false;

			item[Name] = Value;
			return true;
		}

		#endregion
	}
}
