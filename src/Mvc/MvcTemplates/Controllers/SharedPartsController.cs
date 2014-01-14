using N2.Templates.Mvc.Models.Parts;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
    /// <summary>
    /// This controller will handle parts deriving from AbstractItem which are not 
    /// defined by another controller [Controls(typeof(MyPart))]. The default 
    /// behavior is to render a template with this pattern:
    ///  * "~/Views/SharedParts/{ContentTypeName}.ascx"
    /// </summary>
    [Controls(typeof(PartBase))]
    public class SharedPartsController : TemplatesControllerBase<PartBase>
    {
    }
}
