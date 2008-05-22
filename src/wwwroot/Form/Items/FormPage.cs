using N2.Details;
using N2.Templates.Items;
using N2.Web.UI;

namespace N2.Templates.Form.Items
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

		public override string TemplateUrl
		{
			get { return "~/Form/UI/Default.aspx"; }
		}

		public override string IconUrl
		{
			get { return "~/Form/UI/Img/report.png"; }
		}
	}
}