using System;
using System.Collections;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>A DateTime content detail. A number of content details can be associated with one content item.</summary>
	[Serializable]
	[DebuggerDisplay("{Name}: DateTimeValue: {DateTimeValue}")]
	public class DateTimeDetail : ContentDetail
	{
        #region Constuctors
		public DateTimeDetail() : base()
		{
		}

        public DateTimeDetail(ContentItem containerItem, string name, DateTime value) 
		{
            this.ID = 0;
            this.EnclosingItem = containerItem;
            this.Name = name;
            this.dateTimeValue = value;
		}
		#endregion

        #region Properties
        private DateTime dateTimeValue;

        public virtual DateTime DateTimeValue
        {
            get { return dateTimeValue; }
            set { dateTimeValue = value; }
        }

        public override object Value
        {
            get { return this.dateTimeValue; }
            set { this.dateTimeValue = (DateTime)value; }
        }

		public override Type ValueType
		{
			get { return typeof(DateTime); }
		}
        #endregion
    }
}
