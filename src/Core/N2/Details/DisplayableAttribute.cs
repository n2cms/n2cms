using System;
using System.Web.UI;

namespace N2.Details
{
	/// <summary>Associate a property/detail with a control used for presentation.</summary>
    public class DisplayableAttribute : Attribute, IDisplayable
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
        private string name;
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

        /// <summary>Gets or sets the name of the detail (property) on the content item's object.</summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
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

        #region Equals & GetHashCode
        /// <summary>Checks another object for equality.</summary>
        /// <param name="obj">The other object to check.</param>
        /// <returns>True if the items are of the same type and have the same name.</returns>
        public override bool Equals(object obj)
        {
            ControlAssociationAttribute other = obj as ControlAssociationAttribute;
            if (other == null)
                return false;
            return (this.Name == other.Name);
        }

        /// <summary>Gets a hash code based on the attribute's name.</summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        #endregion

		#region IDisplayable Members

		public Control AddTo(ContentItem item, string detailName, Control container)
		{
			Control displayer = (Control)Activator.CreateInstance(ControlType);
			Utility.SetProperty(displayer, ControlPropertyName, item[detailName]);
			container.Controls.Add(displayer);
			return displayer;
		}

		#endregion
	}
}


		//protected override void OnInit(EventArgs e)
		//{
		//    if (Displayable != null)
		//    {
		//        displayer = (Control)Activator.CreateInstance(Displayable.ControlType);
		//        string controlPropertyName = Displayable.ControlPropertyName;
		//        PropertyInfo controlPi = displayer.GetType().GetProperty(controlPropertyName);
		//        Debug.Assert(controlPi != null, "Property not found on control: " + controlPropertyName);
		//        if (controlPi != null)
		//        {
		//            controlPi.SetValue(displayer, CurrentItem[PropertyName], new object[0]);
		//            this.Controls.Add(displayer);
		//        }
		//    }

		//    base.OnInit(e);
		//}

		//protected override void OnLoad(EventArgs e)
		//{
		//    if (Displayer != null)
		//    {
		//        if (Displayable.DataBind)
		//            Displayer.DataBind();
		//        if (Displayable.Focus)
		//            Displayer.Focus();
		//    }
		//    base.OnLoad(e);
		//} 