<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Submit.aspx.cs" Inherits="N2.Templates.Wiki.UI.SubmitArticle" Title="Untitled Page" %>
<%@ Register TagPrefix="parts" TagName="EditArticle" Src="Parts/EditArticle.ascx" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <parts:EditArticle runat="server" IsNew="true" Text="<%$ CurrentPage: WikiRoot.SubmitText %>" />
</asp:Content>
