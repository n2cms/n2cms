using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>
	/// A string content detail. A number of content details can be associated 
	/// with one content item.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{Name}: StringValue: {StringValue}")]
	public class StringDetail : ContentDetail
	{
		#region Constuctors

		public StringDetail() : base()
		{
		}

		public StringDetail(ContentItem containerItem, string name, string value)
		{
			ID = 0;
			EnclosingItem = containerItem;
			Name = name;
			stringValue = value;
		}

		#endregion

		#region Properties

		private string stringValue;

		public virtual string StringValue
		{
			get { return stringValue; }
			set { stringValue = value; }
		}

		public override object Value
		{
			get { return stringValue; }
			set { stringValue = (string) value; }
		}

		public override Type ValueType
		{
			get { return typeof (string); }
		}

		#endregion
	}
}