using N2.Details;
using N2.Web;
using N2.Web.UI;

namespace N2.Templates.Items
{
    [Definition("Form page", "FormPage", "A page with a form that can be sumitted and sent to an email address.", "", 240)]
    [TabContainer(FormPage.FormTab, "Form", Tabs.ContentIndex + 2)]
	[ConventionTemplate("Form")]
    public class FormPage : AbstractContentPage
    {
    	public const string FormTab = "formPanel";

        [EditableItem("Form", 60, ContainerName = FormTab)]
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
    }
}