<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Faq>" %>
<div class="uc">
	<div class="a1">
		<%=Html.DisplayContent(m => m.Title)%>
		<%=Html.DisplayContent(m => m.Answer)%>
	</div>
</div>