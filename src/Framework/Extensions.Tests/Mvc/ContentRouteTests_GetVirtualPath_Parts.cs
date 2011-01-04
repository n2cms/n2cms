using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using N2.Configuration;
using N2.Definitions;
using N2.Engine;
using N2.Extensions.Tests.Fakes;
using N2.Extensions.Tests.Mvc.Controllers;
using N2.Extensions.Tests.Mvc.Models;
using N2.Persistence.NH;
using N2.Tests;
using N2.Web;
using N2.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using HtmlHelper = System.Web.Mvc.HtmlHelper;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace N2.Extensions.Tests.Mvc
{
	[TestFixture]
	public class ContentRouteTests_GetVirtualPath_Parts : ContentRouteTests
	{
		[Test]
		public void GetVirtualPath_ToPartDefaultAction()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			RequestingUrl("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem?page=2&part=10"));
		}

		[Test]
		public void GetVirtualPath_ToPartDefaultAction_ViaPart_IsNotSupported()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			RequestingUrl("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { part = part }));

			Assert.That(vpd.VirtualPath, Is.Not.EqualTo("TestItem?page=2&part=10"));
		}

		[Test]
		public void GetVirtualPath_ToPart_OtherActionAction()
		{
			var part = CreateOneItem<TestItem>(10, "whatever", about);
			RequestingUrl("/about/");

			var vpd = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { item = part, action = "doit" }));

			Assert.That(vpd.VirtualPath, Is.EqualTo("TestItem/doit?page=2&part=10"));
		}

		[Test]
		public void CanCreate_ActionLink_ToPart()
		{
			var item = CreateOneItem<TestItem>(10, "whatever", root);
			RequestingUrl("/?part=10");

			var html = htmlHelper.ActionLink(/*text*/"Hello", /*action*/"Submit", new { q = "something", controller = "TestItem" });

			Assert.That(html.ToString(), Is.EqualTo("<a href=\"/TestItem/Submit?q=something&amp;page=1&amp;part=10\">Hello</a>"));
		}
	}
}
