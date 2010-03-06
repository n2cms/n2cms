<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RssAggregator.ascx.cs" Inherits="N2.Templates.UI.Parts.RssAggregator" %>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box ID="Box1" runat="server">
    <n2:EditableDisplay runat="server" PropertyName="Text" />
    <asp:Repeater ID="rptRss" runat="server">
        <HeaderTemplate><div class="sidelist"></HeaderTemplate>
        <ItemTemplate>
            <div class="item news i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
                <a href='<%# Eval("Url") %>' title='<%# Eval("Published") %>'><%# Eval("Title") %></a>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
</n2:Box>