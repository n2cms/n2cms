<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="SubmitArticle.aspx.cs" Inherits="N2.Templates.Wiki.UI.SubmitArticle" Title="Untitled Page" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <h1>Title <asp:TextBox ID="txtTitle" runat="server" Text="<%$ CurrentPage: NewArticleName %>" /></h1>
    <p><n2:FreeTextArea ID="txtText" runat="server" TextMode="MultiLine" Theme="simple" style="height:200px;width:100%;" /></p>
    <p><asp:Button OnClick="Submit_Click" runat="server" Text="Submit" /></p>
</asp:Content>
<asp:Content ContentPlaceHolderID="Sidebar" runat="server">
    Some wiki code recognized:
    <br />
    [external link]
    <br />
    [link text|link href]
    <br />
    [[internal link]]
    <br />
    [[category: category name]]
    <br />
    {{latest}}
    <br />
    {{all}}
</asp:Content>