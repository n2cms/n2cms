using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Makes at least one of the associated input controls a required field.
    /// </summary>
    [ToolboxData("<{0}:RequireEitherFieldValidator runat=\"server\" ErrorMessage=\"RequireEitherFieldValidator\"></{0}:RequireEitherFieldValidator>")]
    public class RequireEitherFieldValidator : BaseValidator
    {
        private const string EvaluationScriptKey = "RequireEitherFieldValidatorEvaluateIsValid";

        [Themeable(false)]
        [DefaultValue("")]
        public string InitialValue
        {
            get
            {
                return (string)this.ViewState["InitialValue"] ?? string.Empty;
            }
            set
            {
                this.ViewState["InitialValue"] = (object)value;
            }
        }

        [TypeConverter(typeof(ValidatedControlConverter))]
        [Themeable(false)]
        [DefaultValue("")]
        [IDReferenceProperty]
        public string OtherControlToValidate
        {
            get
            {
                return (string)this.ViewState["OtherControlToValidate"] ?? string.Empty;
            }
            set
            {
                this.ViewState["OtherControlToValidate"] = (object)value;
            }
        }

        internal static void AddExpandoAttribute(Control control, HtmlTextWriter writer, string controlId, string attributeName, string attributeValue, bool encode)
        {
            if (writer != null)
            {
                writer.AddAttribute(attributeName, attributeValue, encode);
            }
        }

        internal void AddExpandoAttribute(HtmlTextWriter writer, string controlId, string attributeName, string attributeValue)
        {
            this.AddExpandoAttribute(writer, controlId, attributeName, attributeValue, true);
        }

        internal void AddExpandoAttribute(HtmlTextWriter writer, string controlId, string attributeName, string attributeValue, bool encode)
        {
            RequireEitherFieldValidator.AddExpandoAttribute((Control)this, writer, controlId, attributeName, attributeValue, encode);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.DetermineRenderUplevel() && this.EnableClientScript)
            {
                string clientId = this.ClientID;
                string controlRenderId = this.GetControlRenderID(this.OtherControlToValidate);
                this.AddExpandoAttribute(writer, clientId, "evaluationfunction", EvaluationScriptKey, false);
                this.AddExpandoAttribute(writer, clientId, "othercontroltovalidate", controlRenderId);
                this.AddExpandoAttribute(writer, clientId, "controlhookup", controlRenderId);
                this.AddExpandoAttribute(writer, clientId, "initialvalue", this.InitialValue);
            }
            base.AddAttributesToRender(writer);
        }

        protected override bool ControlPropertiesValid()
        {
            if (this.OtherControlToValidate.Length > 0)
            {
                this.CheckControlValidationProperty(this.OtherControlToValidate, "OtherControlToValidate");
                if (this.ControlToValidate.Equals(this.OtherControlToValidate, StringComparison.OrdinalIgnoreCase))
                {
                    throw new HttpException(string.Format("Control '{0}' cannot have the same value '{1}' for both ControlToValidate and OtherControlToValidate.", this.ID, this.OtherControlToValidate));
                }
            }
            return base.ControlPropertiesValid();
        }

        protected void CreateJavaScript()
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), EvaluationScriptKey, "<script type=\"text/javascript\">\r\n    function RequireEitherFieldValidatorEvaluateIsValid(val){\r\n    var initialValue = ValidatorTrim(val.initialvalue);\r\n    return (ValidatorTrim(ValidatorGetValue(val.controltovalidate)) != initialValue || ValidatorTrim(ValidatorGetValue(val.othercontroltovalidate)) != initialValue);\r\n}</script>");
        }

        protected override bool EvaluateIsValid()
        {
            string controlValidationValue = this.GetControlValidationValue(this.ControlToValidate);
            string otherControlValidationValue = this.GetControlValidationValue(this.OtherControlToValidate);
            if (controlValidationValue == null && otherControlValidationValue == null)
                return true;
            else
                return !controlValidationValue.Trim().Equals(this.InitialValue.Trim()) || !otherControlValidationValue.Trim().Equals(this.InitialValue.Trim());
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.DetermineRenderUplevel() && this.EnableClientScript)
            {
                string clientId = this.ClientID;
                string controlRenderId = this.GetControlRenderID(this.OtherControlToValidate);
                Page.ClientScript.RegisterExpandoAttribute(clientId, "evaluationfunction", EvaluationScriptKey, false);
                Page.ClientScript.RegisterExpandoAttribute(clientId, "othercontroltovalidate", controlRenderId);
                Page.ClientScript.RegisterExpandoAttribute(clientId, "controlhookup", controlRenderId);
                Page.ClientScript.RegisterExpandoAttribute(clientId, "initialvalue", this.InitialValue);
                this.CreateJavaScript();
            }
            base.OnPreRender(e);
        }
    }
}
