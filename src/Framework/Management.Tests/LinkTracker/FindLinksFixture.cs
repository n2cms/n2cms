using System.Collections.Generic;
using N2.Edit.LinkTracker;
using NUnit.Framework;

namespace N2.Edit.Tests.LinkTracker
{
	[TestFixture]
	public class FindLinksFixture
	{
		public static string LinkToStartPage = @"<a href=""/"">startsidan</a>";
		public static string LinkToStartPageWithSingleQuotes = @"<a href='/'>startsidan</a>";
		public static string LinkWithTitle = @"<a title=""just a link"" href=""/test.aspx"">test page</a>";
		public static string LinkRelativeToCurrentDirectory = @"<a href=""right_here.aspx"">page next to here</a>";
		public static string LinkWithSpacesInUrl = @"<a href=""/my pages/right there.aspx"">a page over there</a>";
		public static string LinkWithABunchOfOtherLinks = @"<a href=""/my pages/right here.aspx"">startsidan</a> lorem ipsum
<br/><a href=""/page.aspx"">page</a> fasdasfad <a href='/yess.aspx'>no</a><a href=""/no.aspx"">yes</a><div><a href=""/"">home</a></div>
";
		public static string LinkWithStrangeCharacters = @"<a href=""/Documentation/Content-enabling%20an%20existing%20site.aspx#two"">hello</a>";

		Tracker linkFactory;

		[SetUp]
		public void SetUp()
		{
			linkFactory = new Tracker(null, null, null, null);
		}

		[Test]
		public void FindSingleLinkInHtml()
		{
			string html = LinkToStartPage;
			IList<string> results = linkFactory.FindLinks(html);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("/", results[0]);
		}

		[Test]
		public void FindLinkWithSingleQuotes()
		{
			string html = LinkToStartPageWithSingleQuotes;
			IList<string> results = linkFactory.FindLinks(html);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("/", results[0]);
		}

		[Test]
		public void FindLinkWithTitle()
		{
			string html = LinkWithTitle;
			IList<string> results = linkFactory.FindLinks(html);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("/test.aspx", results[0]);
		}

		[Test]
		public void FindRelativeLink()
		{
			string html = LinkRelativeToCurrentDirectory;
			IList<string> results = linkFactory.FindLinks(html);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("right_here.aspx", results[0]);
		}

		[Test]
		public void FindLinkWithSpaces()
		{
			string html = LinkWithSpacesInUrl;
			IList<string> results = linkFactory.FindLinks(html);
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("/my pages/right there.aspx", results[0]);
		}

		[Test]
		public void FindMultipleLinks()
		{
			string html = LinkWithABunchOfOtherLinks;
			IList<string> results = linkFactory.FindLinks(html);
			Assert.AreEqual(5, results.Count);
		}

		[Test]
		public void FindLinkWithEscapedWhitespaceAndHash()
		{
			IList<string> results = linkFactory.FindLinks(LinkWithStrangeCharacters);
			string expected = "/Documentation/Content-enabling%20an%20existing%20site.aspx#two";
			Assert.AreEqual(expected, results[0]);
		}
	}
}