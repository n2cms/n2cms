using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Renders Request.RawUrl instead of the standard action attribute. This 
    /// is useful to maintain the firnedly url upon postback.
    /// </summary>
    public class RawUrlForm : HtmlForm
    {
        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            string attributes = GetAttributesString();

            int startIndex = attributes.IndexOf("action=\"") + 8;
            int endIndex = attributes.IndexOf('"', startIndex);

            writer.Write(attributes.Substring(0, startIndex));
            writer.Write(Page.Request.RawUrl);
            writer.Write(attributes.Substring(endIndex));
        }

        string GetAttributesString()
        {
            StringBuilder sb = new StringBuilder();
            using (TextWriter tw  = new StringWriter(sb))
            using (HtmlTextWriter htw = new HtmlTextWriter(tw))
            {
                base.RenderAttributes(htw);
            }
            return sb.ToString();
        }
    }
}
