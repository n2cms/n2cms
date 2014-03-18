using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mail;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof (UserRegistration))]
    public class UserRegistrationController : TemplatesControllerBase<UserRegistration>
    {
        private readonly IErrorNotifier errorHandler;
        private readonly IMailSender mailSender;

        public UserRegistrationController(IMailSender mailSender, IErrorNotifier errorHandler)
        {
            this.mailSender = mailSender;
            this.errorHandler = errorHandler;
        }

        [HttpGet]
        public override ActionResult Index()
        {
            return PartialView(new UserRegistrationModel());
        }

        [HttpPost]
        public ActionResult Index(UserRegistrationModel model)
        {
            model.CurrentItem = CurrentItem;

            return PartialView(model);
        }

        public ActionResult Verify(string ticket)
        {
            FormsAuthenticationTicket t = FormsAuthentication.Decrypt(ticket);
            MembershipUser user = Membership.GetUser(t.Name);
            user.IsApproved = true;
            Membership.UpdateUser(user);

            if (CurrentItem.VerifiedPage != null)
                return Redirect(CurrentItem.VerifiedPage.Url);

            return View(new UserRegistrationModel(CurrentItem));
        }

        [ValidateAntiForgeryToken]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Submit(UserRegistrationModel model)
        {
            if (ModelState.IsValid == false)
                return ViewParentPage();

            if (IsEditorOrAdmin(model.RegisterUserName) || Membership.GetUser(model.RegisterUserName) != null)
            {
                ModelState.AddModelError("UserName", Resources.UserRegistration.InvalidUserName);

                return ViewParentPage();
            }

            return CreateUser(model);
        }

        private ActionResult CreateUser(UserRegistrationModel model)
        {
            string un = model.RegisterUserName;
            string pw = model.RegisterPassword;

            Membership.CreateUser(un, pw, model.RegisterEmail);

            Roles.AddUserToRoles(un, CurrentItem.Roles.ToArray<string>());
            MembershipUser user = Membership.GetUser(un);

            if (CurrentItem.RequireVerification)
            {
                user.IsApproved = false;
                Membership.UpdateUser(user);

                string crypto = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(un, true, 60*24*7));
                var route = new UrlHelper(ControllerContext.RequestContext).Action("verify", new { n2item = CurrentItem, ticket = crypto });
                var url = new Url(Request.Url.Scheme, Request.Url.Authority, route);

                string subject = CurrentItem.VerificationSubject;
                string body = CurrentItem.VerificationText.Replace("{VerificationUrl}", url);

                try
                {
                    mailSender.Send(CurrentItem.VerificationSender, model.RegisterEmail, subject, body);

                    if (CurrentItem.SuccessPage != null)
                    {
                        return Redirect(CurrentItem.SuccessPage.Url);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("general", ex.Message);
                    errorHandler.Notify(ex);

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
