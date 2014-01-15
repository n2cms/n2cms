using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace N2.MarkDown
{
    public class WmdEditorAttribute : EditableTextBoxAttribute
    {
        public WmdEditorAttribute(string title, int sortOrder) : base(title, sortOrder) { }
        public WmdEditorAttribute(string title, int sortOrder, int maxLength) : base(title, sortOrder, maxLength) { }

        /// <summary>
        /// Creates a multi-line <see cref="System.Web.UI.WebControls.TextBox">TextBox</see> with a class
        /// called 'markdown-editor'. All Javascript and CSS files are added to the page.
        /// </summary>
        protected override System.Web.UI.Control AddEditor(System.Web.UI.Control container)
        {
            Page page = HttpContext.Current.Handler as System.Web.UI.Page;

            if (page == null)
            {
                Literal literalError = new Literal() { Text = "unable to get current page from HttpContext.Current.Handler!" };
                container.Controls.Add(literalError);
                return literalError;
            }
            else
            {

                page.Header.Controls.Add(new Literal() { Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/Addons/Markdown/Wmd/wmd.css\" />" });
                page.Header.Controls.Add(new Literal() { Text = "<script type=\"text/javascript\" src=\"/Addons/Markdown/Wmd/showdown.js\"></script>" });
                page.Header.Controls.Add(new Literal() { Text = "<script type=\"text/javascript\" src=\"/Addons/Markdown/Wmd/wmd.js\"></script>" });

                container.Controls.Add(new Literal() { Text = "<div id=\"wmd-button-bar\" class=\"wmd-panel\"></div>" });

                // Make the control
                TextBox textbox = new TextBox();
                textbox.CssClass = "wmd-panel wmd-textarea";
                textbox.TextMode = TextBoxMode.MultiLine;
                container.Controls.Add(textbox);

                return textbox;
            }   
        }
    }
}
