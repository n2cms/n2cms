<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tree.aspx.cs" Inherits="N2.Edit.Navigation.Tree" meta:resourceKey="treePage" %>
<%@ Register TagPrefix="nav" TagName="ContextMenu" Src="ContextMenu.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" class="navigation <%= Server.HtmlEncode(Request.QueryString["location"]) %>Location">
    <head runat="server">
        <title>Navigation</title>
        <asp:PlaceHolder runat="server">
		<link rel="stylesheet" href="<%=MapCssUrl("all.css")%>" type="text/css" />
		<link rel="stylesheet" href="<%=MapCssUrl("framed.css")%>" type="text/css" />
		</asp:PlaceHolder>
        <script src="../../Resources/Js/jquery.ui.ashx" type="text/javascript" ></script>
		<script src="../../Resources/Js/ContextMenu.js?v2" type="text/javascript" ></script>
		<style>
			.nodeOption { display:none; padding:5px; }
			.filesselectionLocation .DirectorySelected .onDirectorySelected
			{
				display:block;
			}
		</style>
    </head>
<body class="framed">
    <form id="form1" runat="server">
		<div class="onDirectorySelected nodeOption">
			<input id="inputLocation" type="hidden" runat="server" class="uploadDirectoryLocation" />
			<input id="inputFile" type="file" runat="server" onchange="this.form.submit();" />
			<%--document.getElementById('<%= btnUpload.ClientID %>').click();--%>
			<%--<asp:Button ID="btnUpload" runat="server" OnCommand="btnUpload_Command" />--%>
		</div>
        <div id="nav" class="tree nav focusGroup">
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

        			if (!$a.is("a") || $a.is(".toggler"))
        				return;

        			var handler = n2nav.handlers[$a.attr("data-type")] || n2nav.handlers["fallback"];
        			handler.call($a[0], e);

        			document.body.className = document.body.className.replace(/\w+Selected ?/g, $a.attr("data-type") + "Selected");
        		});


        		window.onNavigating = function(options) {
        			$("a[data-path=" + options.path + "]", this.document).each(function() {
        				$(this).trigger("click");
        				options.showNavigation = function() { };
        			});
        		};

        		toDraggable(jQuery("#nav li li"));

        		$(".tree a.selected").each(function() { document.body.className += " " + $(this).attr("data-type") + "Selected"; });
        	});
        </script>
        
        <% if (Request["location"] == "filesselection" || Request["location"] == "contentselection" || Request["location"] == "selection") { %>
        <script src="../../Resources/tiny_mce/tiny_mce_popup.js" type="text/javascript"></script>
        <script type="text/javascript">
        	var updateOpenerWithUrlAndClose = function(relativeUrl) {
        		function selectIn(opener) {
        			if (opener.onFileSelected && opener.srcField)
        				opener.onFileSelected(relativeUrl);
        			else
        				opener.document.getElementById('<%= Request["tbid"] %>').value = relativeUrl;
        		}
        		$.cookie('lastSelection', relativeUrl);
        		if (window.opener) {
        			selectIn(window.opener);
        			window.close();
        		} else if (typeof tinyMCEPopup != "undefined" && tinyMCEPopup.getWin()) {
        			selectIn(tinyMCEPopup.getWin());
        			tinyMCEPopup.close();
        		}
        	}
        	var updateOpenerAndClose = function(e) {
        		var relativeUrl = $(this).attr("data-url");
        		updateOpenerWithUrlAndClose(relativeUrl);
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
        		if ($(this).attr("data-type") == "Directory")
        			$(".uploadDirectoryLocation").attr("value", $(this).attr("data-url"));
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

		<script type="text/javascript">
			$(document).ready(function() {
				$(".focusGroup a:not(.toggler):visible").n2keyboard({
					left: function(e, ctx) {
						var el = ctx.focused().closest(".folder-open")
							.children(".toggler").click()
							.siblings("a:not(.toggler)");
						ctx.focus(el);
					},
					right: function(e, ctx) {
						ctx.focused().siblings(".folder-close > .toggler").click();
					},
					esc: function() { $("#contextMenu").n2hide(); },
					del: function() { $("#contextMenu a.delete").n2trigger(); },
					c: function() { $("#contextMenu a.copy").n2trigger(); },
					n: function() { $("#contextMenu a.new").n2trigger(); },
					v: function() { $("#contextMenu a.paste").n2trigger(); },
					x: function() { $("#contextMenu a.move").n2trigger(); },
					enter: function() { ctx.focused().click(); }
				}, ".selected");
			});
		</script>
	    
        <nav:ContextMenu id="cm" runat="server" />
    </form>
</body>
</html>
