using System;
using System.Diagnostics;

namespace N2.Details
{
    /// <summary>
    /// An integer content detail. A number of content details can be 
    /// associated with one content item.
    /// </summary>
	[Serializable]
	[DebuggerDisplay( "{Name}: IntValue: {IntValue}")]
	public class IntegerDetail : ContentDetail
	{
        #region Constuctors
		public IntegerDetail() : base()
		{
		}

        public IntegerDetail(ContentItem containerItem, string name, int value) 
		{
            this.ID = 0;
            this.EnclosingItem = containerItem;
            this.Name = name;
            this.intValue = value;
		}
		#endregion

        #region Properties
        private int intValue;

        public virtual int IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }

        public override object Value
        {
            get { return this.intValue; }
            set { this.intValue = (int)value; }
		}

		public override Type ValueType
		{
			get { return typeof(int); }
		}
        #endregion
    }
}
