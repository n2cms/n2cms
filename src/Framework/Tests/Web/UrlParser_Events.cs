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
    public class UrlParser_Events : ParserTestsBase
    {
        protected new UrlParser parser;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            base.parser = parser = TestSupport.Setup(persister, wrapper, host);
            CreateDefaultStructure();
        }

        [Test]
        public void BuildUrl_InvokesEvent()
        {
            UrlEventArgs lastEvent = null;
            parser.BuiltUrl += (s, e) => lastEvent = e;

            parser.BuildUrl(page1_1);
    
            lastEvent.AffectedItem.ShouldBe(page1_1);
        }

        [Test]
        public void BuildUrl_EventCanModifyUrl()
        {
            parser.BuiltUrl += (s, e) => e.Url += "?modifiedBy=BuildUrl_EventCanModifyUrl";

            string url = parser.BuildUrl(page1_1);

            url.ShouldEndWith("?modifiedBy=BuildUrl_EventCanModifyUrl");
        }

        [Test]
        public void ResolvePath_NotFound_IsCalled_ForUnknownUrl()
        {
            PageNotFoundEventArgs lastEvent = null;
            parser.PageNotFound += (s, e) => lastEvent = e;

            var path = parser.FindPath("/hello/");
            
            path.IsEmpty().ShouldBe(true);
            lastEvent.Url.ShouldBe("/hello/");
        }

        [Test]
        public void ResolvePath_NotFoundEvent_CanSetItem()
        {
            parser.PageNotFound += (s, e) => e.AffectedPath.CurrentItem = page2;

            var path = parser.FindPath("/hello/");

            path.CurrentItem.ShouldBe(page2);
        }

        [Test]
        public void Parse_NotFound_IsCalled_ForUnknownUrl()
        {
            PageNotFoundEventArgs lastEvent = null;
            parser.PageNotFound += (s, e) => lastEvent = e;

            var item = parser.Parse("/hello/");

            item.ShouldBe(null);
            lastEvent.Url.ShouldBe("/hello/");
        }

        [Test]
        public void Parse_NotFoundEvent_CanSetItem()
        {
            parser.PageNotFound += (s, e) => e.AffectedItem = page2_1;

            var item = parser.Parse("/hello/");

            item.ShouldBe(page2_1);
        }
    }
}
