using N2.Definitions.Static;
using NUnit.Framework;
using N2.Web;

namespace N2.Tests.Definitions.Static
{
	[TestFixture]
	public class PageDescriptionTests
	{
		Description description = DescriptionDictionary.GetDescription(typeof(DescribablePage));
		ContentItem page = new DescribablePage();

		[Test]
		public void Description_IsPage()
		{
			Assert.That(description.IsPage, Is.True);
		}

		[Test]
		public void Instance_IsPage()
		{
			Assert.That(page.IsPage, Is.True);
		}

		[Test]
		public void Description_HasPageIconUrl()
		{
			Assert.That(description.IconUrl, Is.EqualTo("/N2/Resources/icons/page(description).png"));
		}

		[Test]
		public void Instance_HasPageIconUrl()
		{
			Assert.That(page.IconUrl, Is.EqualTo("/N2/Resources/icons/page(iconUrl).png"));
		}

		[Test]
		public void Instance_DoesntGet_TemplateUrl_FromDefinition_WhenEmpty()
		{
			PathData path = page.FindPath(PathData.DefaultAction);
			Assert.That(path.TemplateUrl, Is.EqualTo("~/Default.aspx"));
		}
	}
}