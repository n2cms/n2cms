namespace N2.Templates.Web.UI
{
    // Please note TemplateUserControl.cs with the static user control base class.

    /// <summary>
    /// A user control that can be dynamically added to a page and injected 
    /// with the current content part data.
    /// </summary>
    /// <typeparam name="TPage">The type of page the user control is dynamically added to.</typeparam>
    /// <typeparam name="TItem">The type of part the user control is a template for.</typeparam>
    /// <remarks>This base class will be associated with the content data of the part which can be accessed through the CurrentItem property.</remarks>
    public class TemplateUserControl<TPage, TItem> : N2.Web.UI.ContentUserControl<TPage, TItem>
        where TPage : ContentItem
        where TItem : ContentItem
    {
        private string cssClass;

        public TemplateUserControl()
        {
            string itemTypeName = GetType().BaseType.Name;
            cssClass = itemTypeName.Substring(0, 1).ToLower() + itemTypeName.Substring(1);
        }

        public virtual string CssClass
        {
            get { return cssClass; }
            set { cssClass = value; }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("<div class='uc {0}'>", CssClass);
            base.Render(writer);
            writer.Write("</div>");
        }
    }
}
