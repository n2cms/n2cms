using System.Web.Compilation;

namespace N2.Web.Compilation
{
    /// <summary>Expression builder used for initializing web controls with the current page's detils or properties with the expression syntax. This is a convenient to bind content data to controls on a template. The expression is evaluated at compile time.</summary>
    /// <example>
    /// &lt;asp:Label text="&lt;%$ CurrentPage: Title %&gt;" runat="server" /&gt;
    /// </example>
    [ExpressionPrefix("CurrentPage")]
    public class CurrentPageExpressionBuilder : N2ExpressionBuilder
    {
        public static object GetCurrentPageValue(string expression)
        {
            ContentItem item = N2.Context.CurrentPage;
            if(item != null)
                return item[expression];
            else
                return null;
        }

        protected override string ExpressionFormat
        {
            get { return @"N2.Web.Compilation.CurrentPageExpressionBuilder.GetCurrentPageValue(""{0}"")"; }
        }
    } 
}
