<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="N2.Edit.Toolbar" %>
<div id="top">
    <div id="toolbar" class="toolbar">
        <div class="navigation area"><div class="left"><div class="right">
            <asp:PlaceHolder runat="server" ID="plhNavigation" />
        </div></div></div>
        <div class="preview area last"><div class="left"><div class="right">
            <asp:PlaceHolder runat="server" ID="plhFrame" />
        </div></div></div>

        <a id="logo" href="http://n2cms.com/" title="To the home of N2 CMS"><img src="img/n2.gif" alt="N2 CMS" /></a>
        <asp:LoginStatus ID="logout" runat="server" CssClass="logout" LogoutText="<img src='img/ico/key.gif' alt='key'/>logout" meta:resourcekey="logout" />
	</div>
</div>

<script type="text/javascript">
$(document).ready(function(){	
	var over = function(){
		$(this).addClass("hover");
	};
	var out = function(){
		$(this).removeClass("hover");
	}
	$(".toolbarItem", "#toolbar").hover(over,out);
});
</script>