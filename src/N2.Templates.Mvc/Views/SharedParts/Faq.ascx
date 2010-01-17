<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Faq>" %>

<div class="a1">
	<%=Html.Display(m => m.Title)%>
	<%=Html.Display(m => m.Answer)%>
</div>