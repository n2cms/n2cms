<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="N2.Templates.Wiki.UI.SearchArticle" Title="Untitled Page" %>
<asp:Content ID="Content9" ContentPlaceHolderID="TextContent" runat="server">
    <h1>Searching for '<%= CurrentPage.ActionParameter %>'</h1>
    <n2:EditableDisplay ID="edText" runat="server" PropertyName="SearchText" Path="<%$ CurrentPage: WikiRoot.Path %>" />
    <asp:Repeater runat="server" ID="rptArticles" runat="server">
        <ItemTemplate>
            <div class="item"><a href="<%# Eval("Url") %>">
                <span class="date"><%# Eval("Published") %></span>
                <%# Eval("Title") %>, <%# Eval("SavedBy") %>
            </a>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
