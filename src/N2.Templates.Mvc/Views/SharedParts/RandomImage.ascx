<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<RandomImage>" %>
<div class="uc">
	<%= Html.DisplayContent(ri => ri.RandomImageUrl)%>
</div>