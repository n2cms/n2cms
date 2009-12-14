<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<BubbleItem>" %>

<div class="uc bubble">
	<div class="inner">
		<div class="text">
			<%= Html.Display(m => m.Text) %>
		</div>
		<div class="bottom"></div>
	</div>
	<style type="text/css">
		@import "<%=ResolveUrl("~/Content/Css/Faq.css")%>";	
	</style>
</div>