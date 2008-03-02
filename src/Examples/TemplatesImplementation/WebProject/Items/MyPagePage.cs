using N2;

namespace MyProject.Items
{
	[Definition("My First Page")]
	public class MyPagePage : N2.Templates.Items.AbstractContentPage
	{
		public override string TemplateUrl
		{
			get { return "~/UI/MyPage.aspx"; }
		}
	}
}
