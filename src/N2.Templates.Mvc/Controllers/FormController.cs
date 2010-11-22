using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
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
			var questions = CurrentItem.GetChildren(Zones.Questions);

			foreach (var q in questions)
			{
				if (q is IQuestion)
					yield return (IQuestion) q;
			}
		}

		public ActionResult Submit(FormCollection collection)
		{
			var sb = new StringBuilder(CurrentItem.MailBody);
			foreach (IQuestion q in GetQuestions())
			{
				sb.AppendFormat("{0}: {1}{2}", q.QuestionText, q.GetAnswerText(collection[q.ElementID]), Environment.NewLine);
			}
			var mm = new MailMessage(CurrentItem.MailFrom, CurrentItem.MailTo.Replace(";", ","))
			         	{
			         		Subject = CurrentItem.MailSubject,
			         		Body = sb.ToString()
			         	};

			mailSender.Send(mm);

			TempData.Add("FormSubmit", true);

			return RedirectToParentPage();
		}
	}
}