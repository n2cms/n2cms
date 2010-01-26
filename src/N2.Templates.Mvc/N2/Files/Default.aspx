<%@ Page MasterPageFile="../Top.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Management.N2.Files.Default" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
	<edit:ToolbarPluginDisplay ID="NavigationPlugins" Area="Navigation" runat="server" />
    <edit:ToolbarPluginDisplay ID="FilesPlugins" Area="Files" runat="server" />
</asp:Content>
<asp:Content ID="cs" ContentPlaceHolderID="Subbar" runat="server">
    <edit:ToolbarPluginDisplay ID="OperationsPlugins" Area="Operations" runat="server" />
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<div id="splitter" class="content">
		<div id="leftPane" class="pane">
			<iframe id="navigationFrame" src="../Content/Navigation/Tree.aspx?location=files&root=<%= Engine.UrlParser.StartPage.Path %>" frameborder="0" name="navigation" class="frame"></iframe>
		</div>
		<div id="rightPane" class="pane">
			<iframe id="previewFrame" src="Start.aspx" frameborder="0" name="preview" class="frame"></iframe>
		</div>
	</div>
	
	<script type="text/javascript">
		window.name = "top";
		window.n2ctx.hasTop = function() { return true; }
		window.n2ctx.initToolbar();
		window.n2ctx.setupToolbar('<%= Selection.SelectedItem.Path %>', '<%= ResolveClientUrl(Selection.SelectedItem.Url) %>');
		window.n2ctx.location = "files";
		
		window.n2.frameManager.init();
		jQuery(document).ready(function() {
		    jQuery(".command").n2glow();
		    jQuery(".operations a").click(function(e) {
		        if (jQuery(document.body).is(".editSelected, .wizardSelected, .versionsSelected, .securitySelected, .exportimportSelected, .globalizationSelected, .linktrackerSelected, .usersSelected")) {
		            e.preventDefault();
		        };
		    });
		});
    </script>
</asp:Content>