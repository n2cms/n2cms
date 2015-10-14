using System.Web.UI;
using System.Web.UI.WebControls;
using System;

namespace N2.Details
{
    /// <summary>An editable checkbox attribute. Besides creating a checkbox it also uses the checkbox's text property to display text.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableCheckBoxAttribute : EditableAttribute, IWritingDisplayable, IDisplayable
    {
        private string checkBoxText;

        public EditableCheckBoxAttribute()
            : base("", typeof(CheckBox), "Checked", 0)
        {
			ClientAdapter = "n2autosave.checkbox";
		}

        /// <summary>Creates a new instance of the checkbox editable attribute.</summary>
        /// <param name="checkBoxText">The text on the checkbox.</param>
        /// <param name="sortOrder">The order of this editable checkbox.</param>
        public EditableCheckBoxAttribute(string checkBoxText, int sortOrder)
            : base(string.Empty, typeof (CheckBox), "Checked", sortOrder)
        {
            CheckBoxText = checkBoxText;
			ClientAdapter = "n2autosave.checkbox";
        }

        /// <summary>Gets or sets the text on the checkbox. This differs from the title property since the text is after the checkbox.</summary>
        public string CheckBoxText
        {
            get { return checkBoxText ?? Name; }
            set { checkBoxText = value; }
        }

        /// <summary>Creates a checkbox.</summary>
        /// <param name="container">The container the checkbox will be added to.</param>
        /// <returns>A checkbox.</returns>
        protected override Control CreateEditor(Control container)
        {
            CheckBox cb = new CheckBox();
            cb.Text = GetLocalizedText("CheckBoxText") ?? CheckBoxText;
            return cb;
        }

        protected override Control AddEditor(Control container)
        {
            var editor = base.AddEditor(container);
            if (string.IsNullOrEmpty(Title))
                AddHelp(container);
            return editor;
        }

        protected override Label AddLabel(Control container)
        {
            if (string.IsNullOrEmpty(Title))
                return null;
            
            return base.AddLabel(container);
        }

        #region IWritingDisplayable Members

        public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            var value = item[propertyName];
            writer.Write("<input type=\"checkbox\" disabled=\"disabled\"" + ((value != null && (bool)value) ? "checked=\"checked\"" : "") + "/>");          
        }

        #endregion
    }
}
