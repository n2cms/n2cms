<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsList.ascx.cs" Inherits="N2.Templates.News.UI.NewsList" %>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box runat="server">
    <n2:ItemDataSource id="idsNews" runat="server" />
    <asp:Repeater runat="server" DataSourceID="idsNews">
        <HeaderTemplate><div class="sidelist"></HeaderTemplate>
        <ItemTemplate>
            <div class="news i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
                <a href='<%# Eval("Url") %>' title='<%# Eval("Published") + ", " + Eval("Introduction") %>'><%# Eval("Title") %></a>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
</n2:Box>