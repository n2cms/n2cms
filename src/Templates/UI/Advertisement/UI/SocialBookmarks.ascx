<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SocialBookmarks.ascx.cs" Inherits="N2.Templates.Advertisement.UI.SocialBookmarks" %>
<%@ Register TagPrefix="adv" Namespace="N2.Templates.Advertisement.UI.WebControls" Assembly="N2.Templates.Advertisement" %>
<%--
<a href="http://blinkbits.com/bookmarklets/save.php?v=1&source_url=XXX&Title=YYY">BlinkBits</a>
<a href="http://www.blinklist.com/index.php?Action=Blink/addblink.php&Url=XXX&Title=YYY">BlinkList</a>
<a href="http://blogmarks.net/my/new.php?mini=1&title=YYY&url=XXX">Blogmarks</a>
<a href="http://buddymarks.com/add_bookmark.php?bookmark_title=YYY&bookmark_url=XXX">Buddymarks</a>
<a href="http://www.citeulike.org/posturl?url=XXX&title=YYY">CiteUlike</a>
<a href="http://www.diigo.com/post?url=XXX&title=YYY">Diigo</a>
<a href="http://myfavorites.earthlink.net/my/add_favorite?v=1&url=XXX&title=YYY" target="_blank">Earthlink</a>
<a href="http://www.feedmarker.com/admin.php?do=bookmarklet_mark&url=XXX&title=YYY;">FeedMarker</a>
<a href="http://www.flogz.com/submit?url=XXX" title="personal finance and investing news website">Flog this!</a>
<a href="http://feedmelinks.com/categorize?from=toolbar&op=submit&name=YYY&url=XXX">feedmelinks</a>
<a href="http://www.furl.net/storeIt.jsp?t=YYY&u=XXX">Furl</a>
<a href="http://www.google.com/bookmarks/mark?op=edit&bkmk=XXX&title=YYY">Google</a>
<a href="http://www.givealink.org/cgi-pub/bookmarklet/bookmarkletLogin.cgi?&uri=XXX&title=YYY">Give a Link</a>
<a href="http://www.gravee.com/account/bookmarkpop?u=XXX&t=YYY">Gravee</a>
<a href="http://www.igooi.com/addnewitem.aspx?self=1&noui=yes&jump=close&url=XXX&title=YYY">igooi</a>
<a href="http://scoop.isedb.com/submit.php?url=XXX&title=YYY">ISEdb</a>
<a href="http://lister.lilisto.com/?t=YYY&l=XXX">Lilisto</a>
<a href="http://www.linkagogo.com/go/AddNoPopup?title=YYY&url=XXX">Linkagogo</a>
<a href="http://linkroll.com/insert.php?url=XXX&title=YYY">Linkroll</a>
<a href="http://api.looklater.com/bookmarks/save?url=XXX&title=YYY">Looklater</a>
<a href="http://ma.gnolia.com/bookmarklet/add?url=XXX&title=YYY">ma.gnolia</a>
<a href="http://www.maple.nu/bookmarks/bookmarklet?bookmark[url]=XXX&bookmark[description]=YYY">Maple.nu</a>
<a href="http://www.marktd.com/submit.php?url=XXX&title=YYY">Marktd</a>
<a href="http://www.mister-wong.de/index.php?action=addurl&bm_url=XXX&bm_description=YYY">Mr. Wong</a>
<a href="http://user.my-tuts.com/tag-tutorial/?title=YYY&url=XXX">My-Tuts</a>
<a href="http://www.netvouz.com/action/submitBookmark?url=XXX&title=YYY&popup=no">Netvouz</a>
<a href="http://www.newsvine.com/_wine/save?popoff=0&u=XXX&h=YYY">Newsvine</a>
<a href="http://nshout.com/submit.php?url=XXX&title=YYY">NShout</a>
<a href="http://www.onlywire.com/b/?u=XXX&t=YYY;">Onlywire</a>
<a href="http://www.plugim.com/submit?url=XXX&title=YYY&trackback=">PlugIM</a>
<a href="http://www.rawsugar.com/pages/tagger.faces?turl=XXX&tttl=YYY">RawSugar</a>
<a href="http://www.dehsoftware.com/recommendzit/submit.php?url=XXX&title=YYY&description=">RecommendzIt</a>
<a href="http://scuttle.org/bookmarks.php/pass?action=add&address=XXX&title=YYY">Scuttle</a>
<a href="http://battellemedia.com/searchmob/submit.php?url=XXX&title=YYY">SearchMob</a>
<a href="http://segnalo.com/post.html.php?url=XXX&title=YYY&description=">Segnalo</a>
<a href="http://www.shadows.com/features/tcr.htm?url=XXX&title=YYY">Shadows</a>
<a href="http://simpy.com/simpy/LinkAdd.do?note=YYY&href=XXX">Simpy</a>
<a href="http://www.sphinn.com/submit.php?url=XXX&title=YYY">Sphinn</a>
<a href="http://www.spurl.net/spurl.php?url=XXX&title=YYY">Spurl</a>
<a href="http://www.squidoo.com/lensmaster/bookmark?XXX">Squidoo</a>
<a href="http://www.stumbleupon.com/submit?url=XXX&title=YYY">StumbleUpon</a>
<a href="http://taggly.com/bookmarks.php/pass?action=add&address=XXX">Taggly</a>
<a href="http://www.tagtooga.com/tapp/db.exe?c=jsEntryForm&b=fx&title=YYY&url=XXX">tagtooga</a>
<a href="http://www.talkdigger.com/index.php?surl=XXX">TalkDigger</a>
<a href="http://tellfriends.com/topics/create?url=XXX">Tellfriends</a>
<a href="http://www.wink.com/_/tag?url=XXX&doctitle=YYY">Wink</a>

<a href="http://del.icio.us/post?url=XXX&title=YYY">del.icio.us</a>
<a href="http://digg.com/submit?phase=2&url=XXX&title=YYY">Digg it</a>
<a href="http://www.dotnetkicks.com/kick/?url=XXX">Dotnetkicks</a>
<a href="http://www.dzone.com/links/add.html?url=XXX&title=YYY">DZone</a>
<a href="http://www.google.com/bookmarks/mark?op=edit&output=popup&bkmk=XXX&title=YYY">Google</a>
<a href="https://favorites.live.com/quickadd.aspx?url=XXX&title=YYY">Live</a>
<a href="http://www.netscape.com/submit/?U=XXX&T=YYY">Netscape</a>
<a href="http://reddit.com/submit?url=XXX&title=YYY">Reddit</a>
<a href="http://slashdot.org/bookmark.pl?url=XXX&title=YYY">Slashdot</a>
<a href="http://technorati.com/faves?sub=addfavbtn&add=XXX">Technorati</a>
<a href="http://myweb2.search.yahoo.com/myresults/bookmarklet?t=YYY&u=XXX">Yahoo MyWeb</a>
--%>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box runat="server">
	<adv:BookmarkList BookmarkUrl="<%$ Code: BookmarkUrl %>" BookmarkText="<%$ Code: BookmarkTitle %>" ShowText="<%$ CurrentItem: ShowText %>" ImageFolder="~/Advertisement/UI/Img/" runat="server">
		<adv:Bookmark UrlFormat="http://del.icio.us/post?url={0}&title={1}" Image="delicious.png" Text="del.icio.us" runat="server" />
		<adv:Bookmark UrlFormat="http://digg.com/submit?phase=2&url={0}&title={1}" Image="digg.png" Text="Digg it" runat="server" />
		<adv:Bookmark UrlFormat="http://www.dotnetkicks.com/kick/?url={0}" Image="dotnetkicks.png" Text="Dotnetkicks" runat="server" />
		<adv:Bookmark UrlFormat="http://www.dzone.com/links/add.html?url={0}&title={1}" Image="dzone.png" Text="DZone" runat="server" />
		<adv:Bookmark UrlFormat="http://www.google.com/bookmarks/mark?op=edit&output=popup&bkmk={0}&title={1}" Image="google.png" Text="Google" runat="server" />
		<adv:Bookmark UrlFormat="https://favorites.live.com/quickadd.aspx?url={0}&title={1}" Image="live.png" Text="Live" runat="server" />
		<adv:Bookmark UrlFormat="http://www.netscape.com/submit/?U={0}&T={1}" Image="netscape.gif" Text="Netscape" runat="server" />
		<adv:Bookmark UrlFormat="http://reddit.com/submit?url={0}&title={1}" Image="reddit.png" Text="Reddit" runat="server" />
		<adv:Bookmark UrlFormat="http://slashdot.org/bookmark.pl?url={0}&title={1}" Image="slashdot.png" Text="Slashdot" runat="server" />
		<adv:Bookmark UrlFormat="http://technorati.com/faves?sub=addfavbtn&add={0}" Image="technorati.png" Text="Technorati" runat="server" />
		<adv:Bookmark UrlFormat="http://myweb2.search.yahoo.com/myresults/bookmarklet?t={1}&u={0}" Image="yahoomyweb.png" Text="Yahoo MyWeb" runat="server" />
	</adv:BookmarkList>
</n2:Box>