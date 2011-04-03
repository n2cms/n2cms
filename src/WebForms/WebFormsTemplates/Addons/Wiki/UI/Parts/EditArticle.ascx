<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditArticle.ascx.cs" Inherits="N2.Addons.Wiki.UI.Parts.EditArticle" %>
<asp:CustomValidator runat="server" CssClass="error" ID="cvAuthorized" Text="You are not authorized to modify this article." Display="Dynamic" />
<asp:PlaceHolder ID="phSubmit" runat="server">
    <n2:Hn Level="1" ID="h1" runat="server" />
    <asp:Panel ID="pnlMessage" runat="server" CssClass="message" />
    <p><n2:FreeTextArea ID="txtText" runat="server" TextMode="MultiLine" Theme="simple" style="height:250px;width:100%;" /></p>
    <p>
        <a style="float:right" href="#<%= pnlMessage.ClientID %>" onclick="$(this).hide();$('#<%= txtText.ClientID %>').animate({height:'550px'},200);">Enlarge</a>
        <asp:Button OnClick="Submit_Click" runat="server" Text="Save" />
    </p>
</asp:PlaceHolder>