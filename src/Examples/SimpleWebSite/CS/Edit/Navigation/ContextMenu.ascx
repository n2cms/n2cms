<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContextMenu.ascx.cs" Inherits="N2.Edit.Navigation.ContextMenu" %>
<script src="../Js/ContextMenu.js?v2" type="text/javascript" ></script>
<div id="contextMenu">
    <asp:PlaceHolder ID="plhMenuItems" runat="server" />
</div>

<script type="text/javascript">
    $(document).ready(function(){
        $("#nav a[@target=preview]").click(function(){
            n2nav.setupToolbar(this.href);
        }).bind("contextmenu", function(){
            n2nav.setupToolbar(this.href);
        })
        .n2contextmenu("#contextMenu");
    });
</script>
