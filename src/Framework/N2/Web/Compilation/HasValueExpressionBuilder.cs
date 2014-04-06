using System.Web.Compilation;

namespace N2.Web.Compilation
{
    /// <summary>
    /// Gives a true/false value indicating wether a certain expressions would 
    /// give a value. Useful in situations where we need to hide a webcontrol 
    /// when there is no value to give it.
    /// </summary>
    /// <example>
    /// &lt;asp:Image ImageUrl="&lt;%$ CurrentItem: MainImageUrl %&gt;" Visible="&lt;%$ HasValue: MainImageUrl %&gt;" runat="server" /&gt;
    /// </example>
    [ExpressionPrefix("HasValue")]
    public class HasValueExpressionBuilder : N2ExpressionBuilder
    {
        /// <summary>The item associated with the currently created control.</summary>
        private static ContentItem CurrentItemInContext
        {
            get { return UI.ItemUtility.CurrentContentItem; }
        }

        /// <summary>Gets wether a certain expression has a value.</summary>
        /// <param name="expression">The expression to check.</param>
        /// <returns>True if the expression would result in a non null or non empty-string value.</returns>
        public static bool HasValue(string expression)
        {
            ContentItem item = CurrentItemInContext;
            if (item != null)
                return HasValue(item, expression);
            else
                return HasValue(Context.CurrentPage, expression);
        }

        private static bool HasValue(ContentItem item, string propertyName)
        {
            return item[propertyName] != null && string.Empty != item[propertyName] as string;
        }

        /// <summary>Gets the expression format for this expression.</summary>
        protected override string ExpressionFormat
        {
            get { return @"N2.Web.Compilation.HasValueExpressionBuilder.HasValue(""{0}"")"; }
        }
    }
}
