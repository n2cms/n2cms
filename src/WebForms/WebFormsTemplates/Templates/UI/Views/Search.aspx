<%@ Page Language="C#" MasterPageFile="../Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.UI.Views.Search" Title="Untitled Page" %>
<asp:Content ID="c" ContentPlaceHolderID="PostContent" runat="server">
    <asp:TextBox ID="txtQuery" runat="server"></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
    
    <asp:Label Text='<%# string.Format("{0} pages found.", TotalCount) %>' runat="server" />
    <asp:Repeater ID="rptHits" runat="server" DataSource='<%# Hits %>'>
        <HeaderTemplate><div class="list"></HeaderTemplate>
        <ItemTemplate>
            <div class="item hit cf i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
                <a href='<%# Eval("Url") %>'><%# Eval("Title") %></a>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
</asp:Content>
