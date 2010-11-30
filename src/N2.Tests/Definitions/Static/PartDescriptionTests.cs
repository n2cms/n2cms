using N2.Definitions.Static;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Definitions.Static
{
	[TestFixture]
	public class PartDescriptionTests
	{
		Description description = DescriptionDictionary.GetDescription(typeof(DescribablePart));
		ContentItem item = new DescribablePart();

		[Test]
		public void Description_IsNotPage()
		{
			Assert.That(description.IsPage, Is.False);
		}

		[Test]
		public void Instance_IsNotPage()
		{
			Assert.That(item.IsPage, Is.False);
		}

		[Test]
		public void Description_HasPartIconUrl()
		{
			Assert.That(description.IsPage, Is.False);
		}

		[Test]
		public void Description_HasDefinitionIconUrl()
		{
			Assert.That(description.IconUrl, Is.EqualTo("/N2/Resources/icons/page_white(description).png"));
		}

		[Test]
		public void Instance_HasPageIconUrl()
		{
			Assert.That(item.IconUrl, Is.EqualTo("/N2/Resources/icons/page_white(iconUrl).png"));
		}

		[Test]
		public void Instance_GetsTemplateUrl_FromDefinition_WhenDefined()
		{
			PathData path = item.FindPath(PathData.DefaultAction);
			Assert.That(path.TemplateUrl, Is.EqualTo("~/My/Template.ascx"));
		}
	}
}