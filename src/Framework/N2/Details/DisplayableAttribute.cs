using System;
using System.Web.UI;

namespace N2.Details
{
	/// <summary>Associate a property/detail with a control used for presentation.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DisplayableAttribute : AbstractDisplayableAttribute
	{
		public DisplayableAttribute(Type controlType, string controlPropertyName)
		{
			this.ControlType = controlType;
			this.ControlPropertyName = controlPropertyName;
		}

        #region Private Members
        private Type controlType;
        private string controlPropertyName;
        private string title;
        private int sortOrder;
        private bool dataBind = false;
        private bool focus = false;
        #endregion

        #region Properties
        /// <summary>Gets or sets whether the control should be databound when it's added to a page.</summary>
        public bool DataBind
        {
            get { return dataBind; }
            set { dataBind = value; }
        }

        /// <summary>Gets or sets whether the control should be focused when it's added to a page.</summary>
        public bool Focus
        {
            get { return focus; }
            set { focus = value; }
        }

        /// <summary>Gets or sets the label used for presentation.</summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>Gets or sets the type of the control that is used in combination with an item's property/detail.</summary>
        public Type ControlType
        {
            get { return controlType; }
            set { controlType = value; }
        }

        /// <summary>Gets or sets the property on the control that is used to get or set content data.</summary>
        public string ControlPropertyName
        {
            get { return controlPropertyName; }
            set { controlPropertyName = value; }
        }

        /// <summary>Gets or sets the order of the associated control</summary>
        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
        #endregion

		#region IDisplayable Members

		public override Control AddTo(ContentItem item, string detailName, Control container)
		{
			Control displayer = (Control)Activator.CreateInstance(ControlType);
			Utility.SetProperty(displayer, ControlPropertyName, item[detailName]);
			container.Controls.Add(displayer);
			return displayer;
		}

		#endregion
	}
}
