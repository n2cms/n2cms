using System.Web.UI.WebControls;
using N2.Details;

namespace N2.Addons.MyAddon.Editables
{
    /// <summary>
    /// The editable reset extends the text box by making it read only and adding a reset button.
    /// </summary>
    public class EditableResetAttribute : EditableTextBoxAttribute
    {
        public EditableResetAttribute(string title, int sortOrder) : base(title, sortOrder)
        {
        }

        /// <summary>
        /// Since we want to add an extra button after the text box we override the AddEditor method.
        /// </summary>
        /// <param name="container">The panel onto which the text box was added.</param>
        /// <returns>The editor control that was added.</returns>
        protected override System.Web.UI.Control AddEditor(System.Web.UI.Control container)
        {
            TextBox tb = base.AddEditor(container) as TextBox;
            tb.ReadOnly = true;

            Button btnReset = new Button();
            btnReset.Text = "Reset";
            btnReset.Click += delegate
                           {
                               tb.Text = "0";
                           };
            container.Controls.Add(btnReset);

            return tb;
        }
    }
}
