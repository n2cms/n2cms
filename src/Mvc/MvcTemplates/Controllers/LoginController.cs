using System.Web.Mvc;
using System.Web.Security;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof(LoginItem))]
    public class LoginController : TemplatesControllerBase<LoginItem>
    {
        public override ActionResult Index()
        {
            var model = new LoginModel(CurrentItem) { LoggedIn = User.Identity.IsAuthenticated };
            return View(model);
        }

        public ActionResult Login(string userName, string password, bool? remember)
        {
            if(Membership.ValidateUser(userName, password) || FormsAuthentication.Authenticate(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, remember ?? false);
                if (string.IsNullOrEmpty(Request["returnUrl"]))
                {
                    return RedirectToParentPage();
                }
                else
                {
                    return Redirect(Request["returnUrl"]);
                }
            }
            else
            {
                ModelState.AddModelError("Login.Failed", CurrentItem.FailureText);
            }
            return ViewParentPage();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToParentPage();
        }
    }
}
