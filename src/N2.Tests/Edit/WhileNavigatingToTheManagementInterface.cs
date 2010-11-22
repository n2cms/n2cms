using NUnit.Framework;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class WhileNavigatingToTheManagementInterface : EditUrlManagerTests
	{
		[Test]
		public void CanGetManagementInterfaceUrl()
		{
			var url = editUrlManager.GetManagementInterfaceUrl();

			Assert.That(url, Is.EqualTo("/N2/"));
		}

		[Test]
		public void CanResolveManagementUrl_WithNullArgument()
		{
			var url = editUrlManager.ResolveManagementInterfaceUrl(null);

			Assert.That(url, Is.EqualTo("/N2/"));
		}

		[Test]
		public void CanResolveResourceUnderManagementUrl()
		{
			var url = editUrlManager.ResolveManagementInterfaceUrl("Resources/aresource.css");

			Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
		}

		[Test]
		public void CanResolveResourceUnderManagementUrl_UsingManagementTag()
		{
			var url = editUrlManager.ResolveManagementInterfaceUrl("|Management|/Resources/aresource.css");

			Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
		}

		[Test]
		public void DoesNotDisruptVirtualPaths()
		{
			var url = editUrlManager.ResolveManagementInterfaceUrl("~/N2/Resources/aresource.css");

			Assert.That(url, Is.EqualTo("/N2/Resources/aresource.css"));
		}
	}
}