<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Default" Title="Edit" meta:resourcekey="DefaultResource" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		<title>Edit</title>
		
		<asp:PlaceHolder runat="server">
		<link rel="stylesheet" href="<%=MapCssUrl("all.css")%>" type="text/css" />
		<link rel="stylesheet" href="<%=MapCssUrl("default.css")%>" type="text/css" />
		<!--[if IE 6]>
		<link rel="stylesheet" href="<%=MapCssUrl("IE6.css")%>" type="text/css" />
		<![endif]-->
		</asp:PlaceHolder>
	</head>
	<body id="default" class="edit">
		<div class="frameForm">
            <div id="top">
                <div id="toolbar" class="header cf">
        		    <edit:ToolbarPluginDisplay ID="NavigationPlugins" Area="Navigation" runat="server" />
                    <edit:ToolbarPluginDisplay ID="PreviewPlugins" Area="Preview" runat="server" />
                </div>

                <asp:HyperLink ID="logout" runat="server" CssClass="logout" NavigateUrl="Login.aspx?logout=true" Text="Log out" />
                <asp:HyperLink ID="hlLogo" runat="server" SkinID="Logo" CssClass="logo" NavigateUrl="http://n2cms.com" Text="To the home of N2 CMS" ImageUrl="~/Edit/img/n2.png" />
                
                <div class="subbar cf">
			        <edit:ToolbarPluginDisplay ID="OperationsPlugins" Area="Operations" runat="server" />
			        <edit:ToolbarPluginDisplay ID="OptionsPlugins" Area="Options" runat="server" />
			        <div class="blinds"></div>
                </div>
			</div>
            
			<div id="splitter" class="content">
				<div id="leftPane" class="pane">
					<iframe id="navigationFrame" src="<%= GetNavigationUrl(SelectedItem) %>" frameborder="0" name="navigation" class="frame"></iframe>
				</div>
				<div id="rightPane" class="pane">
                    <iframe id="previewFrame" src="<%= GetPreviewUrl(SelectedItem) %>" frameborder="0" name="preview" class="frame"></iframe>
				</div>
			</div>
		</div>
		<form id="form1" runat="server"></form>

		<script type="text/javascript">
			window.name = "top";
			window.n2ctx.hasTop = function() { return true; }
			window.n2ctx.initToolbar();
			window.n2ctx.setupToolbar('<%= SelectedPath %>', '<%= ResolveClientUrl(SelectedUrl) %>');

			window.n2.frameManager.init();
			jQuery(document).ready(function() {
			    jQuery(".command").n2glow();
			    jQuery(".operations a").click(function(e) {
			        if (jQuery(document.body).is(".editSelected, .wizardSelected, .versionsSelected, .securitySelected, .exportimportSelected, .globalizationSelected, .linktrackerSelected, .usersSelected, .filemanagerSelected")) {
			            e.preventDefault();
			        };
			    });
			});
        </script>
	</body>
</html>
