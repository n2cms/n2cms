using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web;
using Shouldly;

namespace N2.Tests.Web
{
    [TestFixture]
    public class JsonWriterTests
    {
        [Test]
        public void String()
        {
            var result = "hello".ToJson();

            result.ShouldBe("\"hello\"");
        }

        [Test]
        public void String_SingleQuote()
        {
            var result = "he'llo".ToJson();

            result.ShouldBe("\"he'llo\"");
        }

        [Test]
        public void String_DoubleQuote()
        {
            var result = "he\"llo".ToJson();

            result.ShouldBe("\"he\\\"llo\"");
        }

        [Test]
        public void String_Newline()
        {
            var result = "he\r\nllo".ToJson();

            result.ShouldBe("\"he\\r\\nllo\"");
        }

        [Test]
        public void String_Ampersand()
        {
            var result = "hello & world".ToJson();

            result.ShouldBe("\"hello & world\"");
        }

        [Test]
        public void String_CartoonSwearing()
        {
            var text = "½!¤%&/()=?`´@£$€{[]}åÅ^¨ÖöÄä;,:.<>-_ ";
            var result = text.ToJson();

            result.ShouldBe("\"" + text + "\"");
        }

        [Test]
        public void Object()
        {
            var result = new { hello = "world" }.ToJson();

            result.ShouldBe("{\"hello\":\"world\"}");
        }

        [Test]
        public void Object_With2Properties()
        {
            var result = new { hello = "world", world = "domination" }.ToJson();

            result.ShouldBe("{\"hello\":\"world\", \"world\":\"domination\"}");
        }

        [Test]
        public void Object_WithSubObject()
        {
            var result = new { hello = new { world = "hello" } }.ToJson();

            result.ShouldBe("{\"hello\":{\"world\":\"hello\"}}");
        }

        [Test]
        public void Object_WithSubArray()
        {
            var result = new { hello = new [] { "hello", "world" } }.ToJson();

            result.ShouldBe("{\"hello\":[\"hello\", \"world\"]}");
        }

        [Test]
        public void Dictionary()
        {
            var result = new Dictionary<string, object> { { "hello", "world" } }.ToJson();

            result.ShouldBe("{\"hello\":\"world\"}");
        }

        [Test]
        public void Hashtable()
        {
            var result = new System.Collections.Hashtable { { "hello", "world" } }.ToJson();

            result.ShouldBe("{\"hello\":\"world\"}");
        }

        [Test]
        public void NameValueCollection()
        {
            var result = new System.Collections.Specialized.NameValueCollection { { "hello", "world" } }.ToJson();

            result.ShouldBe("{\"hello\":\"world\"}");
        }

        [Test]
        public void Array()
        {
            var result = new [] { "hello", "world" }.ToJson();

            result.ShouldBe("[\"hello\", \"world\"]");
        }

        [Test]
        public void Array_WithObjects()
        {
            var result = new object[] { new { world = "hello"}, new { hello = "world"} }.ToJson();

            result.ShouldBe("[{\"world\":\"hello\"}, {\"hello\":\"world\"}]");
        }

        [Test]
        public void List()
        {
            var result = new List<string> { "hello", "world" }.ToJson();

            result.ShouldBe("[\"hello\", \"world\"]");
        }

        [Test]
        public void Int()
        {
            var result = 1.ToJson();

            result.ShouldBe("1");
        }

        [Test]
        public void Boolean()
        {
            var result = false.ToJson();

            result.ShouldBe("false");
        }

        [Test]
        public void Double()
        {
            var result = 1.2.ToJson();

            result.ShouldBe("1.2");
        }

        [Test]
        public void DateTime()
        {
            var date = new DateTime(2010, 06, 16, 10, 00, 00);
            var result = date.ToJson();
            var milisecondsSince1970 = date.Subtract(new DateTime(1970, 01, 01)).TotalMilliseconds;

            result.ShouldBe("\"\\/Date(" + milisecondsSince1970 + ")\\/\"");
        }

        [Test]
        public void Uri()
        {
            var result = new Uri("http://n2cms.com/").ToJson();

            result.ShouldBe("\"http://n2cms.com/\"");
        }

        class CI : ContentItem
        {
        }

        [Test]
        public void CircularGraph()
        {
            var parent = new CI { Title = "parent" };
            var child = new CI { Title = "child" };
            child.AddTo(parent);

            var result = parent.ToJson();

            result.ShouldContain("\"Title\":\"parent\"");
            result.ShouldContain("\"Title\":\"child\"");
            result.ShouldContain("\"Parent\":null");
            result.ShouldNotContain("\"Parent\":{");
            result.ShouldContain("\"Children\":[{");
        }
    }
}
