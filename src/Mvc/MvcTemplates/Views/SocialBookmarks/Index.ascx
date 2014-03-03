<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.ContentViewUserControl<SocialBookmarksModel, SocialBookmarks>" %>

<%= ContentHtml.DisplayContent(m => m.Title)%>

<div class="uc">
	<div class="box">
		<div class="inner">
		<%
			string BookmarkUrl = Server.UrlEncode("http://" + Request.Url.Authority + Model.CurrentItem.Url);
			string BookmarkTitle = Server.UrlEncode(Model.CurrentItem.Title);
		%>
		<%foreach(var bookmark in Model.Bookmarks){%>
			<a href="<%=String.Format(bookmark.UrlFormat, BookmarkUrl, BookmarkTitle)%>">
				<img src="<%=ResolveUrl("~/Content/Img/" + bookmark.ImageName) %>"
					alt="<%=bookmark.Text%>" />
				<%=Model.CurrentItem.ShowText ? bookmark.Text : null%>
			</a>
		<%}%>
		</div>
	</div>
</div>