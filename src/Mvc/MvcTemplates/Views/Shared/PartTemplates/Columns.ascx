<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Columns>" %>
<div class="uc columns cf">
	<div class="left">
		<% Html.DroppableZone(Zones.ColumnLeft).AllowExternalManipulation().Render();%>
	</div>
	<div class="right">
		<% Html.DroppableZone(Zones.ColumnRight).AllowExternalManipulation().Render();%>
	</div>
</div>