using System.Configuration;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Engine.MediumTrust;
using N2.Installation;
using N2.Integrity;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Plugin;
using N2.Security;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Engine
{
	[TestFixture]
	public class WindsorCastleEngineTests : EngineSanityTests
	{
		protected override IEngine CreateEngine()
		{
			return new ContentEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None), "n2", EventBroker.Instance);
		}
	}

	[TestFixture]
	public class MediumTrustEngineTests : EngineSanityTests
	{
		protected override IEngine CreateEngine()
		{
			return new MediumTrustEngine(EventBroker.Instance);
		}
	}

	[TestFixture]
	public abstract class EngineSanityTests
	{
		IEngine engine;

		[SetUp]
		public void SetUp()
		{
			engine = CreateEngine();
			engine.Initialize();
		}

		protected abstract IEngine CreateEngine();

		[Test]
		public void CanRetrieve_ImportantServices()
		{
			Assert.That(engine.Resolve<IRequestDispatcher>(), Is.Not.Null);
			Assert.That(engine.Resolve<IWebContext>(), Is.Not.Null);
			Assert.That(engine.Resolve<IHost>(), Is.Not.Null);

			Assert.That(engine.Resolve<IRepository<int, ContentItem>>(), Is.Not.Null);
			Assert.That(engine.Resolve<IRepository<int, ContentDetail>>(), Is.Not.Null);
			Assert.That(engine.Resolve<IRepository<int, AuthorizedRole>>(), Is.Not.Null);
			Assert.That(engine.Resolve<IRepository<int, DetailCollection>>(), Is.Not.Null);
			Assert.That(engine.Resolve<IPersister>(), Is.Not.Null);
			Assert.That(engine.Resolve<IItemFinder>(), Is.Not.Null);
			Assert.That(engine.Resolve<IItemNotifier>(), Is.Not.Null);

			Assert.That(engine.Resolve<IIntegrityManager>(), Is.Not.Null);
			Assert.That(engine.Resolve<IIntegrityEnforcer>(), Is.Not.Null);

			Assert.That(engine.Resolve<ISecurityManager>(), Is.Not.Null);
			Assert.That(engine.Resolve<ISecurityEnforcer>(), Is.Not.Null);

			Assert.That(engine.Resolve<IDefinitionManager>(), Is.Not.Null);

			Assert.That(engine.Resolve<IEngine>(), Is.Not.Null);

			Assert.That(engine.Resolve<IPluginBootstrapper>(), Is.Not.Null);

			Assert.That(engine.Resolve<InstallationManager>(), Is.Not.Null);
		}

		[Test]
		public void AddComponentLifeStyle_DoesNotReturnSameServiceTwiceWhenSingleton()
		{
			engine.AddComponentLifeStyle("testing", typeof(object), ComponentLifeStyle.Singleton);

			var class1 = engine.Resolve<object>();
			var class2 = engine.Resolve<object>();

			Assert.That(class1, Is.SameAs(class2));
		}

		[Test]
		public void AddComponentLifeStyle_DoesNotReturnSameServiceTwiceWhenTransient()
		{
			engine.AddComponentLifeStyle("testing", typeof(object), ComponentLifeStyle.Transient);

			var class1 = engine.Resolve<object>();
			var class2 = engine.Resolve<object>();

			Assert.That(class1, Is.Not.SameAs(class2));
		}
	}
}