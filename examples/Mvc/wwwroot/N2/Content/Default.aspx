<%@ Page MasterPageFile="../Top.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Default" Title="Edit" meta:resourcekey="DefaultResource" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:ToolbarPluginDisplay ID="NavigationPlugins" Area="Navigation" runat="server" />
    <edit:ToolbarPluginDisplay ID="PreviewPlugins" Area="Preview" runat="server" />
</asp:Content>

<asp:Content ID="cs" ContentPlaceHolderID="Subbar" runat="server">
    <edit:ToolbarPluginDisplay ID="OperationsPlugins" Area="Operations" runat="server" />
    <edit:ToolbarPluginDisplay ID="OptionsPlugins" Area="Options" runat="server" />
</asp:Content>

<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<div id="splitter" class="content">
		<div id="leftPane" class="pane">
			<iframe id="navigationFrame" src="<%= GetNavigationUrl(Selection.SelectedItem) %>" frameborder="0" name="navigation" class="frame"></iframe>
		</div>
		<div id="rightPane" class="pane">
			<iframe id="previewFrame" src="<%= GetPreviewUrl(Selection.SelectedItem) %>" frameborder="0" name="preview" class="frame"></iframe>
		</div>
	</div>

	<script type="text/javascript">
		window.name = "top";
		n2ctx.hasTop = function() { return true; }
		n2ctx.initToolbar();
		n2ctx.update({ path: '<%= SelectedPath %>', previewUrl: '<%= ResolveClientUrl(SelectedUrl) %>' });
		n2ctx.location = "content";

		window.n2.frameManager.init();
		jQuery(document).ready(function() {
		    jQuery(".command").n2glow();
		});
    </script>
</asp:Content>