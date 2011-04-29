using Dinamico.Models;
using N2.Details;
using N2.Web;
using N2.Web.Mvc;

namespace Dinamico.Controllers
{
	[Controls(typeof(ListingPage))]
	[WithEditableTemplateSelection(ContainerName = Defaults.Containers.Metadata)]
	public class ListingPagesController : ContentController<ListingPage>
    {
    }
}
