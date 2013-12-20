<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.SearchArticle" Title="Untitled Page" %>
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

<asp:Content ID="sc" ContentPlaceHolderID="Sidebar" runat="server">
    <div id="extras" class="secondary">
        <n2:DroppableZone ID="zrr" ZoneName="RecursiveRight" runat="server" />
        <n2:DroppableZone ID="zr" ZoneName="Right" runat="server" />
        <n2:DroppableZone ID="zsr" ZoneName="SiteRight" runat="server" Path="~/" />
    </div>
</asp:Content>
