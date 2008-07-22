<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RssAggregator.ascx.cs" Inherits="N2.Templates.News.UI.RssAggregator" %>

<asp:Repeater ID="rptRss" runat="server">
    <HeaderTemplate><div class="sidelist"></HeaderTemplate>
    <ItemTemplate>
        <div class="news i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
            <a href='<%# Eval("Url") %>' title='<%# Eval("Published") + ", " + Eval("Introduction") %>'><%# Eval("Title") %></a>
        </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
</asp:Repeater>