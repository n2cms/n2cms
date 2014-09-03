using N2.Edit;
using N2.Management.Api;
using N2.Tests.Fakes;
using N2.Tests.Persistence;
using N2.Tests.Persistence.Definitions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Security.Principal;

namespace N2.Tests.Edit.Api
{
    [TestFixture]
    public class InterfaceBuilderTests : DatabasePreparingBase
    {
        private PersistableItem root;
        private PersistableItem item;
        private InterfaceBuilder builder;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            root = CreateAndSaveItem("root", "root", null);
            engine.Host.DefaultSite.RootItemID = root.ID;
            item = CreateAndSaveItem("item", "item", root);
            engine.Host.DefaultSite.StartPageID = item.ID;
            builder = new InterfaceBuilder(engine);
        }

        [TestCase("preview")]
        [TestCase("add")]
        [TestCase("edit")]
        [TestCase("versions")]
		[TestCase("language")]
		[TestCase("transitions")]
		[TestCase("me")]
		[TestCase("info")]
        public void ActionMenu_ContainsSignOut(string expectedMenuItem)
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext(), new SelectionUtility(item, null));
            definition.ActionMenu.Children.Any(mi => mi.Current.Name == expectedMenuItem).ShouldBe(true);
        }

        [Test]
        public void Authority_RepresentsSiteUrl()
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext("http://localhost/N2/Api/Context.ashx/full?selected=" + item.Path), new SelectionUtility(item, null));
            definition.Authority.ShouldBe("localhost");
        }

        [Test]
        public void ContentRoot_IsSiteRoot()
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext(), new SelectionUtility(item, null));
            definition.Content.Current.Title.ShouldBe(root.Title);
        }

        [TestCase("add")]
        [TestCase("edit")]
        [TestCase("delete")]
		[TestCase("security")]
        public void ContextMenu_ContainsMenuItems(string expectedMenuItem)
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext(), new SelectionUtility(item, null));
            definition.ContextMenu.Children.Any(mi => mi.Current.Name == expectedMenuItem).ShouldBe(true);
        }

		[TestCase("dashboard")]
		[TestCase("pages")]
		[TestCase("sitesettings")]
		[TestCase("templates")]
		[TestCase("wizards")]
		[TestCase("users")]
		// depends on config [TestCase("roles")]
        public void MainMenu_ContainsMenuItems(string expectedMenuItem)
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext(), new SelectionUtility(item, null));
            definition.MainMenu.Children.Any(mi => mi.Current.Name == expectedMenuItem).ShouldBe(true);
        }

        [Test]
        public void Partials_ShouldContain_InterfaceComponentPaths()
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext(), new SelectionUtility(item, null));
            definition.Partials.ContextMenu.ShouldNotBeEmpty();
            definition.Partials.Footer.ShouldNotBeEmpty();
            definition.Partials.Main.ShouldNotBeEmpty();
            definition.Partials.Management.ShouldNotBeEmpty();
            definition.Partials.Menu.ShouldNotBeEmpty();
            definition.Partials.Preview.ShouldNotBeEmpty();
            definition.Partials.Tree.ShouldNotBeEmpty();
        }

        [Test]
        public void Paths_ShouldContain_FeaturePaths()
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext(), new SelectionUtility(item, null));
            definition.Paths.Create.ShouldNotBeEmpty();
            definition.Paths.Delete.ShouldNotBeEmpty();
            definition.Paths.Edit.ShouldNotBeEmpty();
            definition.Paths.ItemQueryKey.ShouldNotBeEmpty();
            definition.Paths.Management.ShouldNotBeEmpty();
            definition.Paths.PageQueryKey.ShouldNotBeEmpty();
            definition.Paths.PreviewUrl.ShouldNotBeEmpty();
            definition.Paths.SelectedQueryKey.ShouldNotBeEmpty();
        }

        [Test, Ignore]
        public void Site_ShouldBeCurrentSite()
        {
            var definition = builder.GetInterfaceDefinition(new FakeHttpContext("http://localhost/N2/Api/Context.ashx/full?selected=" + item.Path), new SelectionUtility(item, null));
            definition.Site.RootItemID.ShouldBe(root.ID);
            definition.Site.StartPageID.ShouldBe(item.ID);
        }

		[Test]
		public void User_ShouldBe_LoggedInUser()
		{
			var definition = builder.GetInterfaceDefinition(new FakeHttpContext() { User = new GenericPrincipal(new GenericIdentity("howdy", ""), null) }, new SelectionUtility(item, null));
			definition.User.Name.ShouldBe("howdy");
		}
    }
}
