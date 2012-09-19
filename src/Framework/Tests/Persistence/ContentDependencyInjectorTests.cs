using N2.Engine;
using N2.Engine.Castle;
using N2.Engine.MediumTrust;
using N2.Persistence;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence.Sources;
using N2.Security;

namespace N2.Tests.Persistence
{
	[TestFixture]
	public class WindsorContentDependencyInjectorTests : ContentDependencyInjectorTests
	{
		public override IServiceContainer GetContainer()
		{
			return new WindsorServiceContainer();
		}
	}

	[TestFixture]
	public class MediumTrustContentDependencyInjectorTests : ContentDependencyInjectorTests
	{
		public override IServiceContainer GetContainer()
		{
			return new MediumTrustServiceContainer();
		}
	}

	[TestFixture]
	public class TinyIoCContentDependencyInjectorTests : ContentDependencyInjectorTests
	{
		public override IServiceContainer GetContainer()
		{
			return new N2.Engine.TinyIoC.TinyIoCServiceContainer();
		}
	}

	public class InjectableItem : ContentItem,
		IInjectable<IItemNotifier>,
		IInjectable<IServiceContainer>
	{
		public IItemNotifier dependency1;
		public IServiceContainer dependency2;

		#region IDependentEntity<IItemNotifier> Members

		public void Set(IItemNotifier dependency)
		{
			dependency1 = dependency;
		}

		#endregion

		#region IDependentEntity<IServiceContainer> Members

		public void Set(IServiceContainer dependency)
		{
			dependency2 = dependency;
		}

		#endregion
	}

	public abstract class ContentDependencyInjectorTests
	{
		public abstract IServiceContainer GetContainer();
		ContentDependencyInjector injector;
		ItemNotifier notifier;

		[SetUp]
		public void SetUp()
		{
			notifier = new ItemNotifier();

			var container = GetContainer();
			container.AddComponent("", typeof(EntityDependencySetter<>), typeof(EntityDependencySetter<>));
			container.AddComponentInstance("", typeof(IUrlParser), MockRepository.GenerateStub<IUrlParser>());
			container.AddComponentInstance("", typeof(IServiceContainer), container);
			container.AddComponentInstance("", typeof(IItemNotifier), notifier);

			injector = new ContentDependencyInjector(container, TestSupport.SetupDefinitions(typeof(InjectableItem)), notifier);
			injector.Start();
		}

		[Test]
		public void Dependencies_AreInjected()
		{
			var item = new InjectableItem();
			var changed = notifier.NotifiyCreated(item);

			Assert.That(changed, Is.True);
			Assert.That(item.dependency1, Is.Not.Null);
			Assert.That(item.dependency2, Is.Not.Null);
		}
	}
}
