<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SocialBookmarks.ascx.cs" Inherits="N2.Templates.Advertisement.UI.SocialBookmarks" %>
<script runat="server">
	public class Bookmark
	{
		private string urlFormat;
		private string imageName;
		private string text;

		public Bookmark(string urlFormat, string imageName, string text)
		{
			this.urlFormat = urlFormat;
			this.imageName = imageName;
			this.text = text;
		}

		public string UrlFormat
		{
			get { return urlFormat; }
			set { urlFormat = value; }
		}

		public string ImageName
		{
			get { return imageName; }
			set { imageName = value; }
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}
	}

	protected Bookmark[] bookmarks = new Bookmark[]{
		new Bookmark("http://del.icio.us/post?url={0}&amp;title={1}", "delicious.png", "del.icio.us"),
		new Bookmark("http://digg.com/submit?phase=2&amp;url={0}&amp;title={1}", "digg.png", "Digg it"),
		new Bookmark("http://www.dotnetkicks.com/kick/?url={0}", "dotnetkicks.png", "Dotnetkicks"),
		new Bookmark("http://www.dzone.com/links/add.html?url={0}&amp;title={1}", "dzone.png", "DZone"),
		new Bookmark("http://www.google.com/bookmarks/mark?op=edit&amp;output=popup&amp;bkmk={0}&amp;title={1}", "google.png", "Google"),
		new Bookmark("https://favorites.live.com/quickadd.aspx?url={0}&amp;title={1}", "live.png", "Live"),
		new Bookmark("http://www.netscape.com/submit/?U={0}&amp;T={1}", "netscape.gif", "Netscape"),
		new Bookmark("http://reddit.com/submit?url={0}&amp;title={1}", "reddit.png", "Reddit"),
		new Bookmark("http://slashdot.org/bookmark.pl?url={0}&amp;title={1}", "slashdot.png", "Slashdot"),
		new Bookmark("http://technorati.com/faves?sub=addfavbtn&amp;add={0}", "technorati.png", "Technorati"),
		new Bookmark("http://myweb2.search.yahoo.com/myresults/bookmarklet?t={1}&amp;u={0}", "yahoomyweb.png", "Yahoo MyWeb")
	};
</script>

<n2:EditableDisplay runat="server" PropertyName="Title" />

<n2:Box runat="server">
	<asp:Repeater runat="server" DataSource="<%# bookmarks %>">
		<ItemTemplate>
			<a href="<%# string.Format((string)Eval("UrlFormat"), BookmarkUrl, BookmarkTitle) %>">
				<img src="<%# N2.Utility.ToAbsolute("~/Advertisement/UI/Img/" + Eval("ImageName")) %>" alt="<%# Eval("Text") %>" />
				<%# CurrentItem.ShowText ? Eval("Text") : null %>
			</a>
		</ItemTemplate>
	</asp:Repeater>
</n2:Box>