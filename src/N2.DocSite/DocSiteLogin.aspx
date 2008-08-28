<%@ Page Language="C#" MasterPageFile="~/DocSite.Master" AutoEventWireup="true" CodeBehind="DocSiteLogin.aspx.cs" Inherits="N2.DocSite.DocSiteLogin" Title="N2.DocSite - Login" 
Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<asp:Content ID="Content4" ContentPlaceHolderID="contentPlaceholder" runat="server">
  <h2 id="docsite_page_header"><asp:Localize runat="server" ID="loginHeaderLocalize" Text="Administrative Login" meta:resourcekey="loginHeaderLocalize" /></h2>
  <div id="docsite_login" class="content_padding docsite_page_options">
    <asp:Login ID="login" runat="Server" OnAuthenticate="login_Authenticate" DisplayRememberMe="false" meta:resourcekey="loginResource1">
    </asp:Login>
  </div>
  <div id="docsite_page_footer">&nbsp;</div>
</asp:Content>