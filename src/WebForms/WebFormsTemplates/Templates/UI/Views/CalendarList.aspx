<%@ Page MasterPageFile="../Layouts/Top+SubMenu.master" Language="C#" AutoEventWireup="true" CodeBehind="Container.aspx.cs" Inherits="N2.Templates.UI.Views.CalendarList" %>
<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
    <n2:Repeater ID="rc" runat="server">
        <HeaderTemplate><div class="list"></HeaderTemplate>
        <ItemTemplate>
            <div class="item i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
                <span class="date"><%# Eval("EventDateString") %></span>
                <a href='<%# Eval("Url") %>'><%# Eval("Title") %></a>
                <p><%# Eval("Introduction") %></p>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
        <EmptyTemplate><%= GetLocalResourceObject("NoItemsFound") %></EmptyTemplate>
    </n2:Repeater>
</asp:Content>
