using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web;
using NUnit.Framework.SyntaxHelpers;

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
        public void CanConstruct_StrangeHomePath()
        {
            Url u = new Url("");
            Assert.That(u.Path, Is.EqualTo("/"));
            Assert.That(u.ToString(), Is.EqualTo("/"));
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
            u = u.UpdateQuery("key", "value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }

        [Test]
        public void CanSet_KeyValuePair_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.UpdateQuery("key=value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }

        [Test]
        public void CanSet_KeyAndValue_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.UpdateQuery("key", "value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&key=value"));
        }
        [Test]
        public void CanSet_KeyValuePair_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.UpdateQuery("key=value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&key=value"));
        }

        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyAndValue()
        {
            Url u = "/?key=somevalue";
            u = u.UpdateQuery("key", "someothervalue");
            Assert.That(u.Query, Is.EqualTo("key=someothervalue"));
        }
        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyValuePair()
        {
            Url u = "/?key=somevalue";
            u = u.UpdateQuery("key=someothervalue");
            Assert.That(u.Query, Is.EqualTo("key=someothervalue"));
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
            u = u.UpdateQuery("key", "cristian & maria");
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
        public void CanAppendSegment_ToEmptyPath2()
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
            Url u = "/UI/500.aspx?aspxerrorpath=/default.aspx";
            Assert.That(u.Path, Is.EqualTo("/UI/500.aspx"));
            Assert.That(u.Query, Is.EqualTo("aspxerrorpath=/default.aspx"));
        }
    }
}
