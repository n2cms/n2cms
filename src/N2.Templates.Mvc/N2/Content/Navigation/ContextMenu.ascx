<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContextMenu.ascx.cs" Inherits="N2.Edit.Navigation.ContextMenu" %>
<div id="contextMenu" class="focusGroup">
    <asp:PlaceHolder ID="plhMenuItems" runat="server" />
</div>

<script type="text/javascript">
	function setUpContextMenu(container){
		jQuery("a[target=preview]", container).click(function() {
			n2nav.setupToolbar(this.rel);
		}).focus(function() {
			n2nav.setupToolbar(this.rel);
		}).bind("contextmenu", function() {
            n2nav.setupToolbar(this.rel);
        })
        .n2contextmenu("#contextMenu");
	};
    jQuery(document).ready(function(){
		setUpContextMenu("#nav");
    });
</script>
