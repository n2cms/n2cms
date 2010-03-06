<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsBox.ascx.cs" Inherits="N2.Templates.UI.Parts.NewsBox" %>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box runat="server">
    <n2:ItemDataSource id="idsNews" runat="server" />
    <asp:Repeater ID="Repeater1" runat="server" DataSourceID="idsNews">
        <HeaderTemplate><div class="sidelist"></HeaderTemplate>
        <ItemTemplate>
            <div class="item news i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
                <a href='<%# Eval("Url") %>' title='<%# Eval("Published") + ", " + Eval("Introduction") %>'><%# Eval("Title") %></a>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
</n2:Box>