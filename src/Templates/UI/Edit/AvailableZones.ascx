<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AvailableZones.ascx.cs" Inherits="N2.Edit.AvailableZones" meta:resourceKey="AvailableZonesResource" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<edit:FieldSet Legend="Zones" runat="server" class="zonesBox" meta:resourceKey="fsZones" >
	<asp:Repeater ID="rptZones" runat="server">
		<HeaderTemplate><dl></HeaderTemplate>
		<ItemTemplate>
			<dt>
				<asp:HyperLink ID="hlNew" meta:resourceKey="hlNew" runat="server" ToolTip="new item" NavigateUrl="<%# GetNewDataItemUrl(Container.DataItem) %>" Text="<%# GetNewDataItemText(Container.DataItem) %>" />
				<strong><%# GetZoneString((string)Eval("ZoneName")) ?? Eval("Title") %></strong>
			</dt>
			<asp:Repeater ID="rptItems" runat="server" DataSource="<%# GetItemsInZone(Container.DataItem) %>">
				<HeaderTemplate><dd class="items"></HeaderTemplate>
				<ItemTemplate>
					<div class="edit">
						<asp:HyperLink runat="server" 
							Text="<%# GetEditDataItemText(Container.DataItem) %>" 
							NavigateUrl="<%# GetEditDataItemUrl(Container.DataItem) %>" />
					</div>
				</ItemTemplate>
				<FooterTemplate></dd></FooterTemplate>
			</asp:Repeater>
		</ItemTemplate>
		<FooterTemplate></dl></FooterTemplate>
	</asp:Repeater>
</edit:FieldSet>