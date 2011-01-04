using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Tests.Persistence;
using NUnit.Framework;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Web;
using N2.Edit.Workflow;
using N2.Security;
using N2.Tests.Fakes;

namespace N2.Tests.Edit
{
	public class NodeAdapterTests : N2.Tests.ItemPersistenceMockingBase
	{
		NodeAdapter adapter;
		FakeFileSystem2 fs;

		ContentItem root;
		ContentItem start;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			root = CreateOneItem<Items.NormalPage>(1, "root", null);
			start = CreateOneItem<Items.NormalPage>(2, "start", root);

			adapter = new NodeAdapter();
			adapter.ManagementPaths = new EditUrlManager(new N2.Configuration.EditSection());
			adapter.FileSystem = fs = new FakeFileSystem2();
			adapter.NodeFactory = new VirtualNodeFactory();
			adapter.WebContext = new Fakes.FakeWebContextWrapper();
			adapter.Security = new SecurityManager(adapter.WebContext, new N2.Configuration.EditSection());
			adapter.Host = new Host(null, root.ID, start.ID);
		}

		[Test]
		public void VirtualNodes_AreAdded_ToTheirParent()
		{
			adapter.NodeFactory.Register(new FunctionalNodeProvider("/hello/", (path) => new Items.NormalPage { Name = "World" }));

			var children = adapter.GetChildren(root, Interfaces.Managing);
			Assert.That(children.Count(), Is.EqualTo(1 + root.Children.Count));
			Assert.That(children.Any(c => c.Name == "World"));
		}

		[Test]
		public void VirtualNodes_AreNotAdded_ToOtherNodes()
		{
			adapter.NodeFactory.Register(new FunctionalNodeProvider("/hello/", (path) => new Items.NormalPage { Name = "World" }));

			var children = adapter.GetChildren(start, Interfaces.Managing);
			Assert.That(children.Count(), Is.EqualTo(start.Children.Count));
			Assert.That(children.Any(c => c.Name == "World"), Is.False);
		}

		[Test]
		public void VirtualNodes_CanBeAdded_ToADeeperLevel()
		{
			adapter.NodeFactory.Register(new FunctionalNodeProvider("/start/hello/", (path) => new Items.NormalPage { Name = "World" }));

			var children = adapter.GetChildren(start, Interfaces.Managing);
			Assert.That(children.Count(), Is.EqualTo(1 + start.Children.Count));
			Assert.That(children.Any(c => c.Name == "World"));
		}

		[Test]
		public void GlobalUploadFolders_AreNotAdded()
		{
			fs.CreateDirectory("~/upload/");

			var uploads = adapter.GetUploadDirectories(adapter.Host.DefaultSite);
			Assert.That(uploads.Count(), Is.EqualTo(0));
		}

		[Test]
		public void SiteUploadFolders_AreAdded()
		{
			fs.CreateDirectory("~/upload/");
			fs.CreateDirectory("~/siteupload/");
			adapter.Host.DefaultSite.UploadFolders.Add("~/siteupload/");

			var uploads = adapter.GetUploadDirectories(adapter.Host.DefaultSite);

			Assert.That(uploads.Count(), Is.EqualTo(1));
			Assert.That(uploads.Any(u => u.VirtualPath == "~/siteupload/"));
		}
	}
}
