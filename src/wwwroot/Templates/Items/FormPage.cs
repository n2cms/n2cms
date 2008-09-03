using N2.Details;
using N2.Templates.Items;
using N2.Web.UI;

namespace N2.Templates.Items
{
    [Definition("Form page", "FormPage", "A page with a form that can be sumitted and sent to an email address.", "", 240)]
    [TabPanel("formPanel", "Form", 30)]
    public class FormPage : AbstractContentPage
    {
        [EditableItem("Form", 60, ContainerName = "formPanel")]
        public virtual Form Form
        {
            get { return (Form) GetChild("Form"); }
            set
            {
                if (value != null)
                {
                    value.Name = "Form";
                    value.AddTo(this);
                }
            }
        }

        protected override string IconName
        {
            get { return "report"; }
        }

        protected override string TemplateName
        {
            get { return "Form"; }
        }
    }
}