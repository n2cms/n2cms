<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Submit.aspx.cs" Inherits="N2.Templates.Wiki.UI.Views.SubmitArticle" Title="Untitled Page" %>
<%@ Import Namespace="N2.Templates.Wiki.UI.Views"%>
<%@ Register TagPrefix="parts" TagName="EditArticle" Src="../Parts/EditArticle.ascx" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <parts:EditArticle runat="server" IsNew="true" Text="<%$ CurrentPage: WikiRoot.SubmitText %>" />
</asp:Content>
