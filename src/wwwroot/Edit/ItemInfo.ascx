<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemInfo.ascx.cs" Inherits="N2.Edit.ItemInfo" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<edit:FieldSet Legend="Info" runat="server" class="infoBox" meta:resourceKey="fsInfo">
	<edit:InfoLabel Label="Type"		Text="<%# CurrentDefinition.Title %>" runat="server" meta:resourceKey="ilType"/>
	<edit:InfoLabel Label="Published"	Text="<%# CurrentItem.Published %>" runat="server" meta:resourceKey="ilPublished"/>
	<edit:InfoLabel Label="Expires"		Text="<%# CurrentItem.Expires %>" runat="server" meta:resourceKey="ilExpires"/>
</edit:FieldSet>