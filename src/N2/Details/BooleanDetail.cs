using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>
	/// A boolean content detail. A number of content details can be associated 
	/// with one content item.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{Name}: BoolValue: {BoolValue}")]
	public class BooleanDetail : ContentDetail
	{
		#region Constuctors

		public BooleanDetail()
		{
		}

		public BooleanDetail(ContentItem containerItem, string name, bool value)
		{
			ID = 0;
			EnclosingItem = containerItem;
			Name = name;
			boolValue = value;
		}

		#endregion

		#region Properties

		private bool boolValue;

		public virtual bool BoolValue
		{
			get { return boolValue; }
			set { boolValue = value; }
		}

		public override object Value
		{
			get { return boolValue; }
			set { boolValue = (bool) value; }
		}

		public override Type ValueType
		{
			get { return typeof (bool); }
		}

		#endregion
	}
}