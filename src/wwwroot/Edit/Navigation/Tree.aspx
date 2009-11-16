<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tree.aspx.cs" Inherits="N2.Edit.Navigation.Tree" meta:resourceKey="treePage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Navigation</title>
        <asp:PlaceHolder runat="server">
		<link rel="stylesheet" href="<%=MapCssUrl("all.css")%>" type="text/css" />
		<link rel="stylesheet" href="<%=MapCssUrl("framed.css")%>" type="text/css" />
		</asp:PlaceHolder>
        <script src="../Js/jquery.ui.ashx" type="text/javascript" ></script>
    </head>
<body class="edit navigation tree">
    <form id="form1" runat="server">

        <div id="nav" class="tree nav">
            <edit:Tree ID="siteTreeView" runat="server" Target="preview" />
        </div>
        <script type="text/javascript">
        	jQuery(document).ready(function() {
        		var dragMemory = null;
        		var onDrop = function(e, ui) {
        			var action = e.ctrlKey ? "copy" : "move";
        			var to = this.rel;
        			var from = dragMemory;
        			parent.preview.location = "../paste.aspx?action=" + action
											+ "&memory=" + encodeURIComponent(from)
											+ "&selected=" + encodeURIComponent(to);
        		};
        		var onStart = function(e, ui) {
        			dragMemory = this.rel;
        		};

        		var toDraggable = function(container) {
        			jQuery("a", container).draggable({
        				delay: 100,
        				cursorAt: { top: 8, left: 8 },
        				start: onStart,
        				helper: 'clone'
        			}).droppable({
        				accept: '#nav li li a',
        				hoverClass: 'droppable-hover',
        				tolerance: 'pointer',
        				drop: onDrop
        			});
        		}

        		jQuery("#nav").SimpleTree({
        			success: function(el) {
        				toDraggable(el);
        				n2nav.refreshLinks(el);
        				setUpContextMenu(el);
        			}
        		});
        		
        		toDraggable(jQuery("#nav li li"));
        	});
        </script>
        <nav:ContextMenu id="cm" runat="server" />
    </form>
</body>
</html>
