using N2.Engine;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Web
{
    [TestFixture]
    public class UrlTests
    {
        [Test]
        public void CanConstruct_HomePath()
        {
            Url u = new Url("/");
            Assert.That(u.Path, Is.EqualTo("/"));
            Assert.That(u.ToString(), Is.EqualTo("/"));
        }

        [Test]
        public void EmptyUrl()
        {
            Url u = new Url("");
            Assert.That(u.Path, Is.EqualTo(""));
            Assert.That(u.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void NullUrl()
        {
            Url u = new Url((string)null);
            Assert.That(u.Path, Is.EqualTo(""));
            Assert.That(u.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath()
        {
            Url u = new Url("/hello.aspx");
            Assert.That(u.Path, Is.EqualTo("/hello.aspx"));
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx"));
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath_WithQuery()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            Assert.That(u.Path, Is.EqualTo("/hello.aspx"));
            Assert.That(u.Query, Is.EqualTo("something=someotherthing"));
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx?something=someotherthing"));
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath_WithFragment()
        {
            Url u = new Url("/hello.aspx#somebookmark");
            Assert.That(u.Path, Is.EqualTo("/hello.aspx"));
            Assert.That(u.Fragment, Is.EqualTo("somebookmark"));
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx#somebookmark"));
        }

        [Test]
        public void CanConstruct_AbsoluteLocalPath_WithQuery_AndFragment()
        {
            Url u = new Url("/hello.aspx?something=someotherthing#somebookmark");
            Assert.That(u.Path, Is.EqualTo("/hello.aspx"));
            Assert.That(u.Query, Is.EqualTo("something=someotherthing"));
            Assert.That(u.Fragment, Is.EqualTo("somebookmark"));
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx?something=someotherthing#somebookmark"));
        }

        [Test]
        public void CanConstruct_FromHostName()
        {
            Url u = new Url("http://somesite/");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite"));
            Assert.That(u.ToString(), Is.EqualTo("http://somesite/"));
        }

        [Test]
        public void CanConstruct_FromHostName_WithoutTrailingSlash()
        {
            Url u = new Url("http://somesite");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite"));
            Assert.That(u.Path, Is.EqualTo("/"));
            Assert.That(u.ToString(), Is.EqualTo("http://somesite/"));
        }

        [Test]
        public void CanConstruct_FromHostName_WithPort()
        {
            Url u = new Url("http://somesite:8080/");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite:8080"));
            Assert.That(u.ToString(), Is.EqualTo("http://somesite:8080/"));
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath()
        {
            Url u = new Url("http://somesite/some/path");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite"));
            Assert.That(u.Path, Is.EqualTo("/some/path"));
            Assert.That(u.ToString(), Is.EqualTo("http://somesite/some/path"));
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath_AndQuery()
        {
            Url u = new Url("http://somesite/some/path?key=value");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite"));
            Assert.That(u.Path, Is.EqualTo("/some/path"));
            Assert.That(u.Query, Is.EqualTo("key=value"));
            Assert.That(u.ToString(), Is.EqualTo("http://somesite/some/path?key=value"));
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath_AndFragment()
        {
            Url u = new Url("http://somesite/some/path#somebookmark");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite"));
            Assert.That(u.Path, Is.EqualTo("/some/path"));
            Assert.That(u.Fragment, Is.EqualTo("somebookmark"));
            Assert.That(u.ToString(), Is.EqualTo("http://somesite/some/path#somebookmark"));
        }

        [Test]
        public void CanConstruct_FromHostName_WithPath_AndQuery_AndFragment()
        {
            Url u = new Url("http://somesite/some/path?key=value#bookmark");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite"));
            Assert.That(u.Path, Is.EqualTo("/some/path"));
            Assert.That(u.Query, Is.EqualTo("key=value"));
            Assert.That(u.Fragment, Is.EqualTo("bookmark"));
            Assert.That(u.ToString(), Is.EqualTo("http://somesite/some/path?key=value#bookmark"));
        }

        [Test]
        public void CanImplicitlyConvert_FromString()
        {
            Url u = "http://somesite/some/path?key=value#bookmark";
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("somesite"));
            Assert.That(u.Path, Is.EqualTo("/some/path"));
            Assert.That(u.Query, Is.EqualTo("key=value"));
            Assert.That(u.Fragment, Is.EqualTo("bookmark"));
        }

        [Test]
        public void CanImplicitlyConvert_ToString()
        {
            Url u = "http://somesite/some/path?key=value#bookmark";
            string url = u;
            Assert.That(url, Is.EqualTo("http://somesite/some/path?key=value#bookmark"));
        }

        [Test]
        public void CanConstruct_RelativePath()
        {
            Url u = "../Default.aspx?key=value#bookmark";
            Assert.That(u.Path, Is.EqualTo("../Default.aspx"));
            Assert.That(u.Query, Is.EqualTo("key=value"));
            Assert.That(u.Fragment, Is.EqualTo("bookmark"));
        }

        [Test]
        public void CanAppend_KeyAndValue_ToEmpyQueryString()
        {
            Url u = "/";
            u = u.AppendQuery("key", "value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }
        [Test]
        public void CanAppend_KeyValuePair_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.AppendQuery("key=value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }

        [Test]
        public void CanAppend_KeyAndValue_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.AppendQuery("key", "value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&key=value"));
        }
        [Test]
        public void CanAppend_KeyValuePair_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.AppendQuery("key=value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&key=value"));
        }

        [Test]
        public void CanSet_KeyAndValue_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.SetQueryParameter("key", "value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }

        [Test]
        public void CanSet_KeyValuePair_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.SetQueryParameter("key=value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }

        [Test]
        public void CanSet_KeyAndValue_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.SetQueryParameter("key", "value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&key=value"));
        }
        [Test]
        public void CanSet_KeyValuePair_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.SetQueryParameter("key=value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&key=value"));
        }

        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyAndValue()
        {
            Url u = "/?key=somevalue";
            u = u.SetQueryParameter("key", "someothervalue");
            Assert.That(u.Query, Is.EqualTo("key=someothervalue"));
        }
        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyValuePair()
        {
            Url u = "/?key=somevalue";
            u = u.SetQueryParameter("key=someothervalue");
            Assert.That(u.Query, Is.EqualTo("key=someothervalue"));
        }

        [Test]
        public void CanRemoveValue_UsingKeyAndValue()
        {
            Url u = "/";
            u = u.SetQueryParameter("key", null, true);
            Assert.That(u.Query, Is.Null);
        }

        [Test]
        public void CanRemoveQuery()
        {
            Url u = "/?key=value&key2=value2";
            u = u.RemoveQuery("key");
            Assert.That(u.ToString(), Is.EqualTo("/?key2=value2"));
        }

        [Test]
        public void CanRemoveQuery2()
        {
            Url u = "/?key=value&key2=value2";
            u = u.RemoveQuery("key2");
            Assert.That(u.ToString(), Is.EqualTo("/?key=value"));
        }

        [Test]
        public void CanRemoveValue_OnExistingQueryString_UsingKeyAndValue()
        {
            Url u = "/?key=somevalue";
            u = u.SetQueryParameter("key", null, true);
            Assert.That(u.Query, Is.Null);
        }

        [Test]
        public void AppendedValue_IsUrlEncoded()
        {
            Url u = "/";
            u = u.AppendQuery("key", "cristian & maria");
            Assert.That(u.Query, Is.EqualTo("key=cristian+%26+maria"));
        }

        [Test]
        public void SetValue_IsUrlEncoded()
        {
            Url u = "/key=sometihng";
            u = u.SetQueryParameter("key", "cristian & maria");
            Assert.That(u.Query, Is.EqualTo("key=cristian+%26+maria"));
        }

        [Test]
        public void CanSetScheme()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetScheme("https");
            Assert.That(u.ToString(), Is.EqualTo("https://n2cms.com/test.aspx?key=value"));
        }

        [Test]
        public void CanSetAuthority()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetAuthority("n2cms.com:8080");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com:8080/test.aspx?key=value"));
        }

        [Test]
        public void CanSetPath()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetPath("/test2.aspx");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2.aspx?key=value"));
        }

        [Test]
        public void CanSetQuery()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.SetQuery("key2=value2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test.aspx?key2=value2"));
        }

        [Test]
        public void CanSetFragment()
        {
            Url u = "http://n2cms.com/test.aspx?key=value#fragment";
            u = u.SetFragment("fragment2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test.aspx?key=value#fragment2"));
        }

        [Test]
        public void CanAppendSegment()
        {
            Url u = "http://n2cms.com/test.aspx?key=value";
            u = u.AppendSegment("test2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test/test2.aspx?key=value"));
        }

        [Test]
        public void CanAppendSegment_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.AppendSegment("test2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2.aspx"));
        }

        [Test]
        public void CanAppendSegment_UsingDefaultExtension_ToPathWithNoExtension()
        {
            Url u = "http://n2cms.com/path";
            u = u.AppendSegment("test2", true);
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/path/test2.aspx"));
        }

        [Test]
        public void CanAppendSegment_UsingDefaultExtension_ToPathWithTrailingSlash()
        {
            Url u = "http://n2cms.com/path/";
            u = u.AppendSegment("test2", true);
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/path/test2.aspx"));
        }

        [Test]
        public void Extension_WillNotCrossSegments()
        {
            Url u = "/hello.world/universe";
            Assert.That(u.Extension, Is.Null);
        }

        [Test]
        public void PathWithoutExtension_WillNotCrossSegments()
        {
            Url u = "/hello.world/universe";
            Assert.That(u.PathWithoutExtension, Is.EqualTo("/hello.world/universe"));
        }

        //[Test]
        //public void CanAppendSegment_UsingDefaultExtension_ToPathWithOtherExtension()
        //{
        //    Url u = "http://n2cms.com/path.html";
        //    u = u.AppendSegment("test2", true);
        //    Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/path/test2.aspx"));
        //}

        [Test]
        public void CanAppendSegment_UsingDefaultExtension_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.AppendSegment("test2", true);
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2.aspx"));
        }

        [Test]
        public void CanAppendSegment_ToPathWithoutExtension()
        {
            Url u = "http://n2cms.com/wiki";
            u = u.AppendSegment("test");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/wiki/test"));
        }

        [Test]
        public void CanAppendSegment_ToPathWithTrailingSlash()
        {
            Url u = "http://n2cms.com/wiki/";
            u = u.AppendSegment("test");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/wiki/test"));
        }

        [Test]
        public void CanPrependSegment_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.PrependSegment("test2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2.aspx"));
        }

        [Test]
        public void CanPrependSegment_ToSimplePath()
        {
            Url u = "http://n2cms.com/test.aspx";
            u = u.PrependSegment("test2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2/test.aspx"));
        }

        [Test]
        public void CanControlExtension_WhilePrependingPath_ToEmptyPath()
        {
            Url u = "http://n2cms.com/";
            u = u.PrependSegment("test", ".html");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test.html"));
        }

        [Test]
        public void CanControlExtension_WhilePrependingPath()
        {
            Url u = "http://n2cms.com/test.aspx";
            u = u.PrependSegment("test2", ".html");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2/test.html"));
        }

        [Test]
        public void WillNotUse_DefaultExtension_WhenAppendingSegment_ToPathWithNoExtension()
        {
            Url u = "http://n2cms.com/test";
            u = u.AppendSegment("test2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test/test2"));
        }

        [Test]
        public void WillUse_UrlExtension_WhenAppendingSegment()
        {
            Url u = "http://n2cms.com/test.html";
            u = u.AppendSegment("test2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test/test2.html"));
        }

        [Test]
        public void CanAppendSegment_WithoutUsingDefaultExtension()
        {
            Url u = "http://n2cms.com/hello";
            u = u.AppendSegment("test2", false);
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/hello/test2"));
        }

        [Test]
        public void CanRemove_TrailingSegment()
        {
            Url u = "/hello/world";

            u = u.RemoveTrailingSegment();

            Assert.That(u.ToString(), Is.EqualTo("/hello"));
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenTrailingSlash()
        {
            Url u = "/hello/world/";

            u = u.RemoveTrailingSegment();

            Assert.That(u.ToString(), Is.EqualTo("/hello")); // tiny inconsitency
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenFileExtension()
        {
            Url u = "/hello/world.aspx";

            u = u.RemoveTrailingSegment();

            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx"));
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenSingleSegment()
        {
            Url u = "/hello";

            u = u.RemoveTrailingSegment();

            Assert.That(u.ToString(), Is.EqualTo("/"));
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenSingleSegment_AndTrailingSlash()
        {
            Url u = "/hello/";

            u = u.RemoveTrailingSegment();

            Assert.That(u.ToString(), Is.EqualTo("/"));
        }

        [Test]
        public void CanRemove_TrailingSegment_WhenSingleSegment_AndExtension()
        {
            Url u = "/hello.aspx";

            u = u.RemoveTrailingSegment();

            Assert.That(u.ToString(), Is.EqualTo("/"));
        }

        [TestCase("/hello/world.aspx", "/hello/")]
        [TestCase("/hello/world/", "/hello/")]
        [TestCase("/hello.aspx", "/")]
        [TestCase("/hello/", "/")]
        [TestCase("/", "")]
        [TestCase("", null)]
        public void CanRemove_LastSegment(string input, string expected)
        {
            string result = Url.RemoveLastSegment(input);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("/hello/world.aspx", "world.aspx")]
        [TestCase("/hello/world/", "world")]
        [TestCase("/hello.aspx", "hello.aspx")]
        [TestCase("/hello/", "hello")]
        [TestCase("/", "")]
        [TestCase("", null)]
        public void CanGet_Name(string input, string expected)
        {
            string result = Url.GetName(input);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanRemove_SegmentIndex0()
        {
            Url u = "/hello/whole/world";
            
            u = u.RemoveSegment(0);

            Assert.That(u.ToString(), Is.EqualTo("/whole/world"));
        }

        [Test]
        public void CanRemove_SegmentIndex0_WhenTrailingSlash()
        {
            Url u = "/hello/whole/world/";

            u = u.RemoveSegment(0);

            Assert.That(u.ToString(), Is.EqualTo("/whole/world/"));
        }

        [Test]
        public void CanRemove_SegmentIndex0_WhenFileExtension()
        {
            Url u = "/hello/whole/world.aspx";

            u = u.RemoveSegment(0);

            Assert.That(u.ToString(), Is.EqualTo("/whole/world.aspx"));
        }

        [Test]
        public void CanRemove_SegmentIndex1()
        {
            Url u = "/hello/whole/world";

            u = u.RemoveSegment(1);

            Assert.That(u.ToString(), Is.EqualTo("/hello/world"));
        }

        [Test]
        public void CanRemove_SegmentIndex1_WhenTrailingSlash()
        {
            Url u = "/hello/whole/world/";

            u = u.RemoveSegment(1);

            Assert.That(u.ToString(), Is.EqualTo("/hello/world")); // tiny inconsitency
        }

        [Test]
        public void CanRemove_SegmentIndex1_WhenFileExtension()
        {
            Url u = "/hello/whole/world.aspx";

            u = u.RemoveSegment(1);

            Assert.That(u.ToString(), Is.EqualTo("/hello/world.aspx")); // tiny inconsitency
        }

        [Test]
        public void CanRemove_LastSegmentIndex()
        {
            Url u = "/hello/whole/world";

            u = u.RemoveSegment(2);

            Assert.That(u.ToString(), Is.EqualTo("/hello/whole"));
        }

        [Test]
        public void CanRemove_LastSegmentIndex_WhenTrailingSlash()
        {
            Url u = "/hello/whole/world/";

            u = u.RemoveSegment(2);

            Assert.That(u.ToString(), Is.EqualTo("/hello/whole"));
        }

        [Test]
        public void CanRemove_LastSegmentIndex_WhenFileExtension()
        {
            Url u = "/hello/whole/world.aspx";

            u = u.RemoveSegment(2);

            Assert.That(u.ToString(), Is.EqualTo("/hello/whole.aspx"));
        }

        [Test]
        public void RemoveSegment_GracefullyExcepts_ArgumentsBeyondBounds()
        {
            Url u = "/hello.aspx";

            u = u.RemoveSegment(2);

            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx"));
        }

        [Test]
        public void RemoveSegment_GracefullyExcepts_BeforeBounds()
        {
            Url u = "/hello.aspx";

            u = u.RemoveSegment(-1);

            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx"));
        }

        [Test]
        public void CanAppendSegment_WithoutUsingDefaultExtension_ToEmptyPath()
        {
            Url u = "http://n2cms.com";
            u = u.AppendSegment("test2", false);
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2"));
        }

        [Test]
        public void CanAppendSegment_ToEmptyPath_WithTrailingSlash()
        {
            Url u = "http://n2cms.com/";
            u = u.AppendSegment("test2");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/test2.aspx"));
        }

        [Test]
        public void CanSetPath_WithQueryString()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            u = u.SetPath("/new/path?new=querystring");
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com/new/path?existing=query"));
        }

        [Test]
        public void CanCombine_PathAndQuery()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            Assert.That(u.PathAndQuery, Is.EqualTo("/some/path.aspx?existing=query"));
        }

        [Test]
        public void CanParsePath_WithSlashInQuery()
        {
            Url u = "/UI/500.aspx?aspxerrorpath=/Default.aspx";
            Assert.That(u.Path, Is.EqualTo("/UI/500.aspx"));
            Assert.That(u.Query, Is.EqualTo("aspxerrorpath=/Default.aspx"));
        }

        [Test]
        public void CanDetermine_DefaultExtension()
        {
            Url u = "/path/to/page.aspx";
            Assert.That(u.Extension, Is.EqualTo(".aspx"));
        }

        [Test]
        public void CanDetermine_HtmlExtension()
        {
            Url u = "/path/to/page.html";
            Assert.That(u.Extension, Is.EqualTo(".html"));
        }

        [Test]
        public void CanDetermine_NoExtension()
        {
            Url u = "/path/to/page";
            Assert.That(u.Extension, Is.Null);
        }

        [Test]
        public void CanDetermine_NoExtension_WhenTrailingSlash()
        {
            Url u = "/path/to/page/";
            Assert.That(u.Extension, Is.Null);
        }

        [Test]
        public void CanSplitPath_IntoPathWithoutExtension()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            Assert.That(u.PathWithoutExtension, Is.EqualTo("/hello"));
        }

        [Test]
        public void CanGet_QueryDictionary()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            var q = u.GetQueries();
            Assert.That(q.Count, Is.EqualTo(1));
            Assert.That(q["something"], Is.EqualTo("someotherthing"));
        }

        [Test]
        public void CanGet_EmptyQueryDictionary()
        {
            Url u = new Url("/hello.aspx");
            var q = u.GetQueries();
            Assert.That(q.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanGet_Query()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            var q = u.GetQuery("something");
            Assert.That(q, Is.EqualTo("someotherthing"));
        }

        [Test]
        public void CanGet_NameVAlueCollection()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            var q = u.GetQueryNameValues()["something"];
            Assert.That(q, Is.EqualTo("someotherthing"));
        }

        [Test]
        public void Getting_NonExistantQuery_GivesNull()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            var q = u.GetQuery("nothing");
            Assert.That(q, Is.Null);
        }

        [Test]
        public void Getting_Query_WhenNoQueries_GivesNull()
        {
            Url u = new Url("/hello.aspx");
            var q = u.GetQuery("something");
            Assert.That(q, Is.Null);
        }

        [Test]
        public void UpdatingQueryToNull_WhenSingleParameter_RemovesItFromUrl()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            u = u.SetQueryParameter("something", null);
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx"));
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameter_WhenUpdatingFirst()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value");
            u = u.SetQueryParameter("something", null);
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx?query=value"));
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameter_WhenUpdatingSecond()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value");
            u = u.SetQueryParameter("query", null);
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx?something=someotherthing"));
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameters_WhenUpdatingFirst()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value&query3=value3");
            u = u.SetQueryParameter("something", null);
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx?query=value&query3=value3"));
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameters_WhenUpdatingInMiddle()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value&query3=value3");
            u = u.SetQueryParameter("query", null);
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx?something=someotherthing&query3=value3"));
        }

        [Test]
        public void UpdatingQueryToNull_ReturnsOtherParameters_WhenUpdatingLast()
        {
            Url u = new Url("/hello.aspx?something=someotherthing&query=value&query3=value3");
            u = u.SetQueryParameter("query3", null);
            Assert.That(u.ToString(), Is.EqualTo("/hello.aspx?something=someotherthing&query=value"));
        }

        [Test]
        public void CanChangeExtension()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            u = u.SetExtension(".html");
            Assert.That(u.ToString(), Is.EqualTo("/hello.html?something=someotherthing"));
        }

        [Test]
        public void CanClearExtension()
        {
            Url u = new Url("/hello.aspx?something=someotherthing");
            u = u.SetExtension("");
            Assert.That(u.ToString(), Is.EqualTo("/hello?something=someotherthing"));
        }

        [Test]
        public void CanSplitOut_HostPart()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            u = u.HostUrl;
            Assert.That(u.ToString(), Is.EqualTo("http://n2cms.com"));
        }

        [Test]
        public void CanSplitOut_HostPart_AndGetExtension()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            string extension = u.HostUrl.Extension;
            Assert.That(extension, Is.Null);
        }

        [Test]
        public void CanSplitOut_LocalPart()
        {
            Url u = "http://n2cms.com/some/path.aspx?existing=query";
            u = u.LocalUrl;
            Assert.That(u.ToString(), Is.EqualTo("/some/path.aspx?existing=query"));
        }

        [Test]
        public void CanConstruct_WithBaseSchemeAndRawUrl()
        {
            Url u = new Url("http", "www.n2cms.com", "/Default.aspx?");
            Assert.That(u.Scheme, Is.EqualTo("http"));
            Assert.That(u.Authority, Is.EqualTo("www.n2cms.com"));
            Assert.That(u.PathAndQuery, Is.EqualTo("/Default.aspx"));
        }

        [Test]
        public void CanRecognize_AbsoluteUrl()
        {
            Url u = "http://www.n2cms.com";
            Assert.That(u.IsAbsolute, Is.True);
        }

        [Test]
        public void CanRecognize_LocalUrl()
        {
            Url u = "/";
            Assert.That(u.IsAbsolute, Is.False);
        }

        [TestCase("", "", "")]
        [TestCase("", "/", "/")]
        [TestCase("/", "", "/")]
        [TestCase("/", "/", "/")]
        [TestCase("", "hello", "hello")]
        [TestCase("/", "/hello", "/hello")]
        [TestCase("/hello", "hello", "/hello/hello")]
        [TestCase("/hello", "/hello", "/hello")]
        [TestCase("/hello.aspx", "hello.aspx", "/hello.aspx/hello.aspx")]
        [TestCase("/hello", "hello?one=1", "/hello/hello?one=1")]
        [TestCase("/hello?one=1", "hello", "/hello/hello?one=1")]
        [TestCase("/hello?one=1", "hello?two=2", "/hello/hello?one=1&two=2")]
        [TestCase("/hello?one=1&two=2", "hello?three=3&four=4", "/hello/hello?one=1&two=2&three=3&four=4")]
        [TestCase("/n2/", "{selected}", "{selected}")]
        [TestCase("/n2/", "javascript:alert(1);", "javascript:alert(1);")]
        [TestCase("/hello", "hello#world", "/hello/hello#world")]
        public void Combine1(string url1, string url2, string expected)
        {
            string result = Url.Combine(url1, url2);
            Assert.That(result, Is.EqualTo(expected), "'" + url1 + "' + '" + url2 + "' != '" + expected + "'");
        }

        [Test]
        public void CanParse_RelativePath_WithUrl_AsQuery()
        {
            Url u = "/path?dns=host.com&url=http://www.hello.net/howdy";
            Assert.That(u.Path, Is.EqualTo("/path"));
            Assert.That(u.Query, Is.EqualTo("dns=host.com&url=http://www.hello.net/howdy"));
        }

        [Test]
        public void ServerUrl_Returns_FallbackValue_InThreadContext()
        {
            var oldEngine = Singleton<IEngine>.Instance;
            try
            {
                var engine = new FakeEngine();
                var webContext = new FakeWebContextWrapper();
                webContext.isWeb = true;
                engine.Container.AddComponentInstance("", typeof(IWebContext), webContext);
                Singleton<IEngine>.Instance = engine;

                webContext.Url = "http://site1/app";
                Assert.That(Url.ServerUrl, Is.EqualTo("http://site1"));

                webContext.isWeb = false;
                webContext.Url = "http://site2/app";
                Assert.That(Url.ServerUrl, Is.EqualTo("http://site1"));
            }
            finally
            {
                Singleton<IEngine>.Instance = oldEngine;
            }
        }

        [Test]
        public void ServerUrl_Returns_DifferentValues_InRequestForDifferentSites()
        {
            var oldEngine = Singleton<IEngine>.Instance;
            try
            {
                var engine = new FakeEngine();
                var webContext = new FakeWebContextWrapper();
                webContext.isWeb = true;
                engine.Container.AddComponentInstance("", typeof(IWebContext), webContext);
                Singleton<IEngine>.Instance = engine;

                webContext.Url = "http://site1/app";
                Assert.That(Url.ServerUrl, Is.EqualTo("http://site1"));
                webContext.Url = "http://site2/app";
                Assert.That(Url.ServerUrl, Is.EqualTo("http://site2"));
            }
            finally
            {
                Singleton<IEngine>.Instance = oldEngine;
            }
        }

        [Test]
        public void Resolve_Uses_DefaultReplacements()
        {
            string result = Url.ResolveTokens("{ManagementUrl}/Resources/Icons/add.png");

            Assert.That(result, Is.EqualTo("/N2/Resources/Icons/add.png"));
        }

        [Test]
        public void Resolve_CanChange_DefaultReplacements()
        {
            string backup = Url.GetToken("{ManagementUrl}");

            try
            {
                Url.SetToken("{ManagementUrl}", "/Manage");

                string result = Url.ResolveTokens("{ManagementUrl}/Resources/Icons/add.png");

                Assert.That(result, Is.EqualTo("/Manage/Resources/Icons/add.png"));
            }
            finally
            {
                Url.SetToken("{ManagementUrl}", backup);
            }
        }

        [Test]
        public void Resolve_CanAdd_Replcement()
        {
            string backup = Url.GetToken("{HelloUrl}");

            try
            {
                Url.SetToken("{HelloUrl}", "/Hello/World");

                string result = Url.ResolveTokens("{HelloUrl}/Resources/Icons/add.png");

                Assert.That(result, Is.EqualTo("/Hello/World/Resources/Icons/add.png"));
            }
            finally
            {
                Url.SetToken("{HelloUrl}", backup);
            }
        }

        [Test]
        public void Resolve_CanClear_Replcement()
        {
            string backup = Url.GetToken("{ManagementUrl}");

            try
            {
                Url.SetToken("{ManagementUrl}", null);

                string result = Url.ResolveTokens("{ManagementUrl}/Resources/Icons/add.png");

                Assert.That(result, Is.EqualTo("{ManagementUrl}/Resources/Icons/add.png"));
            }
            finally
            {
                Url.SetToken("{ManagementUrl}", backup);
            }
        }

        [Test]
        public void Resolve_DoesntReplace_UnknownTokens()
        {
            string result = Url.ResolveTokens("{HelloUrl}/Resources/Icons/add.png");

            Assert.That(result, Is.EqualTo("{HelloUrl}/Resources/Icons/add.png"));
        }

        [Test]
        public void Resolve_MakesPath_ToAbsolute()
        {
            Url.ApplicationPath = "/hello/";
            try
            {
                string result = Url.ResolveTokens("~/N2/Resources/Icons/add.png");

                Assert.That(result, Is.EqualTo("/hello/N2/Resources/Icons/add.png"));
            }
            finally
            {
                Url.ApplicationPath = "/";
            }
        }

        [Test]
        public void RemoveExtension_RemovesAnyExtension()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension();

            Assert.That(result, Is.EqualTo("/hello"));
        }

        [Test]
        public void RemoveExtension_RemovesValidExtension()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension(".aspx");

            Assert.That(result, Is.EqualTo("/hello"));
        }

        [Test]
        public void RemoveExtension_DoesntRemove_InvalidExtension()
        {
            Url url = "/hello.gif";
            string result = url.RemoveExtension(".aspx");

            Assert.That(result, Is.EqualTo("/hello.gif"));
        }

        [Test]
        public void RemoveExtension_DoesntRemove_WhenNoExtension()
        {
            Url url = "/hello.gif";
            string result = url.RemoveExtension("");

            Assert.That(result, Is.EqualTo("/hello.gif"));
        }

        [Test]
        public void RemoveExtension_RemovesValidExtension_WhenDefinedAsFirst()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension(".aspx", ".html");

            Assert.That(result, Is.EqualTo("/hello"));
        }

        [Test]
        public void RemoveExtension_RemovesValidExtension_WhenDefinedAsSecond()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension(".html", ".aspx");

            Assert.That(result, Is.EqualTo("/hello"));
        }

        [Test]
        public void RemoveExtension_RemovesValidExtension_WhenDefinedBeforeEmpty()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension(".aspx", "");

            Assert.That(result, Is.EqualTo("/hello"));
        }

        [Test]
        public void RemoveExtension_RemovesValidExtension_WhenDefinedAfterEmpty()
        {
            Url url = "/hello.aspx";
            string result = url.RemoveExtension("", ".aspx");

            Assert.That(result, Is.EqualTo("/hello"));
        }

        [Test]
        public void AppendEmptySegment_ToUrlWithExtension()
        {
            Url url = "/hello.aspx";
            url = url.AppendSegment("/");

            url.ToString().ShouldBe("/hello.aspx");
        }
    }
}
