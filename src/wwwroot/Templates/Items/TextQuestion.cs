using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using N2.Definitions;
using N2.Templates.Web.UI.WebControls;

namespace N2.Templates.Items
{
    [Definition("Text question (textbox)", "TextQuestion")]
    public class TextQuestion : Question, IContainable
    {

        [N2.Details.EditableTextBox("Rows", 110)]
        public virtual int Rows
        {
            get { return (int)(GetDetail("Rows") ?? 1); }
            set { SetDetail("Rows", value, 1); }
        }

        [N2.Details.EditableTextBox("Columns", 120)]
        public virtual int? Columns
        {
            get { return (int?)GetDetail("Columns"); }
            set { SetDetail("Columns", value); }
        }

        public Control AddTo(Control container)
        {
            TextControl t = new TextControl(this);
            container.Controls.Add(t);
            return t;
        }
    }
}