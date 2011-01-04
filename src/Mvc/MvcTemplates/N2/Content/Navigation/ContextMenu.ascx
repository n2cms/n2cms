<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContextMenu.ascx.cs" Inherits="N2.Edit.Navigation.ContextMenu" %>
<div id="contextMenu" class="focusGroup">
    <asp:PlaceHolder ID="plhMenuItems" runat="server" />
</div>

<script type="text/javascript">
//<![CDATA[
	(function ($) {
		var targetSelector = "a[target=preview]";
		var handler = function (e) {
			$(e.target).closest(targetSelector).each(function (e) {
				n2nav.setupToolbar(this.rel);
			});
		};
		jQuery(document).ready(function () {
			$("#nav").click(handler)
				.focus(handler)
				.bind("contextmenu", handler)
				.n2contextmenu("#contextMenu", { target: targetSelector });
		});
	})(jQuery);
//]]>
</script>
