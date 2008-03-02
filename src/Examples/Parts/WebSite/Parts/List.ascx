<%@ Control Language="C#" AutoEventWireup="true" CodeFile="List.ascx.cs" Inherits="Parts_List" %>
<n2:ItemDataSource id="children" runat="server" CurrentItem="<%$ CurrentItem: ListSubpagesOf %>" PageFilter="true" />
<asp:Repeater runat="server" DataSourceID="children">
    <HeaderTemplate><ul></HeaderTemplate>
    <ItemTemplate>
        <li><asp:HyperLink Text='<%# Eval("Title") %>' NavigateUrl='<%# Eval("Url") %>' runat="server" /></li>
    </ItemTemplate>
    <FooterTemplate></ul></FooterTemplate>
</asp:Repeater>