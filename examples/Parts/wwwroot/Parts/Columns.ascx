<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Columns.ascx.cs" Inherits="Parts_Columns" %>
<style>
	.columns .column
	{
		float:left;
	}
	.columns .content
	{
		width:70%;
	}
	.columns .secondary
	{
		width:30%;
	}
	.Narrow-Wide .content
	{
		float:right;
	}
</style>
<div class="columns <%= CurrentItem.ColumnMode %>">
	<div class="column content">
		<n2:DroppableZone ZoneName="Content" runat="server" DropPointBackImageUrl="~/edit/img/shading.png" />
	</div>
	<div class="column secondary">
		<n2:DroppableZone ZoneName="Secondary" runat="server" DropPointBackImageUrl="~/edit/img/shading.png" />
	</div>
</div>
<hr style="visibility:hidden;width:100%;clear:both;" />