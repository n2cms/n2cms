using N2.Templates.Mvc.Models.Pages;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
    /// <summary>
    /// This controller will handle pages deriving from AbstractPage which are not 
    /// defined by another controller [Controls(typeof(MyPage))]. The default 
    /// behavior is to render a template with this pattern:
    ///  * "~/Views/SharedPages/{ContentTypeName}.aspx"
    /// </summary>
    [Controls(typeof(PageBase))]
    public class SharedPagesController : TemplatesControllerBase<PageBase>
    {
    }
}
