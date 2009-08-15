using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Extensions.Tests.Fakes;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using N2.Persistence;
using N2.Web;
using N2.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ControllerMapperTests
	{
		IControllerMapper controllerMapper;

		[SetUp]
		public void SetUp()
		{
			PathDictionary.AllSingletons.Clear();

			var typeFinder = new FakeTypeFinder();
			typeFinder.typeMap[typeof(IController)] = new[]
			{
				typeof (ExecutiveTeamController),
				typeof (AboutUsSectionPageController), 
				typeof (RegularControllerBase), 
				typeof (FallbackContentController),
				typeof (NonN2Controller),
				typeof (SearchController),
				typeof (TestItemController),
			};

			var definitions = MockRepository.GenerateMock<IDefinitionManager>();

			definitions.Expect(d => d.GetDefinitions()).Return(
				new List<ItemDefinition>
					{
						new ItemDefinition(typeof (RegularPage)),
					}
				);

			controllerMapper = new ControllerMapper(typeFinder, definitions);
		}

		[Test]
		public void MapperDoesNotMapProperties()
		{
			var actionResolver = PathDictionary.GetFinders(typeof (RegularPage)).FirstOrDefault() as ActionResolver;

			Assert.That(actionResolver, Is.Not.Null);
			Assert.That(actionResolver.Methods.Length, Is.EqualTo(1));
		}
	}
}