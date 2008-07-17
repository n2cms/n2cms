<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Submit.aspx.cs" Inherits="N2.Templates.Wiki.UI.SubmitArticle" Title="Untitled Page" %>
<%@ Register TagPrefix="parts" TagName="EditArticle" Src="Parts/EditArticle.ascx" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <div class="message"><n2:EditableDisplay runat="server" PropertyName="SubmitText" Path="<%$ CurrentPage: WikiRoot.Path %>" /></div>
    <parts:EditArticle runat="server" IsNew="true" />
</asp:Content>
