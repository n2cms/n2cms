using N2;
using N2.Templates;

namespace MyProject.Items
{
	[PageDefinition("My First Page", // This attribute marks the MyPage class for usage in the CMS
		TemplateUrl = "~/UI/MyPage.aspx")]
	public class MyPage : N2.Templates.Items.TextPage // Since we're inheriting from TextPage all it's properties are inherited as well
	{
		[N2.Details.EditableTextBox("Author", 160, ContainerName = Tabs.Content)]
		public virtual string Author
		{
			get { return GetDetail("Author", default(string)); }
			set { SetDetail("Author", value, default(string)); }
		}
	}
}
