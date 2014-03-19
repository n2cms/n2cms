using System;
using N2.Collections;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class TreeTests : ItemTestsBase
    {
        public override void SetUp()
        {
            base.SetUp();
            BuildHierarchy();
        }

        private ContentItem a, a_a, a_b, a_a_a, a_a_b, a_b_a, a_b_b;

        private void BuildHierarchy()
        {
            int i = 0;
            a = CreateOneItem<PageItem>(++i, "a", null);
            a_a = CreateOneItem<PageItem>(++i, "a_a", a);
            a_b = CreateOneItem<PageItem>(++i, "a_b", a);

            a_a_a = CreateOneItem<PageItem>(++i, "a_a_a", a_a);
            a_a_b = CreateOneItem<PageItem>(++i, "a_a_b", a_a);

            a_b_a = CreateOneItem<PageItem>(++i, "a_b_a", a_b);
            a_b_b = CreateOneItem<PageItem>(++i, "a_b_b", a_b);
        }

        [Test]
        public void CreateTree()
        {
            string treeString = Tree.From(a).ToString();
            string expected
                = "<ul>"
                  + "<li><a href=\"/a\">a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a\">a_a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a_a\">a_a_a</a></li>"
                  + "<li><a href=\"/a_a_b\">a_a_b</a></li>"
                  + "</ul></li>"
                  + "<li><a href=\"/a_b\">a_b</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_b_a\">a_b_a</a></li>"
                  + "<li><a href=\"/a_b_b\">a_b_b</a></li>"
                  + "</ul></li>"
                  + "</ul></li>"
                  + "</ul>";
            Assert.AreEqual(expected, treeString);
        }

        [Test]
        public void CreateBranch()
        {
            string treeString = Tree.Between(a_a_a, a).ToString();
            string expected
                = "<ul>"
                  + "<li><a href=\"/a\">a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a\">a_a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a_a\">a_a_a</a></li>"
                  + "<li><a href=\"/a_a_b\">a_a_b</a></li>"
                  + "</ul></li>"
                  + "<li><a href=\"/a_b\">a_b</a></li>"
                  + "</ul></li>"
                  + "</ul>";
            Assert.AreEqual(expected, treeString);
        }

        [Test]
        public void CreateSmallBranch()
        {
            string treeString = Tree.Between(a_a_a, a_a).ToString();
            string expected
                = "<ul>"
                  + "<li><a href=\"/a_a\">a_a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a_a\">a_a_a</a></li>"
                  + "<li><a href=\"/a_a_b\">a_a_b</a></li>"
                  + "</ul></li>"
                  + "</ul>";
            Assert.AreEqual(expected, treeString);
        }

        [Test]
        public void CanChangeClassProvider()
        {
            string treeString = Tree.From(a_b).ClassProvider(null, delegate(HierarchyNode<ContentItem> n) { return n.Current.Name; })
                .ToString();
            string expected
                = "<ul>"
                  + "<li class=\"a_b\"><a href=\"/a_b\">a_b</a>"
                  + "<ul>"
                  + "<li class=\"a_b_a\"><a href=\"/a_b_a\">a_b_a</a></li>"
                  + "<li class=\"a_b_b\"><a href=\"/a_b_b\">a_b_b</a></li>"
                  + "</ul></li>"
                  + "</ul>";
            Assert.AreEqual(expected, treeString);
        }

        [Test]
        public void CanChangeLinkProvider()
        {
            string treeString =
                Tree.From(a_b).LinkWriter((n, w) => Link.To(n.Current).Class(n.Current.Name).WriteTo(w))
                    .ToString();
            string expected
                = "<ul>"
                  + "<li><a class=\"a_b\" href=\"/a_b\">a_b</a>"
                  + "<ul>"
                  + "<li><a class=\"a_b_a\" href=\"/a_b_a\">a_b_a</a></li>"
                  + "<li><a class=\"a_b_b\" href=\"/a_b_b\">a_b_b</a></li>"
                  + "</ul></li>"
                  + "</ul>";
            Assert.AreEqual(expected, treeString);
        }

        [Test]
        public void ClanOpenToAPage()
        {
            string treeString = Tree.From(a).OpenTo(a_a).ToString();

            string expected
                = "<ul>"
                  + "<li class=\"open\"><a href=\"/a\">a</a>"
                  + "<ul>"
                  + "<li class=\"open\"><a href=\"/a_a\">a_a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a_a\">a_a_a</a></li>"
                  + "<li><a href=\"/a_a_b\">a_a_b</a></li>"
                  + "</ul></li>"
                  + "<li><a href=\"/a_b\">a_b</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_b_a\">a_b_a</a></li>"
                  + "<li><a href=\"/a_b_b\">a_b_b</a></li>"
                  + "</ul></li>"
                  + "</ul></li>"
                  + "</ul>";
            Assert.AreEqual(expected, treeString);
        }

        private class StartsWithFilter : ItemFilter
        {
            private readonly string prefix;

            public StartsWithFilter(string prefix)
            {
                this.prefix = prefix;
            }

            public override bool Match(ContentItem item)
            {
                return item.Name.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        [Test]
        public void CanFilter()
        {
            string treeString = Tree.From(a)
                .Filters(new InverseFilter(new StartsWithFilter("a_b")))
                .ToString();
            string expected
                = "<ul>"
                  + "<li><a href=\"/a\">a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a\">a_a</a>"
                  + "<ul>"
                  + "<li><a href=\"/a_a_a\">a_a_a</a></li>"
                  + "<li><a href=\"/a_a_b\">a_a_b</a></li>"
                  + "</ul></li>"
                  + "</ul></li>"
                  + "</ul>";
            Assert.AreEqual(expected, treeString);
        }

        //[Test]
        //public void CanSetItemClasses()
        //{
        //    a_a.Published = N2.Utility.CurrentTime().AddDays(3);
        //    a_a.AuthorizedRoles.Add(new AuthorizedRole(a_a, "Administrator"));
        //    a_a_b.Expires = N2.Utility.CurrentTime();

        //    string treeString = Tree.From(a_a)
        //        .OpenTo(a_a_a)
        //        .DecorateLinks(a_a_a)
        //        .ToString();
        //    string expected
        //        = "<ul>"
        //            + "<li class=\"open\"><a href=\"/a_a.aspx\" class=\"unpublished locked\">a_a</a>"
        //            + "<ul>"
        //                + "<li class=\"open\"><a href=\"/a_a_a.aspx\" class=\"new selected\">a_a_a</a></li>"
        //                + "<li><a href=\"/a_a_b.aspx\" class=\"new expired\">a_a_b</a></li>"
        //            + "</ul></li>"
        //        + "</ul>";
        //    Assert.AreEqual(expected, treeString);
        //}
    }
}

//[[<ul><li class="open"><a href="/a_a.aspx">a_a</a><ul><li class="open"><a href="/a_a_a.aspx" class="new selected">a_a_a</a></li><li><a href="/a_a_b.aspx" class="expired">a_a_b</a></li></ul></li></ul>]]
//[[<ul><li class="open"><a href="/a_a.aspx">a_a</a><ul><li class="open"><a href="/a_a_a.aspx" class="new selected">a_a_a</a></li><li><a href="/a_a_b.aspx" class="new expired">a_a_b</a></li></ul></li></ul>]]
