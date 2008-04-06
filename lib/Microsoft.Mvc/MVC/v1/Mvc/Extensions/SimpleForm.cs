namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using System.Web.Routing;

    /// <summary>
    /// A class representing an HTML form. Access this object using Html.Form()
    /// </summary>
    public class SimpleForm : IDisposable {
        string formFormat = "<form action=\"{0}\" method=\"{1}\" {2}>";

        private RouteValueDictionary _htmlAttributes;
        private HttpContextBase _context;
        private FormMethod _formMethod = FormMethod.Post;
        private string _formAction;

        public SimpleForm(HttpContextBase context, string formAction, FormMethod method, RouteValueDictionary htmlAttributes) {
            _htmlAttributes = htmlAttributes;
            _formMethod = method;
            _context = context;
            _formAction = formAction;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "We want the value to be lower case since it is used in HTML.")]
        private string BuildFormOpenTag() {
            string attributeList = string.Empty;
            string result = string.Empty;
            string method = Enum.GetName(typeof(FormMethod), _formMethod).ToLowerInvariant();
            if (_htmlAttributes != null) {
                // TODO: Verify
                attributeList = HtmlExtensionUtility.ConvertObjectToAttributeList(_htmlAttributes);
            }

            result = string.Format(CultureInfo.InvariantCulture, formFormat, _formAction, method, attributeList);

            return result;
        }

        public void WriteStartTag() {
            string formTag = BuildFormOpenTag();
            _context.Response.Write(formTag);
        }

        public void WriteEndTag() {
            _context.Response.Write("</form>");
        }

        protected virtual void Dispose(bool shouldDispose) {
            if (shouldDispose) {
                WriteEndTag();
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
