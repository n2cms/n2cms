using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Castle.Windsor;
using N2.Edit.Settings;
using System.Web.UI.WebControls;

namespace N2.Tests.Edit.Settings
{
	[TestFixture]
	public class SettingsFacilityTests : ItemTestsBase
	{
		private static WindsorContainer CreateContainer()
		{
			WindsorContainer container = new WindsorContainer();
			container.AddComponent("explorer", Type.GetType("N2.Definitions.AttributeExplorer`1, N2"));
			container.AddComponent("manager", typeof(SettingsManager));
			container.AddComponent("packer", typeof(N2.Definitions.EditableHierarchyBuilder<IServiceEditable>), typeof(N2.Definitions.EditableHierarchyBuilder<IServiceEditable>));
			container.AddComponent("usefulService", typeof(Services.IVeryUsefulService), typeof(Services.VeryUsefulService));
			container.AddComponent("settings", typeof(SettingsProvider));
			container.AddFacility("settingsLocator", new SettingsFinder());
			return container;
		}

		[Test]
		public void FindsUsefulService()
		{
			WindsorContainer container = CreateContainer();
			SettingsProvider settings = container.Resolve<SettingsProvider>();

			Assert.AreEqual(1, settings.Settings.Count);
		}

		[Test]
		public void FindsContainers()
		{
			WindsorContainer container = CreateContainer();
			container.AddComponent("anotherService", typeof(Services.JustOneOfThoseServices));
			SettingsProvider settings = container.Resolve<SettingsProvider>();

			N2.Definitions.IEditableContainer rootContainer = settings.RootContainer;
			Assert.IsNotNull(rootContainer);
			IList<N2.Definitions.IContainable> contained = rootContainer.GetContained(null);
			Assert.AreEqual(2, contained.Count, "Should have found an editable and a container");
			Assert.AreEqual(typeof(EditableCheckBoxAttribute), contained[0].GetType(), "The first item should have been the checkbox");
			N2.Web.UI.FieldSetAttribute fieldSet = contained[1] as N2.Web.UI.FieldSetAttribute;
			Assert.IsNotNull(fieldSet, "The second wasn't a fieldset");
			Assert.AreEqual(typeof(EditableCheckBoxAttribute), fieldSet.ContainedEditors[0].GetType(), "The fieldset didn't contain a checkbox");
		}

		[Test]
		public void CanUpdateService()
		{
			N2.Engine.ContentEngine f = new N2.Engine.ContentEngine(System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None));
			f.AddComponent("usefulService", typeof(Services.IVeryUsefulService), typeof(Services.VeryUsefulService));
			f.AddFacility("settingsLocator", new SettingsFinder());

			Services.IVeryUsefulService vus = f.Resolve<Services.IVeryUsefulService>();
			Assert.IsFalse(vus.BeUseful, "Service shouldn't have been useful");

			ISettingsProvider settings = f.Resolve<ISettingsProvider>();
			CheckBox cb = new CheckBox();
			cb.Checked = true;
			settings.Settings[0].UpdateService(f, cb);

			settings = f.Resolve<ISettingsProvider>();
			Assert.IsTrue(vus.BeUseful, "Service should have become useful");
		}
	}
}
