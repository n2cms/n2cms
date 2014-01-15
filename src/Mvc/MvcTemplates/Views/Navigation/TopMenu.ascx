<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<TopMenuModel>" %>

<ul class="topMenu menu">
	<%foreach(var item in Model.MenuItems){%><li class="<%=item.Url == Model.CurrentItem.Url ? "current" : "" %>"><a href="<%=ResolveUrl(item.Url)%>"><%=item.Title%></a></li><%}%>
</ul>

<div class="languageMenu">
	<%foreach(var item in Model.Translations){%>
	<a href="<%= item.Page.Url %>" class="language<%= item.Page == Model.CurrentItem ? " current" : "" %>" title="<%= item.Language.LanguageTitle %>">
		<span class="<%= item.Language.LanguageCode.Split('-').LastOrDefault().ToLower() %> sprite"></span>
	</a>
	<%}%>
</div>