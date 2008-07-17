<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Templates.Wiki.UI.EditArticle" Title="Untitled Page" %>
<%@ Register TagPrefix="parts" TagName="EditArticle" Src="Parts/EditArticle.ascx" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <div class="controls"><a href="<%= CurrentPage.Url %>">View</a>
    | Modify
    | <a href="<%= CurrentPage.AppendUrl("History") %>">History</a>
    </div>
    <div class="message"><n2:EditableDisplay runat="server" PropertyName="ModifyText" Path="<%$ CurrentPage: WikiRoot.Path %>" /></div>
    <parts:EditArticle runat="server" IsNew="false" />
</asp:Content>
