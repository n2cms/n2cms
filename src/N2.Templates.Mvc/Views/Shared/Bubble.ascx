<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<BubbleItem>" %>

<div class="uc bubble">
	<div class="inner">
		<div class="text">
			<%= CurrentItem.Text %>
		</div>
		<div class="bottom"></div>
	</div>
	<style type="text/css">
		@import "<%=ResolveUrl("~/Content/Css/Faq.css")%>";	
	</style>
</div>