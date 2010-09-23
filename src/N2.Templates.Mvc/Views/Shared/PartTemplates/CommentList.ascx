<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<CommentList>" %>
<div class="uc">
	<div class="list">
		<% Html.RenderZone("Comments"); %>
	</div>
</div>