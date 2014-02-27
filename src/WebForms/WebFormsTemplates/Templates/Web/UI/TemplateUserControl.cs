namespace N2.Templates.Web.UI
{
    // Please note TemplateUserControl2.cs with the dynamic user control base class.

    /// <summary>
    /// A user control that encapsulates the page templete's view logic.
    /// </summary>
    /// <typeparam name="TPage">The type of page the user control has been added to.</typeparam>
    /// <remarks>This user control is not dynamically added to the page. It's just a way to factor view logic. See TemplateUserControl&lt;TPage, TItem&gt; for dynamic parts.</remarks>
    public class TemplateUserControl<TPage> : N2.Web.UI.ContentUserControl<TPage>
         where TPage : ContentItem
    {
    }
}
