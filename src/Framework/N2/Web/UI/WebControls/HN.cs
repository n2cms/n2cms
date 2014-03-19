using System;
using System.Web;
using System.Web.UI;
using N2.Edit;

namespace N2.Web.UI.WebControls
{
    public class Hn : Control, ITextControl
    {
		public Hn()
		{
			HtmlEncode = true;
		}

        public int Level
        {
            get { return (int)(ViewState["Level"] ?? DefaultLevel); }
            set 
            {
                if (value < 1 || value > 6) 
                    throw new ArgumentException("The level must be between 1 and 6."); 
                ViewState["Level"] = value; 
            }
        }

        public string Text
        {
            get { return (string)(ViewState["Text"] ?? string.Empty); }
            set { ViewState["Text"] = value; }
        }

		public string CssClass { get; set; }

		public bool HtmlEncode { get; set; }

        protected virtual int DefaultLevel
        {
            get { return 1; }
        }

        protected string TagKey { get { return "h" + this.Level; } }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Text.Length > 0)
            {
                string tag = TagKey;
                writer.WriteFullBeginTag(tag);
                if (HtmlEncode)
                    writer.Write(HtmlSanitizer.Current.Encode(Text));
                else
                    writer.Write(Text);
                if (CssClass != null)
                    writer.WriteAttribute("class", CssClass);
                writer.WriteEndTag(tag);
            }
            else
            {
                writer.Write("<!--empty " + TagKey + "-->");
            }
        }
    }
}
