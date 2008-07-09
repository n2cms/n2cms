<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="SearchArticle.aspx.cs" Inherits="N2.Templates.Wiki.UI.SearchArticle" Title="Untitled Page" %>
<asp:Content ID="Content9" ContentPlaceHolderID="TextContent" runat="server">
    <h1><%= CurrentPage.Title %></h1>
    <n2:EditableDisplay runat="server" PropertyName="SearchText" Path="<%$ CurrentPage: WikiRoot.Path %>" />
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
