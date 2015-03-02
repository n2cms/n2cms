using System.IO;
using System.Web;
using System.Web.UI;
using System;
using N2.Edit;
using N2.Web;

namespace N2.Details
{
    /// <summary>
    /// Specifies a text box which renders as a html meta tag element when rendered on a web page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableMetaTagAttribute : EditableTextAttribute
    {
        public override System.Web.UI.Control AddTo(ContentItem item, string detailName, System.Web.UI.Control container)
        {
            using (var tw = new StringWriter())
            {
                Write(item, detailName, tw);
                var lc = new LiteralControl(tw.ToString());
                return lc;
            }
        }

        public override void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            string content = item[propertyName] as string;
            if (string.IsNullOrEmpty(content))
                return;

            writer.Write("<meta name=\"");
            writer.Write(propertyName.ToLower());
            writer.Write("\" content=\"");
            writer.Write(HtmlSanitizer.Current.Encode(content));
            writer.Write("\" />");          
        }
    }
}
