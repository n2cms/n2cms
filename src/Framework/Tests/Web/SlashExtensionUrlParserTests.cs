using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class SlashExtensionUrlParserTests : AbstractExtensionUrlParserTests
    {
        [SetUp]
        public override void SetUp()
        {
            Items.CustomExtensionPage.extension = "/";
            base.SetUp();
        }
    }
}
