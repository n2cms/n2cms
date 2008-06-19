<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Panel.ascx.cs" Inherits="Parts_Panel" %>
<div style="padding:10px;background-color:<%= CurrentItem.BackColor %>">
	<n2:DroppableZone ZoneName="Content" runat="server" DropPointBackImageUrl="~/edit/img/shading.png" />
</div>