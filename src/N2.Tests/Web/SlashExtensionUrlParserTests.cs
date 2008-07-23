using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
