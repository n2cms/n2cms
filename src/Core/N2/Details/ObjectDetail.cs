using System;
using System.Diagnostics;

namespace N2.Details
{
	/// <summary>A content detail. A number of content details can be associated with one content item.</summary>
	[Serializable]
	[DebuggerDisplay("{Name}: Value: {Value}")]
	public class ObjectDetail : ContentDetail
	{
		#region Constuctors

		/// <summary>Creates a new instance of a specific detail type.</summary>
		public ObjectDetail() : base()
		{
		}

		/// <summary>Creates a new instance of a specific detail type.</summary>
		/// <param name="enclosingItem">The item that encloses this detail.</param>
		/// <param name="name">The name of the detail.</param>
		/// <param name="value">The detail value.</param>
		public ObjectDetail(ContentItem enclosingItem, string name, object value)
		{
			ID = 0;
			EnclosingItem = enclosingItem;
			Name = name;
			Value = value;
		}

		#endregion

		#region Private Fields

		private object value;

		#endregion

		#region Properties

		/// <summary>Gets or sets this details value.</summary>
		public override object Value
		{
			get { return value; }
			set { this.value = value; }
		}

		/// <summary>Gets the type of this detail.</summary>
		public override Type ValueType
		{
			get { return typeof (object); }
		}

		#endregion
	}
}