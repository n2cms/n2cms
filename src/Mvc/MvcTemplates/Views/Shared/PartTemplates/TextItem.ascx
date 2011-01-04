<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<TextItem>" %>
<div class="uc">
	<%= Html.DisplayContent(m => m.Title) %>
	<%= Html.DisplayContent(m => m.Text) %>
</div>