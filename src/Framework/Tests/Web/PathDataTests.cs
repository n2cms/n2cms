using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web;
using Shouldly;
using N2.Persistence;
using N2.Details;

namespace N2.Tests.Web
{
    [TestFixture]
    public class PathDataTests
    {
        private ContentPersister persister;
        private Items.PageItem page;
        private Items.DataItem item;
        
        [SetUp]
        public void SetUp()
        {
            persister = TestSupport.SetupFakePersister();
            persister.Save(page = new Items.PageItem { ID = 1 });
            persister.Save(item = new Items.DataItem { ID = 2 });
        }

        [Test]
        public void SetItem()
        {
            var path = new PathData(page);

            path.CurrentItem.ID.ShouldBe(1);
            path.ID.ShouldBe(1);
        }

        [Test]
        public void SetItem_ShouldBeFallback_OfPage()
        {
            var path = new PathData(page);

            path.CurrentPage.ID.ShouldBe(1);
        }

        [Test]
        public void SetItem_ToNull_ShouldGiveNullItem()
        {
            var path = new PathData(null);

            path.CurrentItem.ShouldBe(null);
        }

        [Test]
        public void Item_and_Page_MayDiffer()
        {
            var path = new PathData();

            path.CurrentItem = item;
            path.CurrentPage = page;

			path.CurrentPage.ShouldNotBeSameAs(item);
			path.CurrentItem.ShouldNotBeSameAs(page);
        }

        [Test]
        public void Item_MayBe_null()
        {
            var path = new PathData();

            path.CurrentItem = item;
            path.CurrentItem = null;

            path.CurrentItem.ShouldBe(null);
            path.ID.ShouldBe(0);
        }

        [Test]
        public void Page_MayBe_null()
        {
            var path = new PathData();

            path.CurrentPage = page;
            path.CurrentPage = null;

            path.CurrentPage.ShouldBe(null);
            path.PageID.ShouldBe(0);
        }

        [Test]
        public void Detach_removes_reference_to_item_but_leaves_id()
        {
            var path = new PathData { CurrentItem = page };
            path = path.Detach();

            path.CurrentItem.ShouldBe(null);
            path.ID.ShouldBe(1);
        }

        [Test]
        public void Detach_removes_reference_to_page_but_leaves_id()
        {
            var path = new PathData { CurrentPage = page };
            path = path.Detach();

            path.CurrentPage.ShouldBe(null);
            path.PageID.ShouldBe(1);
        }

        [Test]
        public void Detach_removes_reference_to_StopItem_but_leaves_id()
        {
            var path = new PathData { StopItem = page };
            path = path.Detach();

            path.StopItem.ShouldBe(null);
            path.StopID.ShouldBe(1);
        }

        [Test]
        public void Detach_creates_cloned_object()
        {
            var path = new PathData();
            var detached = path.Detach();

            detached.ShouldNotBeSameAs(path);
        }

        [Test]
        public void Attach_creates_cloned_object()
        {
            var path = new PathData { Action = "hej", CurrentItem = item, Ignore = true, IsCacheable = true, IsPubliclyAvailable = true, IsRewritable = true, Path = "/x", StopItem = page, TemplateUrl = "/hello.aspx" };

            var detached = path.Detach();
            var reattached = detached.Attach(persister);

            reattached.ShouldNotBeSameAs(detached);
        }

        [Test]
        public void Detached_path_values_doesnt_mutate_original()
        {
            var path = new PathData { Action = "hej", CurrentItem = item, Ignore = true, IsCacheable = true, IsPubliclyAvailable = true, IsRewritable = true, Path = "/x", StopItem = page, TemplateUrl = "/hello.aspx" };

            var detached = path.Detach();
            detached.Action = "hejdå";
            detached.CurrentItem = page;
            detached.Ignore = false;
            detached.IsCacheable = false;
            detached.IsPubliclyAvailable = false;
            detached.IsRewritable = false;
            detached.Path = "/y";
            detached.StopItem = item;
            detached.TemplateUrl = "/world.aspx";
            detached.QueryParameters["Hello"] = "world";

            path.Action.ShouldNotBe("hejdå");
            path.CurrentItem.ShouldNotBeSameAs(page);
            path.Ignore.ShouldNotBe(false);
            path.IsCacheable.ShouldNotBe(false);
            path.IsPubliclyAvailable.ShouldNotBe(false);
            path.IsRewritable.ShouldNotBe(false);
            path.Path.ShouldNotBe("/y");
            path.StopItem.ShouldNotBeSameAs(item);
            path.TemplateUrl.ShouldNotBe("/world.aspx");
            path.QueryParameters.ContainsKey("Hello").ShouldBe(false);
        }

        [Test]
        public void Attached_path_values_doesnt_mutate_original()
        {
            var path = new PathData { Action = "hej", CurrentItem = item, Ignore = true, IsCacheable = true, IsPubliclyAvailable = true, IsRewritable = true, Path = "/x", StopItem = page, TemplateUrl = "/hello.aspx" };

            var detached = path.Detach();
            var reattached = detached.Attach(persister);

            detached.Action = "hejdå";
            detached.CurrentItem = page;
            detached.Ignore = false;
            detached.IsCacheable = false;
            detached.IsPubliclyAvailable = false;
            detached.IsRewritable = false;
            detached.Path = "/y";
            detached.StopItem = item;
            detached.TemplateUrl = "/world.aspx";
            detached.QueryParameters["Hello"] = "world";

            reattached.Action.ShouldNotBe("hejdå");
            reattached.CurrentItem.ShouldNotBeSameAs(page);
            reattached.Ignore.ShouldNotBe(false);
            reattached.IsCacheable.ShouldNotBe(false);
            reattached.IsPubliclyAvailable.ShouldNotBe(false);
            reattached.IsRewritable.ShouldNotBe(false);
            reattached.Path.ShouldNotBe("/y");
            reattached.StopItem.ShouldNotBeSameAs(item);
            reattached.TemplateUrl.ShouldNotBe("/world.aspx");
            reattached.QueryParameters.ContainsKey("Hello").ShouldBe(false);
        }

        [Test]
        public void Attach_uses_persister_to_load_item()
        {
            var path = new PathData { CurrentItem = page };
            path = path.Detach();
            path.CurrentItem.ShouldBe(null);

            var loadedPath = path.Attach(persister);
            loadedPath.CurrentItem.ID.ShouldBe(1);
        }

        [Test]
        public void Attach_uses_persister_to_load_page()
        {
            var path = new PathData { CurrentPage = page };
            path = path.Detach();
            path.CurrentPage.ShouldBe(null);

            var loadedPath = path.Attach(persister);
            loadedPath.CurrentPage.ID.ShouldBe(1);
        }

        [Test]
        public void Attach_uses_persister_to_load_stop()
        {
            var path = new PathData { StopItem = page };
            path = path.Detach();
            path.StopItem.ShouldBe(null);

            var loadedPath = path.Attach(persister);
            loadedPath.StopItem.ID.ShouldBe(1);
        }

        [Test]
        public void PubliclyAvailable_is_determined_by_current()
        {
            var path = new PathData { CurrentItem = item, CurrentPage = page };

            path.IsPubliclyAvailable.ShouldBe(true);
        }

        [Test]
        public void PubliclyAvailable_is_determined_by_current_nonpublic_item()
        {
            var path = new PathData { CurrentItem = new Items.DataItem { AlteredPermissions = N2.Security.Permission.Read }, CurrentPage = page };

            path.IsPubliclyAvailable.ShouldBe(false);
        }

        [Test]
        public void PubliclyAvailable_is_determined_by_current_nonpublic_page()
        {
            var path = new PathData { CurrentItem = item, CurrentPage = new Items.PageItem { AlteredPermissions = N2.Security.Permission.Read } };

            path.IsPubliclyAvailable.ShouldBe(false);
        }

        [TestCase(ContentState.Deleted, false)]
        [TestCase(ContentState.Draft, false)]
        [TestCase(ContentState.New, true)]
        [TestCase(ContentState.None, true)]
        [TestCase(ContentState.Published, true)]
        [TestCase(ContentState.Unpublished, false)]
        [TestCase(ContentState.Waiting, false)]
        public void PubliclyAvailable_is_determined_by_current_nonpublished_item(ContentState state, bool expectedAvailability)
        {
            var path = new PathData { CurrentItem = new Items.DataItem { State = state }, CurrentPage = page };

            path.IsPubliclyAvailable.ShouldBe(expectedAvailability);
        }

        [TestCase(ContentState.Deleted, false)]
        [TestCase(ContentState.Draft, false)]
        [TestCase(ContentState.New, true)]
        [TestCase(ContentState.None, true)]
        [TestCase(ContentState.Published, true)]
        [TestCase(ContentState.Unpublished, false)]
        [TestCase(ContentState.Waiting, false)]
        public void PubliclyAvailable_is_determined_by_current_nonpublished_page(ContentState state, bool expectedAvailability)
        {
            var path = new PathData { CurrentItem = item, CurrentPage = new Items.PageItem { State = state } };

            path.IsPubliclyAvailable.ShouldBe(expectedAvailability);
        }

        [Test]
        public void Cloned_path_data_has_same_values()
        {
            var path = new PathData(page, item) { Action = "hello", Argument = "world", Ignore = true, IsCacheable = false, IsPubliclyAvailable = true, IsRewritable = false, TemplateUrl = "asdf" };
            var clone = path.Clone();

            path.Action.ShouldBe(clone.Action);
            path.Argument.ShouldBe(clone.Argument);
            path.CurrentItem.ShouldBe(clone.CurrentItem);
            path.CurrentPage.ShouldBe(clone.CurrentPage);
            path.ID.ShouldBe(clone.ID);
            path.Ignore.ShouldBe(clone.Ignore);
            path.IsCacheable.ShouldBe(clone.IsCacheable);
            path.IsPubliclyAvailable.ShouldBe(clone.IsPubliclyAvailable);
            path.IsRewritable.ShouldBe(clone.IsRewritable);
            path.PageID.ShouldBe(clone.PageID);
            path.Path.ShouldBe(clone.Path);
            path.QueryParameters.Count.ShouldBe(clone.QueryParameters.Count);
            path.StopID.ShouldBe(clone.StopID);
            path.StopItem.ShouldBe(clone.StopItem);
            path.TemplateUrl.ShouldBe(clone.TemplateUrl);
        }

        [Test]
        public void Cloned_path_data_should_not_be_same_item()
        {
            var path = new PathData(page, item);
            var clone = path.Clone();

            clone.ShouldNotBeSameAs(path);
            clone.QueryParameters.ShouldNotBeSameAs(path.QueryParameters);
        }

        [Test]
        public void ToString_should_maintina_page_and_item_info()
        {
            var path = new PathData(page, item);

            var reparsed = PathData.Parse(path.ToString(), persister);

            reparsed.CurrentItem.ShouldBe(path.CurrentItem);
            reparsed.CurrentPage.ShouldBe(path.CurrentPage);
        }
    }
}
