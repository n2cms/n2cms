<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditArticle.ascx.cs" Inherits="N2.Templates.Wiki.UI.Parts.EditArticle" %>
<asp:CustomValidator runat="server" CssClass="error" ID="cvAuthorized" Text="You are not authorized to modify this article." Display="Dynamic" />
<asp:PlaceHolder ID="phSubmit" runat="server">
    <n2:h1 ID="h1" runat="server" />
    <p><n2:FreeTextArea ID="txtText" runat="server" TextMode="MultiLine" Theme="simple" style="height:200px;width:100%;" /></p>
    <p><asp:Button OnClick="Submit_Click" runat="server" Text="Save" /></p>
</asp:PlaceHolder>