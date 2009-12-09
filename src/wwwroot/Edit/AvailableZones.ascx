<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AvailableZones.ascx.cs" Inherits="N2.Edit.AvailableZones" meta:resourceKey="AvailableZonesResource" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<edit:FieldSet Legend="Zones" runat="server" class="zonesBox" meta:resourceKey="fsZones" >
	<asp:Repeater ID="rptZones" runat="server">
		<HeaderTemplate><dl></HeaderTemplate>
		<ItemTemplate>
			<dt>
				<asp:HyperLink CssClass="new" ID="hlNew" runat="server" ToolTip="New item" NavigateUrl="<%# GetNewDataItemUrl(Container.DataItem) %>"
				    ImageUrl="img/ico/png/add.png" Text="<%# GetNewDataItemText(Container.DataItem) %>" meta:resourceKey="hlNew" />
				<strong><%# GetZoneString((string)Eval("ZoneName")) ?? Eval("Title") %></strong>
			</dt>
			<asp:Repeater ID="rptItems" runat="server" DataSource="<%# GetItemsInZone(Container.DataItem) %>">
				<HeaderTemplate><dd class="items"></HeaderTemplate>
				<ItemTemplate>
					<div class="edit">
						<asp:HyperLink runat="server" Text="<%# GetEditDataItemText(Container.DataItem) %>" 
						    NavigateUrl="<%# GetEditDataItemUrl(Container.DataItem) %>" />
						<asp:ImageButton runat="server" CommandArgument="<%#GetEditDataItemID(Container.DataItem)%>" 
							Enabled="<%#CanMoveItemDown(Container.DataItem) %>"
							CssClass="<%#MoveItemDownClass(Container.DataItem)%>"
							ImageUrl="~/Edit/img/Ico/png/bullet_arrow_down.png" OnClick="MoveItemDown"	/>
						<asp:ImageButton runat="server" CommandArgument="<%#GetEditDataItemID(Container.DataItem)%>"
							Enabled="<%#CanMoveItemUp(Container.DataItem) %>"
							CssClass="<%#MoveItemUpClass(Container.DataItem)%>"
							ImageUrl="~/Edit/img/Ico/png/bullet_arrow_up.png" OnClick="MoveItemUp"/>
					    <asp:HyperLink NavigateUrl="<%# GetDeleteDataItemUrl(Container.DataItem) %>" CssClass="delete" runat="server" 
					        ImageUrl="img/ico/png/cross.png" meta:resourceKey="hlDelete" />
					</div>
				</ItemTemplate>
				<FooterTemplate></dd></FooterTemplate>
			</asp:Repeater>
		</ItemTemplate>
		<FooterTemplate></dl></FooterTemplate>
	</asp:Repeater>
</edit:FieldSet>
