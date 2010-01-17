<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ModelViewUserControl<SocialBookmarksModel, SocialBookmarks>" %>
<script runat="server">
	protected string BookmarkUrl
	{
		get { return Server.UrlEncode("http://" + Request.Url.Authority + Model.CurrentItem.Url); }
	}

	protected string BookmarkTitle
	{
		get { return Server.UrlEncode(Model.CurrentItem.Title); }
	}
</script>

<%= ContentHtml.Display(m => m.Title)%>

<div class="box">
	<div class="inner">
	<%foreach(var bookmark in Model.Bookmarks){%>
		<a href="<%=String.Format(bookmark.UrlFormat, BookmarkUrl, BookmarkTitle)%>">
			<img src="<%=ResolveUrl("~/Content/Img/" + bookmark.ImageName) %>"
				alt="<%=bookmark.Text%>" />
			<%=Model.CurrentItem.ShowText ? bookmark.Text : null%>
		</a>
	<%}%>
	</div>
</div>