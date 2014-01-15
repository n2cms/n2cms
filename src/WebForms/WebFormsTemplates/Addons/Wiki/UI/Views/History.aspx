<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.History" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
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
                <a href="<%# N2.Web.Url.Parse(CurrentPage.Url).AppendSegment("version").AppendSegment(Eval("ID").ToString(), true) %>">
                    <%# Eval("Title") %>
                </a>
                 by <%# Eval("SavedBy") %>
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
