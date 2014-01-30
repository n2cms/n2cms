using System.Web.Mvc;
using N2.Templates.Mvc.Models.Parts;
using N2.Web;
using System.Threading;

namespace N2.Templates.Mvc.Controllers
{
    //[Controls(typeof(Poll))]
    public class PollController : TemplatesControllerBase<Poll>
    {
        public override ActionResult Index()
        {
            return PartialView(CurrentItem);
        }

        public ActionResult Submit(int? selectedItem)
        {
            if (selectedItem == null)
                ModelState.AddModelError("Poll.Errors", Resources.Poll.MakeSelection);
            else
            {
                var cookie = Request.Cookies[CurrentItem.GetAnsweredCookie(selectedItem.Value).Name];

                if (cookie != null && cookie.Value != null)
                    ModelState.AddModelError("Poll.Errors", Resources.Poll.AlreadyVoted);
            }

            if (ModelState.IsValid)
            {
                CurrentItem.AddAnswer(Engine.Persister, selectedItem.Value);
                Response.Cookies.Add(CurrentItem.GetAnsweredCookie(selectedItem.Value));
            }
            return ViewParentPage();
        }
    }
}
