<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="N2.Templates.Wiki.UI.History" Title="Untitled Page" %>
<asp:Content ID="Content9" ContentPlaceHolderID="TextContent" runat="server">
    <div class="controls"><a href="<%= CurrentPage.Url %>">View</a>
    | <a href="<%= CurrentPage.AppendUrl("Modify") %>">Modify</a>
    | History
    </div>
    <h1>History of <%= CurrentPage.Title %></h1>
    <div class="message"><n2:EditableDisplay ID="dText" runat="server" PropertyName="HistoryText" Path="<%$ CurrentPage: WikiRoot.Path %>" /></div>
    <asp:Repeater runat="server" ID="rptArticles" runat="server">
        <HeaderTemplate>
            <div title="<%# CurrentPage.ID %>" class="item">
                <span><%# CurrentPage.Updated %></span>
                <a href="<%# CurrentPage.Url %>">
                    <%# CurrentPage.Title %>
                </a>
                 by <%# CurrentPage.SavedBy %>
            </div>
        </HeaderTemplate>
        <ItemTemplate>
            <div title="<%# Eval("ID") %>" class="item">
                <span><%# Eval("Expires")%></span>
                <a href="<%# Eval("Url") %>">
                    <%# Eval("Title") %>
                </a>
                 by <%# Eval("SavedBy") %>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
