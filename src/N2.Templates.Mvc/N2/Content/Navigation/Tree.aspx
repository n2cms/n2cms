<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tree.aspx.cs" Inherits="N2.Edit.Navigation.Tree" meta:resourceKey="treePage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Navigation</title>
        <asp:PlaceHolder runat="server">
		<link rel="stylesheet" href="<%=MapCssUrl("all.css")%>" type="text/css" />
		<link rel="stylesheet" href="<%=MapCssUrl("framed.css")%>" type="text/css" />
		</asp:PlaceHolder>
        <script src="../../Resources/Js/jquery.ui.ashx" type="text/javascript" ></script>
		<script src="../../Resources/Js/ContextMenu.js?v2" type="text/javascript" ></script>
    </head>
<body class="framed navigation tree">
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
        				setUpContextMenu(el);
        			}
        		});
        		jQuery("#nav").click(function(e) {
        			var $a = $(e.target);
        			if (!$a.is("a"))
        				$a = $a.closest("a");

        			if (!$a.is("a"))
        				return;

        			var handler = n2nav.handlers[$a.attr("data-type")] || n2nav.handlers["fallback"];
        			handler.call($a[0], e);
        		});
        		toDraggable(jQuery("#nav li li"));
        	});
        </script>
        <% if (Request["location"] == "filesselection" || Request["location"] == "contentselection" || Request["location"] == "selection") { %>
        <script type="text/javascript">
        	var updateOpenerAndClose = function(e) {
        		if (window.opener) {
        			var relativeUrl = $(this).attr("data-url");
        			if (window.opener.onFileSelected && window.opener.srcField)
        				window.opener.onFileSelected(relativeUrl);
        			else
        				window.opener.document.getElementById('<%= Request["tbid"] %>').value = relativeUrl;
        			window.close();
        		}
        		e.preventDefault();
        	};
        	n2nav.handlers["fallback"] = updateOpenerAndClose;
        </script>
        <% } %>
		
		<% if (Request["location"] == "filesselection") { %>
        <script type="text/javascript">
        	n2nav.handlers["fallback"] = function(e) {
        		e.preventDefault();
        		if ($(this).attr("data-type") == "File")
        			updateOpenerAndClose.call(this, e);
        	};
        </script>
    	<% } %>
		
		<% if (Request["location"] == "contentselection") { %>
        <script type="text/javascript">
			n2nav.handlers["fallback"] = function(e) {
				e.preventDefault();
				if ($(this).attr("data-id") != "0")
					updateOpenerAndClose.call(this, e);
			};
        </script>
    	<% } %>
		
		<% if (Request["location"] == "files") { %>
        <script type="text/javascript">
        	var fallback = n2nav.handlers["fallback"];
        	n2nav.handlers["fallback"] = function(e) {
        		var type = $(this).attr("data-type");
        		if (type == "File" || type == "Directory" || type == "RootDirectory")
        			fallback.call(this, e);
    			else
    				e.preventDefault();
    		};
    		$("a.selected").focus();
        </script>
    	<% } %>
    	
        <nav:ContextMenu id="cm" runat="server" />
    </form>
</body>
</html>
