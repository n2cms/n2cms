using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using N2.Details;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.MarkDown
{
    /// <summary>
    /// Adds MarkItUp editor support, using the markdown syntax.
    /// </summary>
    /// <example>
    /// Below is example of adding the MarkItDown editor and markdown support to a page.
    /// <code>
    /// <![CDATA[
    /// [Definition("My First Page")]
    /// public class MyPagePage : N2.Templates.Items.AbstractContentPage
    /// {
    ///    [MarkItUpEditor("Text", 30)]
    ///    public override string Text
    ///    {
    ///        get
    ///        {
    ///           return base.Text;
    ///        }
    ///        set
    ///        {
    ///            base.Text = value;
    ///        }
    ///    }
    ///
    ///    [N2.Details.Displayable(typeof(Literal), "Text")]
    ///    public string ParsedText
    ///    {
    ///        get
    ///        {
    ///            return new MarkdownParser().Transform(base.Text);
    ///        }
    ///    }
    ///
    ///    public override string TemplateUrl
    ///    {
    ///        get { return "~/UI/MyPage.aspx"; }
    ///    }
    /// }
    /// ]]>
    /// </code>
    /// 
    /// The following would appear in MyPage.aspx:
    /// <code>
    /// <![CDATA[
    /// <n2:Display PropertyName="ParsedText" runat="server" />
    /// ]]>
    /// </code>
    /// </example>
    public class MarkItUpEditorAttribute : EditableTextBoxAttribute
    {
        public MarkItUpEditorAttribute(string title, int sortOrder) : base(title, sortOrder) { }
        public MarkItUpEditorAttribute(string title, int sortOrder, int maxLength) : base(title, sortOrder, maxLength) { }

        /// <summary>
        /// Creates a multi-line <see cref="System.Web.UI.WebControls.TextBox">TextBox</see> with a class
        /// called 'markdown-editor'. All Javascript and CSS files are added to the page.
        /// </summary>
        protected override System.Web.UI.Control AddEditor(System.Web.UI.Control container)
        {
            string error = "";
            Page page = HttpContext.Current.Handler as System.Web.UI.Page;

            if (page == null)
            {
                error = "unable to get current page from HttpContext.Current.Handler!";
            }
            else
            {
                // Add 2 breaks for the title
                Literal literal = new Literal();
                literal.Text = "<br/><br/>";
                container.Controls.Add(literal);

                // Add the CSS. This could be done with page.Header but we can't guarantee it'll have runat=head
                literal = new Literal();
                literal.Text = "<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/Addons/Markdown/MarkItUp/skins/markitup/style.css\"/>";
                container.Controls.Add(literal);

                literal = new Literal();
                literal.Text = "<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"/Addons/Markdown/MarkItUp/sets/markdown/style.css\"/>";
                container.Controls.Add(literal);

                // Add the JS files
                page.ClientScript.RegisterClientScriptInclude("markitupjs1", "/Addons/Markdown/MarkItUp/jquery.markitup.js");
                page.ClientScript.RegisterClientScriptInclude("markitupjs2", "/Addons/Markdown/MarkItUp/sets/markdown/set.js");

                page.ClientScript.RegisterClientScriptBlock(typeof(MarkItUpEditorAttribute), "markitup", "$(document).ready(function(){ $(\".markdown-editor\").markItUp(mySettings); });", true);
            }

            // Make the control
            TextBox textbox = new TextBox();
            textbox.CssClass = "markdown-editor";
            textbox.Text = error;
            textbox.TextMode = TextBoxMode.MultiLine;
            container.Controls.Add(textbox);

            return textbox;
        }
    }
}
