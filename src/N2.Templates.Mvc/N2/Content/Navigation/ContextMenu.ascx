<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContextMenu.ascx.cs" Inherits="N2.Edit.Navigation.ContextMenu" %>
<script src="../../Resources/Js/ContextMenu.js?v2" type="text/javascript" ></script>
<div id="contextMenu">
    <asp:PlaceHolder ID="plhMenuItems" runat="server" />
</div>

<script type="text/javascript">
	function setUpContextMenu(container){
        jQuery("a[target=preview]", container).click(function(){
            n2nav.setupToolbar(this.rel);
        }).bind("contextmenu", function(){
            n2nav.setupToolbar(this.rel);
        })
        .n2contextmenu("#contextMenu");
	};
    jQuery(document).ready(function(){
		setUpContextMenu("#nav");
    });
</script>
