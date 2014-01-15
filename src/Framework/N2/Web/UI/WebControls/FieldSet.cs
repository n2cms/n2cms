using System.Web.UI;

namespace N2.Web.UI.WebControls
{
    /// <summary>A fieldset container control with legend.</summary>
    public class FieldSet : System.Web.UI.HtmlControls.HtmlContainerControl
    {
        public override string TagName
        {
            get { return "fieldset"; }
        }

        /// <summary>Gets or sets the fieldset legend (it's text/title).</summary>
        public string Legend
        {
            get { return (string)(ViewState["Legend"] ?? ""); }
            set { ViewState["Legend"] = value; }
        }

        /// <summary>Gets or sets this controls class name.</summary>
        public string CssClass
        {
            get { return (string)(ViewState["CssClass"] ?? ""); }
            set { ViewState["CssClass"] = value; }
        }

        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            base.RenderAttributes(writer);
            if (!string.IsNullOrEmpty(ID))
                writer.WriteAttribute("id", this.ClientID);
            if (this.CssClass.Length > 0)
                writer.WriteAttribute("class", this.CssClass + (this.Disabled ? " disabled" : string.Empty));
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            base.RenderBeginTag(writer);
            if (this.Legend.Length > 0)
            {
                writer.Write("<legend>");
                writer.Write(this.Legend);
                writer.Write("</legend>");
            }
        }
    }
}
