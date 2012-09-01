using System;
using N2.Definitions;

namespace N2.Details
{
	[AttributeUsage(AttributeTargets.Property)]
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
