using System;
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
		private FakeTypeFinder typeFinder;
		private IDefinitionManager definitions;

		[SetUp]
		public void SetUp()
		{
			PathDictionary.AllSingletons.Clear();

			typeFinder = new FakeTypeFinder();
			typeFinder.typeMap[typeof(IController)] = new[]
			{
				typeof (ExecutiveTeamController),
				typeof (AboutUsSectionPageController), 
				typeof (RegularController), 
				typeof (FallbackContentController),
				typeof (NonN2Controller),
				typeof (SearchController),
				typeof (TestItemController),
			};

			definitions = MockRepository.GenerateMock<IDefinitionManager>();

			definitions.Expect(d => d.GetDefinitions()).Return(
				new List<ItemDefinition>
					{
						new ItemDefinition(typeof (RegularPage)),
					}
				);
		}

		[Test]
		public void MapperDoesNotMapProperties()
		{
			new ControllerMapper(typeFinder, definitions);

			var actionResolver = PathDictionary.GetFinders(typeof(RegularPage)).FirstOrDefault() as ActionResolver;

			Assert.That(actionResolver, Is.Not.Null);
			Assert.That(actionResolver.Methods.Length, Is.EqualTo(1));
		}

		[Test]
		public void MapperWillNotMap()
		{
			typeFinder.typeMap[typeof (IController)] = new List<Type>(typeFinder.typeMap[typeof (IController)])
			                                           	{
			                                           		typeof (AnotherRegularController),
			                                           	};

			Assert.Throws<N2Exception>(() => new ControllerMapper(typeFinder, definitions), 
				"Duplicate controller AnotherRegularController declared for item type RegularPage." + 
				" The controller RegularController already handles this type and two controllers cannot handle the same item type.");
		}
	}
}