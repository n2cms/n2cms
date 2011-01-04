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
		<script src="Tree.js" type="text/javascript"></script>
		<style>
			.nodeOption { display:none; padding:5px; }
			.filesselectionLocation .DirectorySelected .onDirectorySelected
			{
				display:block;
			}
		</style>
    </head>
<body class="framed noneSelected">
    <form id="form1" runat="server">
		<div class="onDirectorySelected nodeOption">
			<input id="inputLocation" type="hidden" runat="server" class="uploadDirectoryLocation" />
			<input id="inputFile" type="file" runat="server" onchange="this.form.submit();" />
			<asp:Image ImageUrl="../../Resources/Icons/page_white_get.png" runat="server" style="vertical-align:middle" />
		</div>
        <div id="nav" class="tree nav focusGroup">
            <edit:Tree ID="siteTreeView" runat="server" Target="preview" />
        </div>

		<% if(Request["destinationType"] != null) {%>
        <script src="../../Resources/tiny_mce/tiny_mce_popup.js" type="text/javascript"></script>
		<%} %>
        
        <% if (Request["location"] == "filesselection" || Request["location"] == "contentselection" || Request["location"] == "selection") { %>
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
		
        <script type="text/javascript">
		<% if (Request["location"] == "filesselection") { %>
        	n2nav.handlers["fallback"] = function(e) {
        		e.preventDefault();
        		if ($(this).attr("data-type") == "File")
        			updateOpenerAndClose.call(this, e);
        		if ($(this).attr("data-type") == "Directory")
        			$(".uploadDirectoryLocation").attr("value", $(this).attr("data-url"));
        	};
    	<% } %>
		
		<% if (Request["location"] == "contentselection") { %>
			n2nav.handlers["fallback"] = function(e) {
				e.preventDefault();
				if ($(this).attr("data-id") != "0")
					updateOpenerAndClose.call(this, e);
			};
    	<% } %>
		
		<% if (Request["location"] == "files") { %>
        	var fallback = n2nav.handlers["fallback"];
        	n2nav.handlers["fallback"] = function(e) {
        		var type = $(this).attr("data-type");
        		if (type == "File" || type == "Directory" || type == "RootDirectory")
        			fallback.call(this, e);
    			else
    				e.preventDefault();
    		};
    		$("a.selected").focus();
    	<% } %>
        </script>

        <nav:ContextMenu id="cm" runat="server" />
    </form>
</body>
</html>
