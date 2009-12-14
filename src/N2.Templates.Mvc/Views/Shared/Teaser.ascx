<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Teaser>" %>
<%= Html.Display(m => m.Title) %>
<div class="box"><div class="inner">
		<a href="<%=Model.LinkUrl%>">
			<%=Html.Display(m => m.LinkText)%>
		</a>
</div></div>