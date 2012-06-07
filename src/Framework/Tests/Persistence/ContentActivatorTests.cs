using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence;
using NUnit.Framework;
using Shouldly;
using N2.Persistence.Proxying;

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

			activator = new ContentActivator(stateChanger = new N2.Edit.Workflow.StateChanger(), notifier = new ItemNotifier(), new EmptyProxyFactory());
		}

		[Test]
		public void CreateInstance_creates_insatnce_of_given_type()
		{
			var instance = activator.CreateInstance<Definitions.PersistableItem1>(null);

			instance.ShouldBeTypeOf<Definitions.PersistableItem1>();
		}

		[Test]
		public void CreateInstance_creates_insatnce_with_given_parent_without_adding_to_parent_child_collection()
		{
			var root = activator.CreateInstance(typeof(Definitions.PersistableItem1), null);
			var instance = activator.CreateInstance(typeof(Definitions.PersistableItem1), root);

			instance.Parent.ShouldBe(root);
			root.Children.ShouldNotContain(instance);
		}

		[Test]
		public void CreateInstance_notifies_creation()
		{
			ContentItem created = null;
			activator.ItemCreated += (s, e) => created = e.AffectedItem;

			var instance = activator.CreateInstance(typeof(Definitions.PersistableItem1), null);

			created.ShouldBe(instance);
		}

		[Test]
		public void CreateInstance_sets_template_key()
		{
			var instance = activator.CreateInstance(typeof(Definitions.PersistableItem1), null, "Hello");

			instance.TemplateKey.ShouldBe("Hello");
		}

		[Test, Ignore("Currently performed by the editable, might make sense to do here")]
		public void Default_values_are_assigned()
		{
			var instance = activator.CreateInstance<Definitions.PersistableItem1>(null);

			instance.IntProperty.ShouldBe(666);
		}
	}
}
