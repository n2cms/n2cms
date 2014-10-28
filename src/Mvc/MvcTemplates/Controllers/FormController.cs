using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using N2.Collections;
using N2.Templates.Mvc.Details;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;
using N2.Web.Mail;

namespace N2.Templates.Mvc.Controllers
{
    [Controls(typeof (Form))]
    public class FormController : TemplatesControllerBase<Form>
    {
        private readonly IMailSender mailSender;

        public FormController(IMailSender mailSender)
        {
            this.mailSender = mailSender;
        }

        public override ActionResult Index()
        {
            var elements = GetQuestions();

            bool formSubmit;
            if (Boolean.TryParse(Convert.ToString(TempData["FormSubmit"]), out formSubmit) && formSubmit)
                return View(new FormModel(CurrentItem, elements) {FormSubmitted = true});

            return View(new FormModel(CurrentItem, elements));
        }

        private IEnumerable<IQuestion> GetQuestions()
        {
	        var questions = CurrentItem.GetChildPartsUnfiltered(Zones.Questions);
	        return questions.OfType<IQuestion>();
        }

	    public ActionResult Submit(FormCollection collection)
        {
            AnswerContext ac = new AnswerContext();

            ac.Subject = CurrentItem.MailSubject;
            ac.Body.Append(CurrentItem.MailBody);
            ac.HttpContext = HttpContext;
            
            foreach (IQuestion q in GetQuestions())
            {
                q.AppendAnswer(ac, collection[q.ElementID]);
            }

            if(ac.ValidationErrors.Count > 0)
            {
                foreach(var error in ac.ValidationErrors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
                return ViewParentPage();
            }

            var mm = new MailMessage(CurrentItem.MailFrom, CurrentItem.MailTo.Replace(";", ","));
            mm.Subject = ac.Subject;
            mm.Body = ac.Body.ToString();
            foreach(var attachment in ac.Attachments)
                mm.Attachments.Add(attachment);

            mailSender.Send(mm);

            TempData.Add("FormSubmit", true);

            return RedirectToParentPage();
        }
    }
}
