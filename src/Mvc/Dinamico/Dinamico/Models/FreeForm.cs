using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using N2.Web.UI;
using System.Web.UI.WebControls;
using Dinamico.Models;
using N2.Definitions.Runtime;
using N2.Web.Mvc;

namespace Dinamico.Models
{
	[Registration]
	public class FreeFormRegistration : FluentRegisterer<FreeForm>
	{
		public override void RegisterDefinition(IContentRegistration<FreeForm> register)
		{
			register.Part(title: "Free form", description: "A form that can be sumitted and sent to an email address or viewed online.");

			register.ControlledBy<Controllers.FreeFormController>();
			
			register.Definition.SortOrder = 250;
			register.Icon("{IconsUrl}/report.png");

			register.On(ff => ff.Form).FreeText("Form (with tokens)").Configure(eft => {
				eft.HelpTitle = "This text supports tokens";
				eft.HelpText = "{{FormCheckbox}}, {{FormFile}}, {{FormCheckbox}}, {{FormInput}}, {{FormRadio}}, {{FormSelect}}, {{FormSubmit}}, {{FormTextarea}}";
			}).WithTokens();
			register.On(ff => ff.SubmitText).FreeText("Thank you text");

			using(register.FieldSet("Email", "Email").Begin())
			{
				register.On(ff => ff.MailFrom).Text("Mail from").Placeholder("something@mycompany.com");
				register.On(ff => ff.MailTo).Text("Mail to").Placeholder("someone@mycompany.com");
				register.On(ff => ff.MailSubject).Text("Mail subject").Placeholder("Mail title");
				register.On(ff => ff.MailBody).Text("Mail intro text").Placeholder("Mail text before form answers")
					.Configure(et => et.TextMode = TextBoxMode.MultiLine);
			}
		}
	}

	public class FreeForm : PartModelBase
	{
		public virtual string Form { get; set; }

		// submit

		public virtual string SubmitText { get; set; }

		// email

		public virtual string MailFrom { get; set; }
		public virtual string MailTo { get; set; }
		public virtual string MailSubject { get; set; }
		public virtual string MailBody { get; set; }
	}
}