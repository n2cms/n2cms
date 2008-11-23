<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.SearchArticle" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
<asp:Content ID="Content9" ContentPlaceHolderID="TextContent" runat="server">
    <h1>Searching for '<%= CurrentArguments %>'</h1>
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
