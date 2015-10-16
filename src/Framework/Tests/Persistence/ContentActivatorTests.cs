using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using NUnit.Framework;
using Shouldly;
using N2.Persistence.Proxying;
using N2.Definitions;

namespace N2.Tests.Persistence
{
    [TestFixture]
    public class ContentActivatorTests : ItemTestsBase
    {
        private ContentActivator activator;
        private N2.Edit.Workflow.StateChanger stateChanger;
        private ItemNotifier notifier;
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            activator = new ContentActivator(stateChanger = new N2.Edit.Workflow.StateChanger(), notifier = new ItemNotifier(), new InterceptingProxyFactory());
            activator.Initialize(new[] { new ItemDefinition(typeof(Definitions.PersistableItem)), new ItemDefinition(typeof(Definitions.PersistableItem1b)) });
        }

        [Test]
        public void CreateInstance_creates_insatnce_of_given_type()
        {
            var instance = activator.CreateInstance<Definitions.PersistableItem>(null);

			instance.ShouldBeOfType<Definitions.PersistableItem>();
        }

        [Test]
        public void CreateInstance_creates_insatnce_without_proxy()
        {
            var instance = activator.CreateInstance(typeof(Definitions.PersistableItem), null);

            instance.ShouldBeOfType<Definitions.PersistableItem>();
        }

        [Test]
        public void CreateInstance_creates_insatnce_with_proxys()
        {
            var instance = activator.CreateInstance(typeof(Definitions.PersistableItem), null, null, asProxy: true);

            instance.GetType().BaseType.ShouldBe(typeof(Definitions.PersistableItem));
        }

        [Test]
        public void CreateInstance_creates_insatnce_with_given_parent_without_adding_to_parent_child_collection()
        {
            var root = activator.CreateInstance(typeof(Definitions.PersistableItem), null);
            var instance = activator.CreateInstance(typeof(Definitions.PersistableItem), root);

            instance.Parent.ShouldBe(root);
            root.Children.ShouldNotContain(instance);
        }

        [Test]
        public void CreateInstance_notifies_creation()
        {
            ContentItem created = null;
            activator.ItemCreated += (s, e) => created = e.AffectedItem;

            var instance = activator.CreateInstance(typeof(Definitions.PersistableItem), null);

            created.ShouldBe(instance);
        }

        [Test]
        public void CreateInstance_sets_template_key()
        {
            var instance = activator.CreateInstance(typeof(Definitions.PersistableItem), null, "Hello");

            instance.TemplateKey.ShouldBe("Hello");
        }

        [Test]
        public void Default_values_are_assigned_for_non_proxies()
        {
            var instance = activator.CreateInstance<Definitions.PersistableItem1b>(null, null, asProxy: false);

            instance.CompilerGeneratedIntProperty.ShouldBe(666);
        }

        [Test]
        public void Default_values_are_retrievable_from_proxies()
        {
            var instance = activator.CreateInstance<Definitions.PersistableItem1b>(null, null, asProxy: true);

            instance.CompilerGeneratedIntProperty.ShouldBe(666);
        }
    }
}
