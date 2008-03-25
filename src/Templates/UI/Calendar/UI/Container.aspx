<%@ Page MasterPageFile="~/Layouts/Top+SubMenu.Master" Language="C#" AutoEventWireup="true" CodeBehind="Container.aspx.cs" Inherits="N2.Templates.Calendar.UI.Container" %>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <n2:Path ID="p1" runat="server" />
    <n2:EditableDisplay PropertyName="Title" runat="server" />
    <n2:EditableDisplay PropertyName="Text" runat="server" />
    <asp:Repeater ID="rc" runat="server">
        <HeaderTemplate><div class="list"></HeaderTemplate>
        <ItemTemplate>
            <div class="item i<%# Container.ItemIndex %> a<%# Container.ItemIndex % 2 %>">
                <span class="date"><%# Eval("EventDate")%></span>
                <a href='<%# Eval("Url") %>'><%# Eval("Title") %></a>
                <p class="intro"><%# Eval("Introduction") %></p>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
</asp:Content>