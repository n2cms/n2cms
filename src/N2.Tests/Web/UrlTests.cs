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
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&amp;key=value"));
        }
        [Test]
        public void CanAppend_KeyValuePair_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.AppendQuery("key=value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&amp;key=value"));
        }

        [Test]
        public void CanSet_KeyAndValue_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.SetQuery("key", "value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }

        [Test]
        public void CanSet_KeyValuePair_ToEmptyQueryString()
        {
            Url u = "/";
            u = u.SetQuery("key=value");
            Assert.That(u.Query, Is.EqualTo("key=value"));
        }

        [Test]
        public void CanSet_KeyAndValue_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.SetQuery("key", "value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&amp;key=value"));
        }
        [Test]
        public void CanSet_KeyValuePair_ToExistingQueryString()
        {
            Url u = "/?somekey=somevalue";
            u = u.SetQuery("key=value");
            Assert.That(u.Query, Is.EqualTo("somekey=somevalue&amp;key=value"));
        }

        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyAndValue()
        {
            Url u = "/?key=somevalue";
            u = u.SetQuery("key", "someothervalue");
            Assert.That(u.Query, Is.EqualTo("key=someothervalue"));
        }
        [Test]
        public void CanReplaceValue_OnExistingQueryString_UsingKeyValuePair()
        {
            Url u = "/?key=somevalue";
            u = u.SetQuery("key=someothervalue");
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
            u = u.SetQuery("key", "cristian & maria");
            Assert.That(u.Query, Is.EqualTo("key=cristian+%26+maria"));
        }
    }
}
