using System.Web.UI.WebControls;
using Dinamico.Controllers;
using Dinamico.Models;
using N2.Definitions.Runtime;
using N2.Web.Mvc;

namespace Dinamico.Registrations
{
    [Registration]
    public class FreeFormRegistration : FluentRegisterer<FreeForm>
    {
        public override void RegisterDefinition(IContentRegistration<FreeForm> register)
        {
            register.Part(title: "Free form", description: "A form that can be sumitted and sent to an email address or viewed online.");

            register.ControlledBy<FreeFormController>();

            register.Definition.SortOrder = 250;
            register.Icon("{IconsUrl}/report.png");

            register.On(ff => ff.Form).FreeText("Form (with tokens)").Configure(eft =>
            {
                eft.HelpTitle = "This text supports tokens";
                eft.HelpText = "{{FormCheckbox}}, {{FormFile}}, {{FormInput}}, {{FormRadio}}, {{FormSelect}}, {{FormSubmit}}, {{FormTextarea}}";
            }).WithTokens();
            register.On(ff => ff.SubmitText).FreeText("Thank you text");

            using (register.FieldSetContainer("Email", "Email").Begin())
            {
                register.On(ff => ff.MailFrom).Text("Mail from").Placeholder("something@mycompany.com");
                register.On(ff => ff.MailTo).Text("Mail to").Placeholder("someone@mycompany.com");
                register.On(ff => ff.MailSubject).Text("Mail subject").Placeholder("Mail title");
                register.On(ff => ff.MailBody).Text("Mail intro text").Placeholder("Mail text before form answers")
                    .Configure(et => et.TextMode = TextBoxMode.MultiLine);
            }
        }
    }
}
