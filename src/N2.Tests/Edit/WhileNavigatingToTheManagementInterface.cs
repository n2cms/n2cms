using NUnit.Framework;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class WhileNavigatingToTheManagementInterface : EditManagerTests
	{
		[Test]
		public void CanResolveManagementUrl()
		{
			var url = editManager.GetManagementInterfaceUrl();

			Assert.That(url, Is.EqualTo("/N2/"));
		}

		[Test]
		public void CanResolveResourceUnderManagementUrl()
		{
			var url = editManager.ResolveManagementInterfaceUrl("Resources/aresource.css");

			Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
		}

		[Test]
		public void CanResolveResourceUnderManagementUrl_UsingManagementTag()
		{
			var url = editManager.ResolveManagementInterfaceUrl("|Management|/Resources/aresource.css");

			Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
		}

		[Test]
		public void DoesNotDisruptVirtualPaths()
		{
			var url = editManager.ResolveManagementInterfaceUrl("~/N2/Resources/aresource.css");

			Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
		}
	}
}