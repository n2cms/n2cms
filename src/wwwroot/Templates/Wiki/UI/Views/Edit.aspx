<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Templates.Wiki.UI.EditArticle" Title="Untitled Page" %>
<%@ Register TagPrefix="parts" TagName="EditArticle" Src="../Parts/EditArticle.ascx" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <div class="controls"><a href="<%= CurrentPage.Url %>">View</a>
    | Modify
    | <a href="<%= CurrentPage.AppendUrl("History") %>">History</a>
    </div>
    <parts:EditArticle runat="server" IsNew="false" Text="<%$ CurrentPage: WikiRoot.ModifyText %>" />
</asp:Content>
