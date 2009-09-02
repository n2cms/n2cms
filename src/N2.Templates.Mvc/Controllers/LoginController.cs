using System.Web.Mvc;
using System.Web.Security;
using MvcContrib.Filters;
using N2.Templates.Mvc.Items.Items;
using N2.Templates.Mvc.Models;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(LoginItem))]
	public class LoginController : N2Controller<LoginItem>
	{
		[ModelStateToTempData]
		public override ActionResult Index()
		{
			var model = new LoginModel(CurrentItem);

			model.LoggedIn = User.Identity.IsAuthenticated;

			return PartialView(model);
		}

		[ModelStateToTempData]
		public ActionResult Login(string userName, string password, bool? remember)
		{
			if(Membership.ValidateUser(userName, password) || FormsAuthentication.Authenticate(userName, password))
			{
				FormsAuthentication.SetAuthCookie(userName, remember ?? false);
			}
			else
			{
				ModelState.AddModelError("Login.Failed", CurrentItem.FailureText);
			}
			return Redirect(CurrentItem.Parent.Url);
		}

		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();

			return Redirect(CurrentItem.Parent.Url);
		}
	}
}