using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using N2.Templates.Mvc.Items.Items;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mail;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof (UserRegistration))]
	public class UserRegistrationController : N2Controller<UserRegistration>
	{
		private readonly IErrorHandler _errorHandler;
		private readonly IMailSender _mailSender;

		public UserRegistrationController(IMailSender mailSender, IErrorHandler errorHandler)
		{
			_mailSender = mailSender;
			_errorHandler = errorHandler;
		}

		[NonAction]
		public override ActionResult Index()
		{
			return Index(null, new UserRegistrationModel());
		}

		public ActionResult Index(string verify, UserRegistrationModel model)
		{
			if (verify != null)
			{
				return VerifyUser(verify);
			}
			model.CurrentItem = CurrentItem;

			return View(model);
		}

		private ActionResult VerifyUser(string encryptedTicket)
		{
			FormsAuthenticationTicket t = FormsAuthentication.Decrypt(encryptedTicket);
			MembershipUser user = Membership.GetUser(t.Name);
			user.IsApproved = true;
			Membership.UpdateUser(user);

			if (CurrentItem.VerifiedPage != null)
				return Redirect(CurrentItem.VerifiedPage.Url);

			return View(new UserRegistrationModel(CurrentItem));
		}

		[ValidateAntiForgeryToken(Salt = "register")]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Submit(UserRegistrationModel model)
		{
			if (IsEditorOrAdmin(model.UserName) || Membership.GetUser(model.UserName) != null)
			{
				ModelState.AddModelError("UserName", "Invalid user name.");

				return ViewParentPage();
			}

			if (ModelState.IsValid == false)
				return ViewParentPage();

			return CreateUser(model);
		}

		private ActionResult CreateUser(UserRegistrationModel model)
		{
			string un = model.UserName;
			string pw = model.Password;

			Membership.CreateUser(un, pw);

			Roles.AddUserToRoles(un, CurrentItem.Roles.ToArray<string>());
			MembershipUser user = Membership.GetUser(un);

			if (CurrentItem.RequireVerification)
			{
				user.IsApproved = false;
				Membership.UpdateUser(user);

				string crypto = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(un, true, 60*24*7));
				var url = new Url(Request.Url.Scheme, Request.Url.Authority, CurrentItem.Url).AppendQuery("verify", crypto);

				string subject = CurrentItem.VerificationSubject;
				string body = CurrentItem.VerificationText.Replace("{VerificationUrl}", url);

				try
				{
					_mailSender.Send(CurrentItem.VerificationSender, model.Email, subject, body);

					if (CurrentItem.SuccessPage != null)
					{
						return Redirect(CurrentItem.SuccessPage.Url);
					}
				}
				catch (InvalidOperationException ex)
				{
					ModelState.AddModelError("general", ex.Message);
					_errorHandler.Notify(ex);

					return ViewParentPage();
				}
			}
			else if (CurrentItem.SuccessPage != null)
			{
				return Redirect(CurrentItem.SuccessPage.Url);
			}

			return ViewParentPage();
		}

		private bool IsEditorOrAdmin(string username)
		{
			var user = new GenericPrincipal(new GenericIdentity(username), new string[0]);

			return Engine.SecurityManager.IsEditor(user) || Engine.SecurityManager.IsAdmin(user);
		}
	}
}