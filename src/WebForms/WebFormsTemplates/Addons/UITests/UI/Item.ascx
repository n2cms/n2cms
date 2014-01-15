<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Item.ascx.cs" Inherits="N2.Addons.UITests.UI.Item" %>
<fieldset style="padding:10px;margin:10px 0">
	<legend><%= CurrentItem.TypeName%></legend>
	<n2:DroppableZone ZoneName="<%$ CurrentItem: TypeName %>" runat="server" />
</fieldset>