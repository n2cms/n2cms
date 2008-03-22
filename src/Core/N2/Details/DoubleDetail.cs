using System;
using System.Collections;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>
    /// An double content detail. A number of content details can be associated 
    /// with one content item.
    /// </summary>
	[Serializable]
	[DebuggerDisplay("{Name}: DoubleValue: {DoubleValue}")]
	public class DoubleDetail : ContentDetail
	{
        #region Constuctors
		public DoubleDetail() : base()
		{
		}

		public DoubleDetail(ContentItem containerItem, string name, double value) 
		{
            this.ID = 0;
            this.EnclosingItem = containerItem;
            this.Name = name;
            this.doubleValue = value;
		}
		#endregion

        #region Properties
		private double doubleValue;

        public virtual double DoubleValue
        {
            get { return doubleValue; }
            set { doubleValue = value; }
        }

        public override object Value
        {
            get { return this.doubleValue; }
            set { this.doubleValue = (double)value; }
        }

		public override Type ValueType
		{
			get { return typeof(double); }
		}
        #endregion
    }
}
