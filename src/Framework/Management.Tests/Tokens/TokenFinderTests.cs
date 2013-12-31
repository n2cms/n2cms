using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web;
using N2.Web.Tokens;
using System.Web.Mvc;
using Shouldly;
using N2.Management.Tokens;
using N2.Web.Mvc;

namespace N2.Tests.Web.Tokens
{
    [TestFixture]
    public class TokenFinderTests
    {
        private ViewEngineCollection viewEngines;
        private TokenDefinitionFinder finder;
        private Fakes.FakeProvider<ViewEngineCollection> provider;

        [SetUp]
        public void SetUp()
        {
            viewEngines = new ViewEngineCollection();
            provider = new Fakes.FakeProvider<ViewEngineCollection>(() => viewEngines);
            finder = new TokenDefinitionFinder(new Fakes.FakeWebContextWrapper(), provider);
        }

        [Test]
        public void Find_BuiltInTokens()
        {
            viewEngines.RegisterTokenViewEngine();
            
            var definitions = finder.FindTokens().ToList();

            definitions.Single().Name.ShouldBe("Hello");
        }
    }
}
