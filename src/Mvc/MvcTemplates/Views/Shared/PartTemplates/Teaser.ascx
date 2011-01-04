<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Teaser>" %>
<div class="uc">
	<%= Html.DisplayContent(m => m.Title) %>
	<div class="box"><div class="inner">
		<a href="<%=Model.LinkUrl%>">
			<%=Html.DisplayContent(m => m.LinkText)%>
		</a>
	</div></div>
</div>