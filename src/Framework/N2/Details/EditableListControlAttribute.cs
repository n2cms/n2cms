using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace N2.Details
{
    /// <summary>
    /// An abstract base class that implements editable list functionality.
    /// Override and implement GetListItems to use.
    /// Implement a CreateEditor() method to instantiate a desired editor control.
    /// </summary>
    public abstract class EditableListControlAttribute : AbstractEditableAttribute, IDisplayable, IWritingDisplayable
    {
        public EditableListControlAttribute(): base() { }

        public EditableListControlAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            ListControl ddl = editor as ListControl;
            string current = GetValue(item);
            string newValue = GetValue(ddl);

            if (!newValue.Equals(current))
            {
                item[Name] = ConvertToValue(newValue);
                return true;
            }
            else if (current != null && current.Equals(DefaultValue))
            {
                item[Name] = newValue;
                return true;
            }
            return false;
        }

        /// <summary>Gets the object to store as content from the drop down list editor.</summary>
        /// <param name="ddl">The editor.</param>
        /// <returns>The value to store.</returns>
        protected virtual string GetValue(ListControl ddl)
        {
            return ddl.SelectedValue;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            ListControl ddl = editor as ListControl;
            if (item[Name] != null)
            {
                ddl.SelectedValue = GetValue(item);
            }
        }

        /// <summary>Gets a string value from the drop down list editor from the content item.</summary>
        /// <param name="item">The item containing the value.</param>
        /// <returns>A string to use as selected value.</returns>
        protected virtual string GetValue(ContentItem item)
        {
            return ConvertToString(item[Name]) ?? ConvertToString(DefaultValue);
        }

        protected virtual string ConvertToString(object value)
        {
            if (value == null)
                return null;

            return value.ToString();
        }

        protected virtual object ConvertToValue(string value)
        {
            return value;
        }
        
        protected abstract ListControl CreateEditor();

        protected override Control AddHelp(Control container)
        {
            return base.AddHelp(container);
        }

        protected override Control AddRequiredFieldValidator(Control container, Control editor)
        {
            return null;
        }
         
        protected override Control AddEditor(Control container)
        {
            ListControl ddl = this.CreateEditor();
            ddl.ID = Name;
            if (!Required)
                ddl.Items.Add(new ListItem());

            ddl.Items.AddRange(GetListItems());
            container.Controls.Add(ddl);
            return ddl;
        }

        protected abstract ListItem[] GetListItems();

        #region IDisplayable Members

        Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
        {
            using (var sw = new StringWriter())
            {
                Write(item, detailName, sw);
                var lc = new LiteralControl(sw.ToString());
                container.Controls.Add(lc);
                return lc;
            }
        }

        #endregion

        #region IWritingDisplayable Members

        public virtual void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            var selected = item[propertyName] as string;
            if (selected != null)
                writer.Write(GetListItems().Where(li => li.Value == selected).Select(li => li.Text).FirstOrDefault());
        }

        #endregion
    }
}
