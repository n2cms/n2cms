<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<BubbleItem>" %>
<div class="uc bubble">
	<div class="inner">
		<div class="text">
			<%= Html.DisplayContent(m => m.Text) %>
		</div>
		<div class="bottom"></div>
	</div>
</div>