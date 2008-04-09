<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SocialBookmarks.ascx.cs" Inherits="N2.Templates.Advertisement.UI.SocialBookmarks" %>
<%@ Register TagPrefix="adv" Namespace="N2.Templates.Advertisement.UI.WebControls" Assembly="N2.Templates.Advertisement" %>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box runat="server">
	<adv:BookmarkList BookmarkUrl="<%$ Code: BookmarkUrl %>" BookmarkText="<%$ Code: BookmarkTitle %>" ShowText="<%$ CurrentItem: ShowText %>" ImageFolder="~/Advertisement/UI/Img/" runat="server">
		<adv:Bookmark UrlFormat="http://del.icio.us/post?url={0}&amp;title={1}" Image="delicious.png" Text="del.icio.us" runat="server" />
		<adv:Bookmark UrlFormat="http://digg.com/submit?phase=2&amp;url={0}&amp;title={1}" Image="digg.png" Text="Digg it" runat="server" />
		<adv:Bookmark UrlFormat="http://www.dotnetkicks.com/kick/?url={0}" Image="dotnetkicks.png" Text="Dotnetkicks" runat="server" />
		<adv:Bookmark UrlFormat="http://www.dzone.com/links/add.html?url={0}&amp;title={1}" Image="dzone.png" Text="DZone" runat="server" />
		<adv:Bookmark UrlFormat="http://www.google.com/bookmarks/mark?op=edit&amp;output=popup&amp;bkmk={0}&amp;title={1}" Image="google.png" Text="Google" runat="server" />
		<adv:Bookmark UrlFormat="https://favorites.live.com/quickadd.aspx?url={0}&amp;title={1}" Image="live.png" Text="Live" runat="server" />
		<adv:Bookmark UrlFormat="http://www.netscape.com/submit/?U={0}&amp;T={1}" Image="netscape.gif" Text="Netscape" runat="server" />
		<adv:Bookmark UrlFormat="http://reddit.com/submit?url={0}&amp;title={1}" Image="reddit.png" Text="Reddit" runat="server" />
		<adv:Bookmark UrlFormat="http://slashdot.org/bookmark.pl?url={0}&amp;title={1}" Image="slashdot.png" Text="Slashdot" runat="server" />
		<adv:Bookmark UrlFormat="http://technorati.com/faves?sub=addfavbtn&amp;add={0}" Image="technorati.png" Text="Technorati" runat="server" />
		<adv:Bookmark UrlFormat="http://myweb2.search.yahoo.com/myresults/bookmarklet?t={1}&amp;u={0}" Image="yahoomyweb.png" Text="Yahoo MyWeb" runat="server" />
	</adv:BookmarkList>
</n2:Box>