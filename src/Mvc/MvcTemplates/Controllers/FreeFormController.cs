using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using N2.Templates.Mvc.Details;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mail;
using N2.Details;
using N2.Web.Rendering;
using System.IO;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof (FreeForm))]
    public class FreeFormController : TemplatesControllerBase<FreeForm>
    {
        private readonly IMailSender mailSender;

        public FreeFormController(IMailSender mailSender)
        {
            this.mailSender = mailSender;
        }

        public override ActionResult Index()
        {
            bool formSubmit;
            if (Boolean.TryParse(Convert.ToString(TempData["FormSubmit"]), out formSubmit) && formSubmit)
                return PartialView("Submitted");

            if (CurrentItem.GetTokens("Form").Any(dt => dt.Is("FormSubmit")))
                return PartialView("Form");
            else
                return PartialView();
        }

        public ActionResult Submit(FormCollection collection)
        {
            var mm = new MailMessage(CurrentItem.MailFrom, CurrentItem.MailTo.Replace(";", ","));
            mm.Subject = CurrentItem.MailSubject;
            mm.Headers["X-FreeForm-Submitter-IP"] = Request.UserHostName;
            mm.Headers["X-FreeForm-Submitter-Date"] = N2.Utility.CurrentTime().ToString();
            using (var sw = new StringWriter())
            {
                sw.WriteLine(CurrentItem.MailBody);
                foreach (var token in CurrentItem.GetTokens("Form").Where(dt => dt.Name.StartsWith("Form", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var components = token.GetComponents();
                    string name = components[0];
                    name = token.Value ?? token.GenerateInputName();

                    if (token.Is("FormSubmit"))
                    {
                        continue;
                    }
                    else if (token.Is("FormFile"))
                    {
                        name = token.Value ?? token.GenerateInputName();

                        if (Request.Files[name] == null)
                            continue;
                        
                        var postedFile = Request.Files[name];
                        if (postedFile.ContentLength == 0)
                            continue;

                        var fileName = Path.GetFileName(postedFile.FileName);
                        sw.WriteLine(name + ": " + fileName + " (" + (int)(postedFile.ContentLength / 1024) + "kB)");
                        mm.Attachments.Add(new Attachment(postedFile.InputStream, fileName, postedFile.ContentType));
                    }
                    else if (token.Is("FormInput") || token.Is("FormTextarea"))
                    {
                        name = token.Value ?? token.GenerateInputName();
                        var value = collection[name];
                        sw.WriteLine(name + ": " + value);
                    }
                    else
                    {
                        name = token.GetOptionalInputName(0, 1);
                        var value = collection[name];
                        sw.WriteLine(name + ": " + value);
                    }
                }

                mm.Body = sw.ToString();
            }

            mailSender.Send(mm);

            TempData.Add("FormSubmit", true);

            return RedirectToParentPage();
        }
    }
}
