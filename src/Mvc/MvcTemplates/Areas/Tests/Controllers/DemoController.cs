#if DEMO
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using N2.Templates.Mvc.Areas.Tests.Models;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{

    //<appSettings>
    //  <add key="Demo.From" value="from@gmail.com"/>
    //  <add key="Demo.To" value="to@gmail.com"/>
    //  <add key="Demo.EnableContentReset" value="true"/>
    //</appSettings>
    [Controls(typeof(DemoPart))]
    public class DemoController : ContentController<DemoPart>
    {
        //
        // GET: /Tests/Demo/

        private readonly Engine.Logger<DemoController> logger;

        public override ActionResult Index()
        {
            var model = new DemoViewModel();
            if (TempData.ContainsKey("Deferred"))
            {
                TempData.Remove("Deferred");
                model = TempData["DeferredModel"] as DemoViewModel;
                var state = TempData["DeferredState"] as ModelStateDictionary;
                foreach (var kvp in state)
                {
                    ModelState[kvp.Key] = kvp.Value;
                }
            }
            
            if (User.Identity.IsAuthenticated)
                return PartialView("Authenticated", model);
            else
                return PartialView("Unauthenticated", model);
        }

        public ActionResult Login()
        {
            var model = new DemoViewModel();
            if (TryUpdateModel(model))
            {
                SendEmail("Demo Login [" + Request.UserHostAddress + "]", model.Email);
                FormsAuthentication.SetAuthCookie("admin", false);
                return Redirect(CurrentPage.Url);
            }

            TempData["Deferred"] = true;
            TempData["DeferredModel"] = model;
            TempData["DeferredState"] = ModelState;

            return RedirectToParentPage();
        }

        public ActionResult Logout(string vote, string feedback)
        {
            if (feedback == DemoViewModel.FeedbackInstruction)
                feedback = "";

            if (!string.IsNullOrEmpty(vote) || !string.IsNullOrEmpty(feedback))
            {
                SendEmail("Demo Feedback [" + Request.UserHostAddress + "]", "Vote: " + vote + Environment.NewLine + feedback);
            }

            FormsAuthentication.SignOut();
            string returnUrl = Request["returnUrl"];
            string logoutUrl = System.Configuration.ConfigurationManager.AppSettings["N2.logout.url"];
            if (!String.IsNullOrEmpty(logoutUrl)) logoutUrl = CurrentPage.Url;
            if (!String.IsNullOrEmpty(returnUrl)) logoutUrl = returnUrl;
            return Redirect(logoutUrl); 
        }

        private void SendEmail(string subject, string body)
        {
            string from = ConfigurationManager.AppSettings["Demo.From"];
            string to = ConfigurationManager.AppSettings["Demo.To"];
            string serverUn = ConfigurationManager.AppSettings["Demo.SmtpUN"];
            string serverPw = ConfigurationManager.AppSettings["Demo.SmtpPW"];

            try
            {
                MailMessage mm = new MailMessage(from, to);
                mm.Subject = subject;
                mm.Body = string.Format(body, body);
                SmtpClient sc = new SmtpClient();
                if (!string.IsNullOrEmpty(serverUn))
                    sc.Credentials = new System.Net.NetworkCredential(serverUn, serverPw);
                sc.Send(mm);
            }
            catch (Exception ex)
            {
                logger.Error(subject + Environment.NewLine + body + Environment.NewLine + ex);
            }
        }
    }
}
#endif
