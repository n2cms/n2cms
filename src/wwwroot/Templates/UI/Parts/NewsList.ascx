<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsList.ascx.cs" Inherits="N2.Templates.UI.Parts.NewsList" %>
<n2:Hn Level='<%$ CurrentItem: TitleLevel %>' Visible="<%$ HasValue: Title %>" Text="<%$ CurrentItem: Title %>" runat="server" />
<n2:ItemDataSource id="idsNews" runat="server" />
<asp:Repeater runat="server" DataSourceID="idsNews">
    <HeaderTemplate><div class="sidelist"></HeaderTemplate>
    <ItemTemplate>
        <div class="item news i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
            <a href='<%# Eval("Url") %>' title='<%# Eval("Published") + ", " + Eval("Introduction") %>'><%# Eval("Title") %></a>
        </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
</asp:Repeater>
