using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;
using N2.Web;
using System;
using System.Diagnostics;

namespace N2.Templates.Web.Adapters
{
    /// <summary>
    /// Maintains friendly url between postbacks.
    /// </summary>
    [Obsolete("Use tagMappings and RawUrlForm instead.")]
    public class FormAdapter : ControlAdapter
    {
        protected override void Render(HtmlTextWriter writer)
        {
            HtmlForm form = Control as HtmlForm;

            writer.WriteBeginTag("form");
            WriteAttributes(writer, form);
            writer.Write(HtmlTextWriter.TagRightChar);

            RenderChildren(writer);

            writer.WriteEndTag("form");
        }

        private void WriteAttributes(HtmlTextWriter writer, HtmlForm form)
        {
            if (form.ID != null)
                writer.WriteAttribute("id", form.ClientID);
            writer.WriteAttribute("name", form.Name);
            writer.WriteAttribute("method", form.Method);

            foreach (string key in form.Attributes.Keys)
                writer.WriteAttribute(key, form.Attributes[key]);

            string url = Page.Request.QueryString["postback"];
            if (string.IsNullOrEmpty(url))
                writer.WriteAttribute("action", Page.Request.RawUrl);
            else
                writer.WriteAttribute("action", url);
        }
    }
}
