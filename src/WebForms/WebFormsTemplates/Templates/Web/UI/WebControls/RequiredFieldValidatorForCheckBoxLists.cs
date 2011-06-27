using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text;

namespace N2.Templates.Web.UI.WebControls
{
    public class RequiredFieldValidatorForCheckBoxLists : BaseValidator
    {
        private const string SCRIPTBLOCK = "RFV4CL";



        protected override bool ControlPropertiesValid()
        {
            Control ctrl = FindControl(ControlToValidate);
            if (ctrl != null)
            {
                CheckBoxList _listctrl = (CheckBoxList)ctrl;
                return (_listctrl != null);
            }
            else
                return false;
        }

        protected override bool EvaluateIsValid()
        {
            return EvaluateIsChecked();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (EnableClientScript) { this.ClientScript(); }

        }

        private void ClientScript()
        {
            StringBuilder sb_Script = new StringBuilder();
            sb_Script.Append("<script language=\"javascript\">");
            sb_Script.Append("\r");
            sb_Script.Append("\r");
            sb_Script.Append("function cb_verify(sender) {");
            sb_Script.Append("\r");
            sb_Script.Append("var val = document.getElementById(document.getElementById(sender.id).controltovalidate);");
            sb_Script.Append("\r");
            sb_Script.Append("var col = val.getElementsByTagName(\"*\");");
            sb_Script.Append("\r");
            sb_Script.Append("if ( col != null ) {");
            sb_Script.Append("\r");
            sb_Script.Append("for ( i = 0; i < col.length; i++ ) {");
            sb_Script.Append("\r");
            sb_Script.Append("if (col.item(i).tagName == \"INPUT\") {");
            sb_Script.Append("\r");
            sb_Script.Append("if ( col.item(i).checked ) {");
            sb_Script.Append("\r");
            sb_Script.Append("\r");
            sb_Script.Append("return true;");
            sb_Script.Append("\r");
            sb_Script.Append("}");
            sb_Script.Append("\r");
            sb_Script.Append("}");
            sb_Script.Append("\r");
            sb_Script.Append("}");
            sb_Script.Append("\r");
            sb_Script.Append("\r");
            sb_Script.Append("\r");
            sb_Script.Append("return false;");
            sb_Script.Append("\r");
            sb_Script.Append("}");
            sb_Script.Append("\r");
            sb_Script.Append("}");
            sb_Script.Append("\r");
            sb_Script.Append("</script>");
            Page.ClientScript.RegisterClientScriptBlock(GetType(), SCRIPTBLOCK, sb_Script.ToString());
            Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "cb_verify");
        }


        private bool EvaluateIsChecked()
        {
            CheckBoxList _cbl = ((CheckBoxList)FindControl(ControlToValidate));
            foreach (ListItem li in _cbl.Items)
            {
                if (li.Selected)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
