using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using N2.Configuration;
using N2.Edit;
using N2.Plugin;
using N2.Security;
using N2.Tests.Edit.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Plugin
{
    [TestFixture]
    public class PluginFinderTests : TypeFindingBase
    {
        PluginFinder finder;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

			var edit = new EditSection();// { Editors = new PermissionElement(), Administrators  };
			finder = new PluginFinder(typeFinder, new SecurityManager(new ThreadContext(), edit), TestSupport.SetupEngineSection());
        }

        protected override Type[] GetTypes()
        {
            return new Type[]{
				typeof(ComplexContainersItem),
				typeof(ItemWithRequiredProperty),
				typeof(ItemWithModification),
				typeof(PlugIn1),
				typeof(PlugIn2),
				typeof(PlugIn3),
				typeof(ThrowingPlugin1),
				typeof(ThrowingPlugin2),
				typeof(SeveralPlugins),
				typeof(PermittablePlugins)
			};
        }

        [Test]
        public void CanGetNavigationPlugIns()
        {
            IEnumerable<NavigationPluginAttribute> plugIns = finder.GetPlugins<NavigationPluginAttribute>();
			Assert.That(plugIns.Count(), Is.EqualTo(3));
		}

        [Test]
        public void CanGet_SortNavigation_PlugIns()
        {
            IList<NavigationPluginAttribute> plugIns = new List<NavigationPluginAttribute>(finder.GetPlugins<NavigationPluginAttribute>());
			Assert.That(plugIns.Count(), Is.EqualTo(3));

            NavigationPluginAttribute plugin1 = plugIns[0];
            Assert.AreEqual("chill", plugin1.Name);
            Assert.AreEqual("Chill in", plugin1.Title);

            NavigationPluginAttribute plugin2 = plugIns[1];
            Assert.AreEqual("buzz", plugin2.Name);
            Assert.AreEqual("Buzz out", plugin2.Title);
        }

        [Test]
        public void Doesnt_GetNavigationPlugins_ThatRequires_SpecialAuthorization()
        {
			IPrincipal user = CreatePrincipal("Joe", "Carpenter");
			var plugIns = finder.GetPlugins<NavigationPluginAttribute>(user).ToList();
			Assert.That(plugIns.Count, Is.EqualTo(1));
		}

        [Test]
        public void CanGet_Restricted_NavigationPlugins_IfAuthorized()
        {
			IPrincipal user = CreatePrincipal("Bill", "ÜberEditor");
            IEnumerable<NavigationPluginAttribute> plugIns = finder.GetPlugins<NavigationPluginAttribute>(user);
			Assert.That(plugIns.Count(), Is.EqualTo(2));
		}

        [Test]
        public void CanGet_ToolbarPlugIns()
        {
            IEnumerable<ToolbarPluginAttribute> plugIns = finder.GetPlugins<ToolbarPluginAttribute>();
			Assert.That(plugIns.Count(), Is.EqualTo(3));
		}

        [Test]
        public void CanGetSortToolbarPlugIns()
        {
            IList<ToolbarPluginAttribute> plugIns = new List<ToolbarPluginAttribute>(finder.GetPlugins<ToolbarPluginAttribute>());
			Assert.That(plugIns.Count(), Is.EqualTo(3));

            ToolbarPluginAttribute plugin1 = plugIns[0];
            Assert.AreEqual("peace", plugin1.Name);
            Assert.AreEqual("Don't worry be happy", plugin1.Title);

            ToolbarPluginAttribute plugin2 = plugIns[1];
            Assert.AreEqual("panic", plugin2.Name);
            Assert.AreEqual("Worry we're coming", plugin2.Title);
        }

        [Test]
        public void DoesntGet_ToolbarPlugins_ThatRequires_SpecialAuthorization()
        {
			IPrincipal user = CreatePrincipal("Joe", "Carpenter");
            var plugIns = finder.GetPlugins<ToolbarPluginAttribute>(user);
			Assert.That(plugIns.Count(), Is.EqualTo(1));
        }

		[Test]
		public void CanGet_AllRestrictedToolbarPlugins_IfAuthorized()
		{
			IPrincipal user = CreatePrincipal("Bill", "ÜberEditor");
			var plugIns = finder.GetPlugins<ToolbarPluginAttribute>(user);
			Assert.That(plugIns.Count(), Is.EqualTo(2));
		}

		[Test]
		public void DoesntGet_ToolbarPlugins_ThatRequires_SpecialPermission()
		{
			IPrincipal user = CreatePrincipal("Joe", "Editors");
			var plugIns = finder.GetPlugins<ToolbarPluginAttribute>(user);
			Assert.That(plugIns.Count(), Is.EqualTo(1));
			Assert.That(!plugIns.Any(p => p.Name == "panic2"));
		}

		[Test]
		public void CanGet_AllRestrictedToolbarPlugins_IfPermitted()
		{
			IPrincipal user = CreatePrincipal("Bill", "Administrators");
			var plugIns = finder.GetPlugins<ToolbarPluginAttribute>(user);

			Assert.That(plugIns.Count(), Is.EqualTo(2));
			Assert.That(plugIns.Any(p => p.Name == "panic2"));
		}

		[Test]
		public void CanRemovePlugins_ThroughConfiguration()
		{
			int initialCount = finder.GetPlugins<NavigationPluginAttribute>().Count();
			finder = new PluginFinder(typeFinder, new SecurityManager(new ThreadContext(), new EditSection()), CreateEngineSection(new[] { new InterfacePluginElement { Name = "chill" } }));
			
			IEnumerable<NavigationPluginAttribute> plugins = finder.GetPlugins<NavigationPluginAttribute>();
			
			Assert.That(plugins.Count(), Is.EqualTo(initialCount - 1), "Found unexpected items, e.g.:" + plugins.FirstOrDefault());
		}

    	EngineSection CreateEngineSection(InterfacePluginElement[] removedElements)
    	{
    		return new EngineSection
    		{
				InterfacePlugins = new InterfacePluginCollection
				{
					RemovedElements = removedElements
				}
    		};
    	}
    }
}
