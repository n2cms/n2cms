<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemInfo.ascx.cs" Inherits="N2.Edit.ItemInfo" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<edit:FieldSet Legend="Info" runat="server" class="infoBox" meta:resourceKey="fsInfo">
	<edit:InfoLabel Label="ID"	        Text="<%# CurrentItem.ID %>" runat="server" meta:resourceKey="ilID"/>
	<edit:InfoLabel Label="Type"		Text="<%# CurrentDefinition.Title %>" runat="server" meta:resourceKey="ilType"/>
	<edit:InfoLabel Label="Version"		Text="<%# CurrentItem.VersionIndex + 1 %>" runat="server" meta:resourceKey="ilVersion"/>
	<edit:InfoLabel Label="State"		Text="<%# GetLocalResourceObject(CurrentItem.State.ToString()) ?? CurrentItem.State.ToString() %>" runat="server" meta:resourceKey="ilState"/>
	<edit:InfoLabel Label="Published"	Text="<%# CurrentItem.Published %>" runat="server" meta:resourceKey="ilPublished"/>
	<edit:InfoLabel Label="Expires"		Text="<%# CurrentItem.Expires %>" runat="server" meta:resourceKey="ilExpires"/>
</edit:FieldSet>