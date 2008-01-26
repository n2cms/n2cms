<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="N2.Edit.Toolbar" %>
<div id="top">
    <div id="toolbar" class="toolbar">
        <div class="navigation"><div class="left"><div class="right">
            <asp:PlaceHolder runat="server" ID="plhNavigation" />
        </div></div></div>
        <div class="preview"><div class="left"><div class="right">
            <asp:PlaceHolder runat="server" ID="plhFrame" />
        </div></div></div>

        <a id="logo" href="http://n2cms.com/" title="Go to the home of N2 CMS"><img src="img/toolbar_logo.gif" alt="N2" /></a>
        <asp:LoginStatus ID="logout" runat="server" CssClass="logout command" LogoutText="<img src='img/ico/key.gif' alt='key'/>logout" meta:resourcekey="logout" />
	</div>
</div>
