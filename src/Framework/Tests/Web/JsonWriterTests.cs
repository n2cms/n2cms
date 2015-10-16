using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web;
using Shouldly;
using System.Web.Script.Serialization;

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

			result.ShouldBe("\"2010-06-16T08:00:00.000Z\"");
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
        public void CircularGraph_Parent()
        {
            var parent = new CI { ID = 1, Title = "parent" };
            var child = new CI { ID = 2, Title = "child" };
            child.AddTo(parent);

            var result = parent.ToJson();
            var deserialized = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);

            deserialized["Title"].ShouldBe("parent");
            deserialized.ContainsKey("Children").ShouldBe(false);
        }

        [Test]
        public void CircularGraph_Child()
        {
            var parent = new CI { ID = 1, Title = "parent" };
            var child = new CI { ID = 2, Title = "child" };
            child.AddTo(parent);

            var result = child.ToJson();
            var deserialized = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);

            deserialized["Title"].ShouldBe("child");
            deserialized["Parent"].ShouldBe(1);
        }

        [Test]
        public void Properties()
        {
            var time = new DateTime(2013, 03, 27, 10, 00, 00);
            //time.Subtract(new DateTime(1970, 01, 01, 0, 0, 0)).TotalSeconds
            var item = new CI { AlteredPermissions = N2.Security.Permission.ReadWrite, AncestralTrail = "/1/", ChildState = N2.Collections.CollectionState.ContainsPublicParts, Created = time, Expires = time, ID = 2, Name = "hello-world", Published = time, SavedBy = "theboyz", SortOrder = 666, State = ContentState.Waiting, TemplateKey = "1234", Title = "Hello World", TranslationKey = 5678, VersionIndex = 222, Visible = false, ZoneName = "HelloZone", Updated = time };

            var result = item.ToJson();

            //var jsDatetime = new JavaScriptSerializer().Serialize(time);
            var deserialized = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
            deserialized["Title"].ShouldBe("Hello World");
            deserialized["AlteredPermissions"].ShouldBe((int)N2.Security.Permission.ReadWrite);
            deserialized["AncestralTrail"].ShouldBe("/1/");
            deserialized["ChildState"].ShouldBe((int)N2.Collections.CollectionState.ContainsPublicParts);
            //deserialized["Created"].ShouldBe(jsDatetime);
            //deserialized["Expires"].ShouldBe(jsDatetime);
            deserialized["ID"].ShouldBe(2);
            deserialized["Name"].ShouldBe("hello-world");
            //deserialized["Published"].ShouldBe(time);
            deserialized["SavedBy"].ShouldBe("theboyz");
            deserialized["SortOrder"].ShouldBe(666);
            deserialized["State"].ShouldBe((int)ContentState.Waiting);
            deserialized["TemplateKey"].ShouldBe("1234");
            deserialized["Title"].ShouldBe("Hello World");
            deserialized["TranslationKey"].ShouldBe(5678);
            deserialized["VersionIndex"].ShouldBe(222);
            deserialized["Visible"].ShouldBe(false);
            deserialized["ZoneName"].ShouldBe("HelloZone");
            deserialized["Updated"].ShouldBe("2013-03-27T09:00:00.000Z");
        }

        [Test]
        public void Details()
        {
            var item = new CI();
            item["Hello"] = "World";

            var result = item.ToJson();

            var deserialized = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
            deserialized["Hello"].ShouldBe("World");
        }

        [Test]
        public void Details_Link()
        {
            var first = new CI { ID = 1, Title = "first" };
            var second = new CI { ID = 2, Title = "second" };

            first["Hello"] = second;

            var result = first.ToJson();

            var deserialized = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
            deserialized["Hello"].ShouldBe(2);
        }

        [Test]
        public void DetailCollections()
        {
            var item = new CI();
            item.GetDetailCollection("Hello", true).Add("World");

            var result = item.ToJson();

            var deserialized = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
            deserialized["Hello"].ShouldBe(new[] { "World" });
        }

        [Test]
        public void DetailCollections_Link()
        {

            var first = new CI { ID = 1, Title = "first" };
            var second = new CI { ID = 2, Title = "second" };

            first.GetDetailCollection("Hello", true).Add(second);

            var result = first.ToJson();

            var deserialized = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
            deserialized["Hello"].ShouldBe(new[] { 2 });
        }
    }
}
