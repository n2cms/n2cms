using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit.Web.UI.Controls
{
    public class FieldSet : System.Web.UI.HtmlControls.HtmlContainerControl
    {
        public override string TagName
        {
            get
            {
                return "fieldset";
            }
        }

        public string Legend
        {
            get { return (string)(ViewState["Legend"] ?? ""); }
            set { ViewState["Legend"] = value; }
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
