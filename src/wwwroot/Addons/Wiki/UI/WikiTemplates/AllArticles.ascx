<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AllArticles.ascx.cs" Inherits="N2.Addons.Wiki.UI.WikiTemplates.AllArticles" %>
<asp:Repeater runat="server" ID="rptArticles" runat="server">
    <ItemTemplate>
        <div class="item"><a href="<%# Eval("Url") %>">
            <span class="date"><%# Eval("Published") %></span>
            <%# Eval("Title") %>, <%# Eval("SavedBy") %>
        </a>
        </div>
    </ItemTemplate>
</asp:Repeater>